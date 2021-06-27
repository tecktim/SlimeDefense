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
        private Grid.CellType cell;
        private int column;
        private int row;

        public Control(Model model, View view)
        {
            _model = model;
            _view = view;
        }

        internal void Update(float deltaTime, KeyboardState keyboard)
        {
            Camera camera = CheckForZoom(deltaTime, keyboard);
            CheckForTranslation(deltaTime, keyboard, camera);
        }

        private void CheckForTranslation(float deltaTime, KeyboardState keyboard, Camera camera)
        {
            if (_view.GameCamera.Center.Y >= -30f || _view.GameCamera.Center.Y <= 0f || _view.GameCamera.Center.X >= -1f || _view.GameCamera.Center.X <= -54f)
            {
                // translate
                float axisLeftRight = keyboard.IsKeyDown(Keys.D) ? -1.0f : keyboard.IsKeyDown(Keys.A) ? 1.0f : 0.0f;
                float axisUpDown = keyboard.IsKeyDown(Keys.W) ? -1.0f : keyboard.IsKeyDown(Keys.S) ? 1.0f : 0.0f;
                var movement = deltaTime * new Vector2(axisLeftRight, axisUpDown);
                // convert movement from camera space into world space
                camera.Center += movement.TransformDirection(camera.CameraMatrix.Inverted());
                if (Math.Floor(_view.GameCamera.Center.Y) == -31f)
                {
                    _view.GameCamera.Center = new Vector2(_view.GameCamera.Center.X, -30f);
                }
                if (Math.Floor(_view.GameCamera.Center.Y) == 0f)
                {
                    _view.GameCamera.Center = new Vector2(_view.GameCamera.Center.X, 0f);
                }
                if (Math.Floor(_view.GameCamera.Center.X) == -1f)
                {
                    _view.GameCamera.Center = new Vector2(-1f, _view.GameCamera.Center.Y);
                }
                if (Math.Floor(_view.GameCamera.Center.X) == -55f)
                {
                    _view.GameCamera.Center = new Vector2(-54f, _view.GameCamera.Center.Y);
                }
            }
        }

        private Camera CheckForZoom(float deltaTime, KeyboardState keyboard)
        {
            var axisX = keyboard.IsKeyDown(Keys.E) ? -1f : keyboard.IsKeyDown(Keys.Q) ? 1f : 0f;
            var camera = _view.GameCamera;
            // zoom
            var zoom = camera.Scale * (1 + deltaTime * axisX);
            zoom = MathHelper.Clamp(zoom, 2f, 11f);
            camera.Scale = zoom;
            return camera;
        }

        internal void RemovePath(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.R) && _model.waypoints.Count > 1 && !_model.placed)
            {
                _model.MakeEmpty();
            }
        }

        internal void Click(float x, float y, KeyboardState keyboard)
        {
            GetWorldCoordinates(x, y);
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
                            _view.removeIndicator = false;
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
            GetWorldCoordinates(x, y);
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
            GetWorldCoordinates(x, y);
            if (cell == Grid.CellType.Empty)
            {
                //Path setzen
                if (_view.Window.IsMouseButtonDown(mb))
                {
                    if (cell != Grid.CellType.Empty) { return; }
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

        internal void ShowRemoveIndicator(float x, float y, KeyboardState keyboard)
        {
            GetWorldCoordinates(x, y);

            if (cell == Grid.CellType.Rifle || cell == Grid.CellType.Sniper || cell == Grid.CellType.Bouncer)
            {
                if (keyboard.IsKeyDown(Keys.R))
                {
                    _view.removeIndicator = true;
                    _view.removeColRow = new Vector2(column, row);
                }
            }
            else _view.removeIndicator = false;
        }

        internal void ShowTowerSample(float x, float y, KeyboardState keyboard)
        {
            GetWorldCoordinates(x, y);

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

        internal void GetWorldCoordinates(float x, float y)
        {
            var cam = _view.GameCamera;
            var fromViewportToWorld = Transformation2d.Combine(cam.InvViewportMatrix, cam.CameraMatrix.Inverted());
            var pixelCoordinates = new Vector2(x, y);
            var world = pixelCoordinates.Transform(fromViewportToWorld);
            if (world.X < 0 || _model.Grid.Columns < world.X) return;
            if (world.Y < 0 || _model.Grid.Rows < world.Y) return;
            this.column = (int)Math.Truncate(world.X);
            this.row = (int)Math.Truncate(world.Y);
            this.cell = _model.CheckCell(column, row);
        }
    }
}
