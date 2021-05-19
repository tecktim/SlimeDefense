using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseNew;
using TowerDefenseNew.GameObjects;
using TowerDefenseNew.Grid;

namespace TowerDefenseNew
{
	internal class View
	{
		public View()
		{
			//TODO: Change the clear color of the screen.
			GL.ClearColor(Color4.CornflowerBlue); // set the clear color of the screen
			GL.ClearColor(Color4.Bisque);
		}
		internal Camera Camera { get; } = new Camera();

		internal List<Vector2> circlePoints = CreateCirclePoints(20);


		internal void Draw(Model model)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit); // clear the screen

			Camera.Draw();

			DrawGrid(model.Grid);

			foreach (Enemy enemy in model.enemies.ToList())
			{
				DrawEnemy(enemy);
			}

			foreach (Bullet bullet in model.bullets.ToList())
            {
				DrawBullet(bullet);
            }

		}

		private void DrawGrid(IReadOnlyGrid grid)
		{
			DrawGridLines(grid.Columns, grid.Rows);
			GL.Color4(Color4.Gray);
			for (int column = 0; column < grid.Columns; ++column)
			{
				for (int row = 0; row < grid.Rows; ++row)
				{
					if (CellType.Sniper == grid[column, row])
					{
						DrawCircle(new Vector2(column + 0.5f, row + 0.5f), 0.4f, Color4.RosyBrown);
					}
					if (CellType.Rifle == grid[column, row])
                    {
						DrawCircle(new Vector2(column + 0.5f, row + 0.5f), 0.4f, Color4.Blue);
					}
					if (CellType.Path == grid[column, row])
                    {
						DrawRectangle(new Vector2(column, row), new Vector2(1, 1), Color4.Transparent);
                    }
				}
			}
		}

		private void DrawEnemy(IReadOnlyCircle enemy)
        {
			DrawCircle(enemy.Center, enemy.Radius, Color4.Red);
        }

		private void DrawBullet(IReadOnlyCircle bullet)
        {
			DrawCircle(bullet.Center, bullet.Radius, Color4.Black);
        }

		private void DrawCircle(Vector2 center, float radius, Color4 color)
		{
			GL.Begin(PrimitiveType.Polygon);
			GL.Color4(color);
			foreach (var point in circlePoints)
			{
				GL.Vertex2(center + radius * point);
			}
			GL.End();
		}

		private void DrawRectangle(Vector2 min, Vector2 size, Color4 color)
		{
			var max = min + size;
			GL.Begin(PrimitiveType.Quads);
			GL.Color4(color);
			GL.Vertex2(min);
			GL.Vertex2(max.X, min.Y);
			GL.Vertex2(max);
			GL.Vertex2(min.X, max.Y);
			GL.End();
		}

		private static void DrawGridLines(int columns, int rows)
		{
			GL.Color4(Color4.White);
			GL.LineWidth(1.0f);
			GL.Begin(PrimitiveType.Lines);
			for (float x = 0; x < columns + 1; ++x)
			{
				GL.Vertex2(x, 0f);
				GL.Vertex2(x, rows);
			}
			for (float y = 0; y < rows + 1; ++y)
			{
				GL.Vertex2(0f, y);
				GL.Vertex2(columns, y);
			}
			GL.End();
		}

		/// <summary>
		/// Calculates points on a circle.
		/// </summary>
		/// <returns></returns>
		private static List<Vector2> CreateCirclePoints(int corners)
		{
			float delta = 2f * MathF.PI / corners;
			var points = new List<Vector2>();
			for (int i = 0; i < corners; ++i)
			{
				var alpha = i * delta;
				// step around the unit circle
				var x = MathF.Cos(alpha);
				var y = MathF.Sin(alpha);
				points.Add(new Vector2(x, y));
			}
			return points;
		}

		internal void Resize(int width, int height)
		{
			Camera.Resize(width, height);
		}

	}
}