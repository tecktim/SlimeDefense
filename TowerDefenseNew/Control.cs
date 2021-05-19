using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using Zenseless.OpenTK;
using TowerDefenseNew.GameObjects;
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

		}

		internal void Click(float x, float y, KeyboardState keyboard)
		{
			var cam = _view.Camera;
			var fromViewportToWorld = Transformation2d.Combine(cam.InvViewportMatrix, cam.CameraMatrix.Inverted());
			var pixelCoordinates = new Vector2(x, y);
			var world = pixelCoordinates.Transform(fromViewportToWorld);
			Console.WriteLine($"{world}");
			if (world.X < 0 || _model.Grid.Columns < world.X) return;
			if (world.Y < 0 || _model.Grid.Rows < world.Y) return;
			var column = (int)Math.Truncate(world.X);
			var row = (int)Math.Truncate(world.Y);
			Console.WriteLine($"{column}, {row}");
			if(_model.CheckCell(column, row) == Grid.CellType.Sniper)
            {
				//Sell Sniper
				_model.ClearCell(column, row);
				Console.WriteLine("removed sniper");
			}
			if (_model.CheckCell(column, row) == Grid.CellType.Rifle)
			{
				//Sell Rifle
				_model.ClearCell(column, row);
				Console.WriteLine("removed rifle");
			}
			if(_model.CheckCell(column, row) == Grid.CellType.Empty)
            {
                if (keyboard.IsKeyDown(Keys.D1)) {
					_model.PlaceSniper(column, row);
					_model.towers.Add(new Tower(new Vector2(column, row), 9f, 10, 1000, _model.enemies, _model.bullets));
					Console.WriteLine("placed sniper");
                }
				if (keyboard.IsKeyDown(Keys.D2))
				{
					_model.PlaceRifle(column, row);
					_model.towers.Add(new Tower(new Vector2(column, row), 3f, 5, 100, _model.enemies, _model.bullets));
					Console.WriteLine("placed rifle");
				}
				if (keyboard.IsKeyDown(Keys.D3))
				{
					if (_model.PlacePath(column, row)) {
						Console.WriteLine("placed path");
					}
                    else
                    {
						Console.WriteLine("did not place path");
					}
					Console.WriteLine("");

				}
			}

		}
	}
}