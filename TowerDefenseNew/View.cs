using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseNew.GameObjects;
using TowerDefenseNew.Grid;
using TowerDefenseNew.Interfaces;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
    internal class View
    {
        private readonly Texture texExplosion;
        private readonly Texture texFont;
        private readonly Texture tileSet;
        //private readonly Texture texSniper;

        public View(GameWindow window)
        {
            //TODO: Change the clear color of the screen.
            GL.ClearColor(Color4.Black);
            Window = window;


            var content = $"{nameof(TowerDefenseNew)}.Content.";

            //texSniper = TextureLoader.LoadFromResource(content + "sniperTower.png");
            texExplosion = TextureLoader.LoadFromResource(content + "smokin.png");
            texFont = TextureLoader.LoadFromResource(content + "sonic_asalga.png");
            tileSet = TextureLoader.LoadFromResource(content + "TileSet_CG_offset_1_top_after_first_tile.png");

        }

        internal Camera Camera { get; } = new Camera();
        public GameWindow Window { get; }
        internal List<Vector2> circlePoints = CreateCirclePoints(360);
        internal bool sampleBouncer;

        internal void Draw(Model model)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit); // clear the screen
            if (model.gameOver)
            {

                GL.BindTexture(TextureTarget.Texture2D, texFont.Handle); // bind font texture
                DrawText("GAME OVER", 24f, 15f, 1f);
                DrawText("Press ESC to close the game", 22f, 14f, 0.5f);
                DrawText($"Total kills:{model.killCount}", 25f, 11f, 0.5f);
                DrawText($"Stage {model.stage} was conquered", 23f, 10f, 0.5f);

                Window.UpdateFrequency = 0;
                Window.RenderFrequency = 0;
            }
            else
            {
                Camera.Draw();
                DrawGrid(model.Grid, Color4.White);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);
                GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);
                DrawSamples(sampleSniper, sampleRifle, sampleBouncer, sampleColRow.X, sampleColRow.Y);
                GL.Disable(EnableCap.Blend);
                try
                {
                    foreach (Enemy enemy in model.enemies.ToList())
                    {
                        if (enemy != null)
                        {
                            GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);
                            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                            GL.Enable(EnableCap.Blend);
                            DrawText($"HP:{ enemy.health}", enemy.Center.X - 0.5f, enemy.Center.Y + .85f, .3f);

                            GL.Disable(EnableCap.Blend);
                            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                            GL.Enable(EnableCap.Blend);
                            if (enemy.health >= model.enemyHealth * 0.8)
                            {
                                DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 0f, 4 * 5 + 4);
                            }
                            else if (enemy.health >= model.enemyHealth * 0.6)
                            {
                                DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 0f, 4 * 5 + 3);
                            }
                            else if (enemy.health >= model.enemyHealth * 0.4)
                            {
                                DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 0f, 4 * 5 + 2);
                            }
                            else if (enemy.health >= model.enemyHealth * 0.2)
                            {
                                DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 0f, 4 * 5 + 1);
                            }
                            else if (enemy.health > 0)
                            {
                                DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 0f, 4 * 5);
                            }
                            GL.Disable(EnableCap.Blend);
                        }
                    }

                    foreach (Bullet bullet in model.bullets.ToList())
                    {
                        if (bullet != null)
                        {
                            DrawBullet(bullet, bullet.TowerType);
                        }else continue;
                    }

                    DrawExplosion(model.explosions);
                }
                catch (System.ArgumentException)
                {
                    Console.WriteLine("View.Draw exception");
                }
                catch (System.NullReferenceException)
                {
                    Console.WriteLine("View.Draw exception");
                }
                DrawHelpText(model);
            }
        }

        private void DrawSamples(bool sampleSniper, bool sampleRifle, bool sampleBouncer, float column, float row)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            if (sampleSniper)
            {
                DrawCircle(new Vector2(column + .5f, row + .5f), 5f, Color4.White);
                DrawTile(column, row, 0f, 0f, 1 * 5);
            }
            else if (sampleRifle)
            {
                DrawCircle(new Vector2(column + .5f, row + .5f), 2f, Color4.White);
                DrawTile(column, row, 0f, 0f, 2 * 5);
            }
            else if(sampleBouncer)
            {
                DrawCircle(new Vector2(column + .5f, row + .5f), 3f, Color4.White);
                DrawTile(column, row, 0f, 0f, 6 * 5);
            }
            GL.Disable(EnableCap.Blend);
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


                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);
                DrawRectangleTexture(rect, texCoords);
                GL.Disable(EnableCap.Blend);
                rect.MinX += rect.SizeX;
            }
        }

        public void DrawTile(float x, float y, float offset, float scale, uint tileNumber)
        {
            GL.Color4(Color4.White);
            GL.BindTexture(TextureTarget.Texture2D, tileSet.Handle); // bind font texture
            const uint tilesPerColumn = 10;
            const uint tilesPerRow = 5;
            var rect = new Rect(x+offset, y+offset, 1f-scale, 1f-scale);
            var tileCoords = SpriteSheetTools.CalcTexCoords(tileNumber, tilesPerRow, tilesPerColumn);
            DrawRectangleTexture(rect, tileCoords);
        }

        private void DrawGrid(IReadOnlyGrid grid, Color4 color)
        {

            DrawGridLines(grid.Columns, grid.Rows);
            for (int column = 0; column < grid.Columns; ++column)
            {
                for (int row = 0; row < grid.Rows; ++row)
                {

                    if (CellType.Sniper == grid[column, row])
                    {
                        DrawTile(column, row, 0f, 0f, 1 * 5); //Snake
                    }
                    if (CellType.Rifle == grid[column, row])
                    {
                        DrawTile(column, row, 0f, 0f, 2 * 5); //Ghost
                    }
                    if (CellType.Bouncer == grid[column, row])
                    {
                        DrawTile(column, row, 0f, 0f, 6 * 5); //Bouncer
                    }
                    if (CellType.Path == grid[column, row])
                    {
                        DrawTile(column, row, 0f, 0f, 1); //Path
                    }
                    if (CellType.Path == grid[column, row] && column == 0)
                    {
                        DrawTile(column, row, 0f, 0f, 2); //Portal, nur 1x
                    }
                    if (CellType.Empty == grid[column, row] || CellType.Finish == grid[column, row])
                    {
                        DrawTile(column, row, 0f, 0f, 0); //Weed :)
                    }
                }
            }
        }

        private void DrawBullet(IReadOnlyCircle bullet, uint type)
        {
            GL.BindTexture(TextureTarget.Texture2D, tileSet.Handle);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            DrawTile(bullet.Center.X, bullet.Center.Y, 0.25f, 0.75f, 25 + (type*5));
            GL.Disable(EnableCap.Blend);
        }

        private void DrawExplosion(IEnumerable<Explosion> explosions)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.Color4(Color4.White);
            // how many sprites are in each column and row
            const uint spritesPerColumn = 8;
            const uint spritesPerRow = 8;
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

        public void DrawCircle(Vector2 center, float radius, Color4 color)
        {
            GL.Begin(PrimitiveType.LineLoop);
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

        private void DrawHelpText(Model model)
        {
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            //text to help the player
            GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);

            DrawText($"Cash:", 54.5f, 29f, 0.7f);
            DrawText($"{model.cash}$", 54.5f, 28f, 0.5f);

            DrawText($"Kills:", 54.5f, 27f, 0.6f);
            DrawText($"{model.killCount}", 54.5f, 26f, 0.5f);

            DrawText($"Stage:", 54.5f, 25f, 0.6f);
            DrawText($"{model.stage}", 54.5f, 24f, 0.5f);

            DrawText("Costs:", 54.25f, 21f, 0.4f);
            DrawText("Rifle 5$", 54.25f, 20.4f, 0.4f);
            DrawText("Sniper 20$", 54.25f, 19.8f, 0.4f);

            DrawText("____________", 54.2f, 14.8f, 0.35f);
            DrawText("How to play:", 54.25f, 15f, 0.35f);

            DrawText("1+Click to", 54.25f, 14f, 0.4f);
            DrawText("buy Sniper", 54.25f, 13.5f, 0.4f);

            DrawText("2+Click to", 54.25f, 12.5f, 0.4f);
            DrawText("buy Rifle", 54.25f, 12f, 0.4f);

            DrawText("3+Drag to", 54.25f, 11f, 0.4f);
            DrawText("place Path", 54.25f, 10.5f, 0.4f);

            DrawText("4+Click to", 54.25f, 9.5f, 0.4f);
            DrawText("sell Tower", 54.25f, 9f, 0.4f);

            DrawText("To start the", 54.15f, 8f, 0.3f);
            DrawText("game,first you", 54.15f, 7.6f, 0.3f);
            DrawText("place a path", 54.15f, 7.2f, 0.3f);
            DrawText("from left to", 54.15f, 6.8f, 0.3f);
            DrawText("right.When the", 54.15f, 6.4f, 0.3f);
            DrawText("path reaches", 54.15f, 6f, 0.3f);
            DrawText("the right side,", 54.15f, 5.6f, 0.3f);
            DrawText("enemies will", 54.15f, 5.2f, 0.3f);
            DrawText("spawn.Dont let", 54.15f, 4.8f, 0.3f);
            DrawText("them reach the", 54.15f, 4.4f, 0.3f);
            DrawText("end!", 54.15f, 4f, 0.3f);
            GL.Disable(EnableCap.Blend);
        }
        private static void DrawCircleTexture(IReadOnlyCircle circle, IReadOnlyRectangle texCoords)
        {

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.Begin(PrimitiveType.Quads);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.TexCoord2(texCoords.MinX, texCoords.MinY);
            GL.Vertex2(circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius);
            GL.TexCoord2(texCoords.MaxX, texCoords.MinY);
            GL.Vertex2(circle.Center.X + circle.Radius, circle.Center.Y - circle.Radius);
            GL.TexCoord2(texCoords.MaxX, texCoords.MaxY);
            GL.Vertex2(circle.Center.X + circle.Radius, circle.Center.Y + circle.Radius);
            GL.TexCoord2(texCoords.MinX, texCoords.MaxY);
            GL.Vertex2(circle.Center.X - circle.Radius, circle.Center.Y + circle.Radius);
            GL.End();
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);

        }

        private static void DrawRectangleTexture(IReadOnlyRectangle rectangle, IReadOnlyRectangle texCoords)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
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
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
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

        internal bool sampleSniper { get; set; } = false;
        internal bool sampleRifle { get; set; } = false;
        internal Vector2 sampleColRow { get; set; }

    }
}