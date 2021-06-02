using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseNew;
using TowerDefenseNew.GameObjects;
using TowerDefenseNew.Grid;
using TowerDefenseNew.Interfaces;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
	internal class View
	{
		private readonly Texture texExplosion;
		private readonly Texture texEnemy1;
		private readonly Texture texEnemy2;
		private readonly Texture texFont;
		//private readonly Texture texSniper;

		public View(GameWindow window)
		{
			//TODO: Change the clear color of the screen.
			GL.ClearColor(new Color4(220, 150, 30, 255));
			this.Window = window;


			var content = $"{nameof(TowerDefenseNew)}.Content.";

			texEnemy1 = TextureLoader.LoadFromResource(content + "laughEmoji.png");
			texEnemy2 = TextureLoader.LoadFromResource(content + "fuckedUpEmoji.png");
			//texSniper = TextureLoader.LoadFromResource(content + "sniperTower.png");
			texExplosion = TextureLoader.LoadFromResource(content + "explosion.png");
			texFont = TextureLoader.LoadFromResource(content + "null_terminator.png");

		}

		internal Camera Camera { get; } = new Camera();
        public GameWindow Window { get; }
        internal List<Vector2> circlePoints = CreateCirclePoints(20);

		internal void Draw(Model model)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit); // clear the screen

			Camera.Draw();


			DrawGrid(model.Grid, Color4.White);

			if (model.gameOver)
			{
				this.Window.Close();
			}

			try
			{
				foreach (Enemy enemy in model.enemies.ToList())
				{
					if (enemy != null)
					{
						//draw enemy
						if (enemy.health >= model.enemyHealth / 2)
						{
							GL.BindTexture(TextureTarget.Texture2D, texEnemy1.Handle);
							DrawCircleTexture(enemy, new Rect(0f, 0f, 1f, 1f));
						}else
                        {
							GL.BindTexture(TextureTarget.Texture2D, texEnemy2.Handle);
							DrawCircleTexture(enemy, new Rect(0f, 0f, 1f, 1f));
                        }
					}
				}

				foreach (Bullet bullet in model.bullets.ToList())
				{
					DrawBullet(bullet);
				}

				DrawExplosion(model.explosions);
			}
			catch (System.ArgumentException)
            {
				Console.WriteLine("View.Draw exception");
            }


			GL.BindTexture(TextureTarget.Texture2D, texFont.Handle); // bind font texture
			DrawText($"{model.cash}$", -.99f, -0.99f, 3f);
		}


		private void DrawText(string text, float x, float y, float size)
		{
			GL.Color4(Color4.White);
			const uint firstCharacter = 32; // the ASCII code of the first character stored in the bitmap font
			const uint charactersPerColumn = 12; // how many characters are in each column
			const uint charactersPerRow = 8; // how many characters are in each row
			var rect = new Rect(x, y, size, size); // rectangle of the first character
			foreach (var spriteId in SpriteSheetTools.StringToSpriteIds(text, firstCharacter))
			{
				//TODO: Calculate the texture coordinates of the characters letter from the bitmap font texture
				//TODO: Draw a rectangle at the characters relative position
				var texCoords = SpriteSheetTools.CalcTexCoords(spriteId, charactersPerRow, charactersPerColumn);
				DrawRectangleTexture(rect, texCoords);
				rect.MinX += rect.SizeX;
			}
		}

		private void DrawGrid(IReadOnlyGrid grid, Color4 color)
		{
			DrawGridLines(grid.Columns, grid.Rows);
			GL.Color4(color);
			for (int column = 0; column < grid.Columns; ++column)
			{
				for (int row = 0; row < grid.Rows; ++row)
				{
					if (CellType.Sniper == grid[column, row])
					{
						DrawCircle(new Vector2(column + 0.5f, row + 0.5f), 0.4f, new Color4(32, 44 ,89, 255));
					}
					if (CellType.Rifle == grid[column, row])
                    {
						DrawCircle(new Vector2(column + 0.5f, row + 0.5f), 0.4f, new Color4(115, 147, 126, 255));
					}
					if (CellType.Path == grid[column, row])
                    {
						DrawRectangle(new Vector2(column, row), new Vector2(1, 1), new Color4(206, 185, 146, 255));
                    }
				}
			}
		}

		private void DrawBullet(IReadOnlyCircle bullet)
        {
			try
			{
				//DrawCircle(bullet.Center, bullet.Radius, Color4.Black);
				GL.BindTexture(TextureTarget.Texture2D, texEnemy1.Handle);
				DrawCircleTexture(bullet, new Rect(0f, 0f, 1f, 1f));
			}
			catch (System.NullReferenceException)
			{
				Console.WriteLine("DrawBullet NullReferenceException");
			}
		}

		private void DrawExplosion(IEnumerable<Explosion> explosions)
		{
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Blend);
			GL.Color4(Color4.White);
			// how many sprites are in each column and row
			const uint spritesPerColumn = 5;
			const uint spritesPerRow = 5;
			foreach (var explosion in explosions)
			{
				GL.BindTexture(TextureTarget.Texture2D, texExplosion.Handle);
				// calculate the current frame of an animation
				var spriteId = (uint)MathF.Round(explosion.NormalizedAnimationTime * (spritesPerRow * spritesPerColumn - 1));
				var texCoords = SpriteSheetTools.CalcTexCoords(spriteId, spritesPerRow, spritesPerColumn);
				DrawCircleTexture(explosion, texCoords);
			}
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Blend);
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

		private static void DrawCircleTexture(IReadOnlyCircle circle, IReadOnlyRectangle texCoords)
        {

			GL.Enable(EnableCap.Texture2D);
			GL.Begin(PrimitiveType.Quads);
			
			GL.TexCoord2(texCoords.MinX, texCoords.MinY);
			GL.Vertex2(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius);
			GL.TexCoord2(texCoords.MaxX, texCoords.MinY);
			GL.Vertex2(circle.Center.X + circle.Radius, circle.Center.Y - circle.Radius);
			GL.TexCoord2(texCoords.MaxX, texCoords.MaxY);
			GL.Vertex2(circle.Center.X + circle.Radius, circle.Center.Y + circle.Radius);
			GL.TexCoord2(texCoords.MinX, texCoords.MaxY);
			GL.Vertex2(circle.Center.X - circle.Radius, circle.Center.Y + circle.Radius);
			GL.End();
			GL.Disable(EnableCap.Texture2D);
		}

		private static void DrawRectangleTexture(IReadOnlyRectangle rectangle, IReadOnlyRectangle texCoords)
		{
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Blend);
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(texCoords.MinX, texCoords.MinY);
			GL.Vertex2(rectangle.MinX, rectangle.MinY);
			GL.TexCoord2(texCoords.MaxX, texCoords.MinY);
			GL.Vertex2(rectangle.MaxX, rectangle.MinY);
			GL.TexCoord2(texCoords.MaxX, texCoords.MaxY);
			GL.Vertex2(rectangle.MaxX, rectangle.MaxY);
			GL.TexCoord2(texCoords.MinX, texCoords.MaxY);
			GL.Vertex2(rectangle.MinX, rectangle.MaxY);
			GL.End();
			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.Texture2D);
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