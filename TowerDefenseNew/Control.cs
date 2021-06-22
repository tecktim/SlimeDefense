using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Linq;
using TowerDefenseNew.GameObjects;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
    internal class Control
    {
        private readonly Model _model;
        private readonly View _view;

        public Control(Model model, View view)
        {
            _model = model;
            _view = view;
        }

        internal void Update(float deltaTime, KeyboardState keyboard)
        {
            var axisX = keyboard.IsKeyDown(Keys.PageDown) ? -1f : keyboard.IsKeyDown(Keys.PageUp) ? 1f : 0f;
            var camera = _view.GameCamera;
            // zoom
            var zoom = camera.Scale * (1 + deltaTime * axisX);
            zoom = MathHelper.Clamp(zoom, 5f, 20f);
            camera.Scale = zoom;

            // translate
            float axisLeftRight = keyboard.IsKeyDown(Keys.D) ? -1.0f : keyboard.IsKeyDown(Keys.A) ? 1.0f : 0.0f;
            float axisUpDown = keyboard.IsKeyDown(Keys.W) ? -1.0f : keyboard.IsKeyDown(Keys.S) ? 1.0f : 0.0f;
            var movement = deltaTime * new Vector2(axisLeftRight, axisUpDown);
            // convert movement from camera space into world space
            camera.Center += movement.TransformDirection(camera.CameraMatrix.Inverted());
        }

        internal void RemovePath(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.R) && _model.waypoints.Count > 1 && !_model.placed)
            {
                Console.WriteLine("RRRR");
                _model.makeEmpty();
            }
        }

        internal void Click(float x, float y, KeyboardState keyboard)
        {
            var cam = _view.GameCamera;
            var fromViewportToWorld = Transformation2d.Combine(cam.InvViewportMatrix, cam.CameraMatrix.Inverted());
            var pixelCoordinates = new Vector2(x, y);
            var world = pixelCoordinates.Transform(fromViewportToWorld);
            if (world.X < 0 || _model.Grid.Columns < world.X) return;
            if (world.Y < 0 || _model.Grid.Rows < world.Y) return;
            var column = (int)Math.Truncate(world.X);
            var row = (int)Math.Truncate(world.Y);
            var cell = _model.CheckCell(column, row);
            if (_model.gameOver == false)
            {
                if ((cell == Grid.CellType.Sniper && keyboard.IsKeyDown(Keys.R)) || (cell == Grid.CellType.Rifle && keyboard.IsKeyDown(Keys.R)) || (cell == Grid.CellType.Bouncer && keyboard.IsKeyDown(Keys.R)))
                {
                    foreach (Tower tower in _model.towers.ToList())
                    {
                        if (tower.Center.X == column && tower.Center.Y == row)
                        {
                            _model.ClearCell(column, row, tower);
                            _model.towerCount--;
                        }
                        else continue;
                    }
                    return;
                }

                //Schauen ob Cell leer ist
                if (cell == Grid.CellType.Empty && _model.towerCount < 50)
                {
                    //Sniper kaufen
                    if (keyboard.IsKeyDown(Keys.D2))
                    {
                        if (cell != Grid.CellType.Empty) { return; }
                        else { _model.PlaceSniper(column, row); }
                        return;
                    }
                    //Rifle kaufen
                    if (keyboard.IsKeyDown(Keys.D1))
                    {
                        if (cell != Grid.CellType.Empty) { return; }
                        else { _model.PlaceRifle(column, row); }
                        return;
                    }
                    //Bouncer kaufen
                    if (keyboard.IsKeyDown(Keys.D3))
                    {
                        if (cell != Grid.CellType.Empty) { return; }
                        else { _model.PlaceBouncer(column, row); }
                        return;
                    }
                }
            }
        }

        internal void ShowRange(float x, float y, MouseButton mb)
        {
            var cam = _view.GameCamera;
            var fromViewportToWorld = Transformation2d.Combine(cam.InvViewportMatrix, cam.CameraMatrix.Inverted());
            var pixelCoordinates = new Vector2(x, y);
            var world = pixelCoordinates.Transform(fromViewportToWorld);
            if (world.X < 0 || _model.Grid.Columns < world.X) return;
            if (world.Y < 0 || _model.Grid.Rows < world.Y) return;
            var column = (int)Math.Truncate(world.X);
            var row = (int)Math.Truncate(world.Y);
            var cell = _model.CheckCell(column, row);

            if (_view.Window.IsMouseButtonDown(mb))
            {
                if (cell == Grid.CellType.Sniper && _model.cash >= 20)
                {
                    _view.sampleSniper = true;
                    _view.sampleColRow = new Vector2(column, row);
                }
                else _view.sampleSniper = false;
                if (cell == Grid.CellType.Rifle && _model.cash >= 5)
                {

                    _view.sampleRifle = true;
                    _view.sampleColRow = new Vector2(column, row);
                }
                else _view.sampleRifle = false;
                if (cell == Grid.CellType.Bouncer && _model.cash >= 40)
                {

                    _view.sampleBouncer = true;
                    _view.sampleColRow = new Vector2(column, row);

                }
                else _view.sampleBouncer = false;
            }
            else return;
        }

        internal void PlacePath(float x, float y, MouseButton mb)
        {
            var cam = _view.GameCamera;
            var fromViewportToWorld = Transformation2d.Combine(cam.InvViewportMatrix, cam.CameraMatrix.Inverted());
            var pixelCoordinates = new Vector2(x, y);
            var world = pixelCoordinates.Transform(fromViewportToWorld);
            if (world.X < 0 || _model.Grid.Columns < world.X) return;
            if (world.Y < 0 || _model.Grid.Rows < world.Y) return;
            var column = (int)Math.Truncate(world.X);
            var row = (int)Math.Truncate(world.Y);
            var cell = _model.CheckCell(column, row);
            if (cell == Grid.CellType.Empty)
            {
                //Path setzen
                if (_view.Window.IsMouseButtonDown(mb))
                {
                    if (cell != Grid.CellType.Empty) {  return; }
                    else
                    {
                        if (_model.PlacePath(column, row))
                        {
                            return;
                        }
                    }
                    _view.samplePath = false;
                    return;
                }
                else
                {
                    _view.samplePath = true;
                    _view.sampleColRow = new Vector2(column, row);
                }
            }
        }

        internal void ShowTowerSample(float x, float y, KeyboardState keyboard)
        {
            var cam = _view.GameCamera;
            var fromViewportToWorld = Transformation2d.Combine(cam.InvViewportMatrix, cam.CameraMatrix.Inverted());
            var pixelCoordinates = new Vector2(x, y);
            var world = pixelCoordinates.Transform(fromViewportToWorld);
            if (world.X < 0 || _model.Grid.Columns < world.X) return;
            if (world.Y < 0 || _model.Grid.Rows < world.Y) return;
            var column = (int)Math.Truncate(world.X);
            var row = (int)Math.Truncate(world.Y);
            var cell = _model.CheckCell(column, row);

            if (cell == Grid.CellType.Empty)
            {
                if (keyboard.IsKeyDown(Keys.D2))
                {
                    if (cell != Grid.CellType.Empty && _model.cash >= 20) { return; }
                    else
                    {
                        if (_model.cash >= 20)
                        {
                            _view.sampleSniper = true;
                            _view.sampleColRow = new Vector2(column, row);
                        }
                    }//Snake
                    return;
                }
                else _view.sampleSniper = false;
                if (keyboard.IsKeyDown(Keys.D1))
                {
                    if (cell != Grid.CellType.Empty && _model.cash >= 5) { return; }
                    else
                    {
                        if (_model.cash >= 5)
                        {
                            _view.sampleRifle = true;
                            _view.sampleColRow = new Vector2(column, row);
                        }
                    }//Snake 
                    return;
                }
                else _view.sampleRifle = false;
                if (keyboard.IsKeyDown(Keys.D3))
                {
                    if (cell != Grid.CellType.Empty) { return; }
                    else 
                    {
                        if (_model.cash >= 40)
                        {
                            _view.sampleBouncer = true;
                            _view.sampleColRow = new Vector2(column, row);
                        }
                    }//Snake 
                    return;
                }
                else _view.sampleBouncer = false;
            }

        }
    }
}
