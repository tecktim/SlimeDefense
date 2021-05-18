using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
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

		}

		internal void Click(float x, float y)
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
			_model.ClearCell(column, row);
		}
	}
}