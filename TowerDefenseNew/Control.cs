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
			//Sniper verkaufen
			if (_model.CheckCell(column, row) == Grid.CellType.Sniper && keyboard.IsKeyDown(Keys.D4))
            {
				//Sell Sniper hi dev brach
				_model.ClearCell(column, row, _model.sniperCost);
				Console.WriteLine("sold sniper, new balance: " + _model.cash);
				return;
			}
			//Rifle verkaufen
			if (_model.CheckCell(column, row) == Grid.CellType.Rifle && keyboard.IsKeyDown(Keys.D4))
			{
				//Sell Rifle
				_model.ClearCell(column, row, _model.rifleCost);
				Console.WriteLine("sold rifle, new balance: " + _model.cash);
				return;
			}
			//Schauen ob Cell leer ist
			if(_model.CheckCell(column, row) == Grid.CellType.Empty)
            {
				//Sniper kaufen
                if (keyboard.IsKeyDown(Keys.D1)) {
						_model.PlaceSniper(column, row);
					}
                }
				if (keyboard.IsKeyDown(Keys.D2))
				{
						_model.PlaceRifle(column, row);
				}
				if (keyboard.IsKeyDown(Keys.D3))
				{
					if (_model.PlacePath(column, row)) {
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
