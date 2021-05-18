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
			var axisX = keyboard.IsKeyDown(Keys.PageDown) ? -1f : keyboard.IsKeyDown(Keys.PageUp) ? 1f : 0f;
			var camera = _view.Camera;
			// zoom
			var zoom = camera.Scale * (1 + deltaTime * axisX);
			zoom = MathHelper.Clamp(zoom, 10f, 20f);
			camera.Scale = zoom;
			
			// translate
			float axisLeftRight = keyboard.IsKeyDown(Keys.Left) ? -1.0f : keyboard.IsKeyDown(Keys.Right) ? 1.0f : 0.0f;
			float axisUpDown = keyboard.IsKeyDown(Keys.Down) ? -1.0f : keyboard.IsKeyDown(Keys.Up) ? 1.0f : 0.0f;
			var movement = deltaTime * new Vector2(axisLeftRight, axisUpDown);
			// convert movement from camera space into world space
			camera.Center += movement.TransformDirection(camera.CameraMatrix.Inverted());
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