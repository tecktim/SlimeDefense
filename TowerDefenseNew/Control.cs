using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using Zenseless.OpenTK;
using TowerDefenseNew.GameObjects;
using System.Linq;

namespace TowerDefenseNew
{
	internal class Control
	{
		private readonly Model _model;
		private readonly View _view;
		private bool placePath = true;

		public Control(Model model, View view)
		{
			_model = model;
			_view = view;
		}

		internal void Update(float deltaTime, KeyboardState keyboard)
		{
			if (keyboard.IsKeyReleased(Keys.D5))
			{
				_model.giveCash();
			}
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
			var cell = _model.CheckCell(column, row);
			Console.WriteLine(cell);

			if (keyboard.IsKeyDown(Keys.D0))
			{
				_model.switchGameOver(true);
			}
			if(_model.gameOver == false) { 
			//Sniper verkaufen
			if (cell == Grid.CellType.Sniper && keyboard.IsKeyDown(Keys.D4))
			{
				//Sell Sniper hi dev brach
				foreach (Tower tower in _model.towers.ToList())
				{
						_model.ClearCell(column, row, tower);
						Console.WriteLine("sold sniper, new balance: " + _model.cash);
				}
			}

			//Rifle verkaufen
			if (cell == Grid.CellType.Rifle && keyboard.IsKeyDown(Keys.D4))
			{
				//Sell Rifle
				foreach (Tower tower in _model.towers.ToList()) 
				{
						_model.ClearCell(column, row, tower);
						Console.WriteLine("sold rifle, new balance: " + _model.cash);
				}
				return;
			}

				//Schauen ob Cell leer ist
				if (cell == Grid.CellType.Empty)
				{

					//Sniper kaufen
					if (keyboard.IsKeyDown(Keys.D1))
					{
						if (cell != Grid.CellType.Empty) { return; }
						else { _model.PlaceSniper(column, row); }
						return;
					}
					//Rifle kaufen
					if (keyboard.IsKeyDown(Keys.D2))
					{
						if (cell != Grid.CellType.Empty) { return; }
						else { _model.PlaceRifle(column, row); }
						return;
					}
					//Path setzen
					if (keyboard.IsKeyDown(Keys.D3) && placePath == true)
					{
						for (int i = 0; i < 54; i++)
						{
							if (_model.CheckCell(i, row) == Grid.CellType.Sniper || _model.CheckCell(i, row) == Grid.CellType.Rifle)
							{
								placePath = false;
								break;
							}
						}
						if (placePath)
						{
							_model.PlacePath(column, row);
							placePath = false;
							return;
						}
						Console.WriteLine("");
					}

				}
			}
		}
	}
}
