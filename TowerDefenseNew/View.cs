﻿using OpenTK.Graphics.OpenGL;
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
        private readonly Texture texParticle;

        public View(GameWindow window)
        {
            GL.ClearColor(Color4.Black);
            Window = window;
            var content = $"{nameof(TowerDefenseNew)}.Content.";
            texParticle = TextureLoader.LoadFromResource(content + "smoke_256a.png");
            texExplosion = TextureLoader.LoadFromResource(content + "smokin.png");
            texFont = TextureLoader.LoadFromResource(content + "sonic_asalga.png");
            tileSet = TextureLoader.LoadFromResource(content + "TileSet_CG_5x12_kippe.png");
        }

        internal Camera GameCamera { get; } = new Camera(17f, new Vector2(29f, 15f));
        internal Camera GuiCamera { get; } = new Camera(17f, new Vector2(29f, 15f));
        public GameWindow Window { get; }
        internal List<Vector2> circlePoints = CreateCirclePoints(360);
        internal bool sampleBouncer;
        internal bool samplePath;
        internal bool samplePortal = true;

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
                GameCamera.Draw();
                DrawGrid(model);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);
                GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);
                UpdateSamples(model);
                DrawSamples(sampleSniper, sampleRifle, sampleBouncer, samplePath, samplePortal, model.waypoints, sampleColRow.X, sampleColRow.Y);
                GL.Disable(EnableCap.Blend);
                try
                {
                    DrawEnemy(model);
                    DrawBullet(model);
                    DrawExplosion(model.explosions);
                    DrawParticles(model.particles);
                }
                catch (System.ArgumentException)
                {
                    Console.WriteLine("View.Draw exception");
                }
                catch (System.NullReferenceException)
                {
                    Console.WriteLine("View.Draw exception");
                }
                GuiCamera.Draw();
                DrawGui(model);
            }
        }

        private void DrawGui(Model model)
        {
            DrawQuadRectangle(new Vector2(-50f, 30f), new Vector2(150f, 100f), Color4.Black);//balken oben
            DrawQuadRectangle(new Vector2(54f, -2f), new Vector2(100f, 34f), Color4.Black);//balken rechts
            DrawQuadRectangle(new Vector2(-50f, -100f), new Vector2(150f, 100f), Color4.Black);//balken unten
            DrawQuadRectangle(new Vector2(-100f, -2f), new Vector2(100f, 34f), Color4.Black);//balkan links
            DrawHelpText(model);
        }

        private void UpdateSamples(Model model)
        {
            if (model.placed == true)
            {
                samplePath = false;
            }
            if (model.waypoints.Count != 0)
            {
                samplePortal = false;
            }
        }

        private void DrawBullet(Model model)
        {
            foreach (Bullet bullet in model.bullets.ToList())
            {
                if (bullet != null)
                {
                    DrawBullet(bullet, bullet.TowerType); 
                }
                else continue;
            }
        }

        private void DrawParticles(IEnumerable<IParticle> particles)
        {
            GL.BindTexture(TextureTarget.Texture2D, texParticle.Handle);
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            try
            {
                foreach (var particle in particles.ToList())
                {
                    GL.Color4(1f, 1f, 1f, 1f - particle.Age);
                    DrawParticle(particle.Location, 0.05f);
                }
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("particle draw exception");
            }
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawParticle(Vector2 location, float radius)
        {
            var transform = Transformation2d.Combine(Transformation2d.Scale(radius), Transformation2d.Translate(location.X, location.Y), GameCamera.CameraMatrix);
            GL.LoadMatrix(ref transform);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0f, 0f); GL.Vertex2(-1f, -1f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(1f, -1f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0f, 1f); GL.Vertex2(-1f, 1f);
            GL.End();
        }

        private void DrawEnemy(Model model)
        {
            foreach (Enemy enemy in model.enemies.ToList())
            {
                if (enemy != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);
                    GL.Enable(EnableCap.Blend);
                    if (enemy.dir == direction.right || enemy.dir == direction.up )
                    {
                        if (enemy.health >= model.enemyHealth * 0.8)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 7 * 5 + 4);
                        }
                        else if (enemy.health >= model.enemyHealth * 0.6)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 7 * 5 + 3);
                        }
                        else if (enemy.health >= model.enemyHealth * 0.4)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 7 * 5 + 2);
                        }
                        else if (enemy.health >= model.enemyHealth * 0.2)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 7 * 5 + 1);
                        }
                        else if (enemy.health > 0)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 7 * 5);
                        }
                    }
                    if (enemy.dir == direction.left || enemy.dir == direction.down)
                    {
                        if (enemy.health >= model.enemyHealth * 0.8)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 6 * 5 + 4);
                        }
                        else if (enemy.health >= model.enemyHealth * 0.6)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 6 * 5 + 3);
                        }
                        else if (enemy.health >= model.enemyHealth * 0.4)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 6 * 5 + 2);
                        }
                        else if (enemy.health >= model.enemyHealth * 0.2)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 6 * 5 + 1);
                        }
                        else if (enemy.health > 0)
                        {
                            DrawTile(enemy.Center.X, enemy.Center.Y, 0f, 1f, 6 * 5);
                        }
                    }
                    GL.Disable(EnableCap.Blend);
                }
            }
        }

        private void DrawSamples(bool sampleSniper, bool sampleRifle, bool sampleBouncer, bool samplePath, bool samplePortal, List<Vector2> waypoints, float column, float row)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            if (sampleSniper)
            {
                DrawCircle(new Vector2(column + .5f, row + .5f), 5f, Color4.White);
                DrawTile(column, row, 0f, 1f, 3 * 5);
            }
            else if (sampleRifle)
            {
                DrawCircle(new Vector2(column + .5f, row + .5f), 2f, Color4.White);
                DrawTile(column, row, 0f, 1f, 4 * 5);
            }
            else if (sampleBouncer)
            {
                DrawCircle(new Vector2(column + .5f, row + .5f), 3f, Color4.White);
                DrawTile(column, row, 0f, 1f, 5 * 5);
            }
            else if (samplePortal)
            {
                if (column == 0)
                {
                    DrawTile(column, row, 0f, 1f, 6);
                }
            }
            else if (samplePath)
            {
                if (column > 0 && column < 53)
                {
                    DrawTile(column, row, 0f, 1f, 1);
                    if(waypoints.Count > 0)
                    {
                        DrawTile(waypoints.Last().X, waypoints.Last().Y, 0f, 1f, 1);
                    }
                }
            }
            else return;
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
            const uint tilesPerColumn = 12;
            const uint tilesPerRow = 5;
            var rect = new Rect(x + offset, y + offset, 1f * scale, 1f * scale);
            var tileCoords = SpriteSheetTools.CalcTexCoords(tileNumber, tilesPerRow, tilesPerColumn);
            DrawRectangleTexture(rect, tileCoords);
        }

        private void DrawGrid(Model model)
        {
            for (int column = 0; column < model.Grid.Columns; ++column)
            {
                for (int row = 0; row < model.Grid.Rows; ++row)
                {
                    foreach (Tower tower in model.towers)
                    {
                        if (tower.aimAtEnemy != null)
                        {
                            if (tower.aimAtEnemy.Center.X < column)
                            {
                                if (CellType.Sniper == model.Grid[column, row])
                                {
                                    DrawTile(column, row, 0f, 1f, 3 * 5); //Sniper
                                }
                                if (CellType.Rifle == model.Grid[column, row])
                                {
                                    DrawTile(column, row, 0f, 1f, 4 * 5); //Rifler
                                }
                                if (CellType.Bouncer == model.Grid[column, row])
                                {
                                    DrawTile(column, row, 0f, 1f, 5 * 5); //Bouncer
                                }
                            }
                            if (tower.aimAtEnemy.Center.X > column)
                            {
                                if (CellType.Sniper == model.Grid[column, row])
                                {
                                    DrawTile(column, row, 0f, 1f, 3 * 5 + 1); //Sniper
                                }
                                if (CellType.Rifle == model.Grid[column, row])
                                {
                                    DrawTile(column, row, 0f, 1f, 4 * 5 + 1); //Rifler
                                }
                                if (CellType.Bouncer == model.Grid[column, row])
                                {
                                    DrawTile(column, row, 0f, 1f, 5 * 5 + 1); //Bouncer
                                }
                            }
                        }
                        else
                        {
                            if (CellType.Sniper == model.Grid[column, row])
                            {
                                DrawTile(column, row, 0f, 1f, 3 * 5); //Sniper
                            }
                            if (CellType.Rifle == model.Grid[column, row])
                            {
                                DrawTile(column, row, 0f, 1f, 4 * 5); //Rifler
                            }
                            if (CellType.Bouncer == model.Grid[column, row])
                            {
                                DrawTile(column, row, 0f, 1f, 5 * 5); //Bouncer
                            }
                        }
                    }
                    if (CellType.Path == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 1 * 5); //Path
                    }
                    if (CellType.PathRight == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 2 * 5); //Path
                    }
                    if (CellType.PathUp == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 2 * 5 + 1); //Path
                    }
                    if (CellType.PathLeft == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 2 * 5 + 2); //Path
                    }
                    if (CellType.PathDown == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 2 * 5 + 3); //Path
                    }
                    if (CellType.PathCross == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 2 * 5 + 4); //Path
                    }
                    if (CellType.Path == model.Grid[column, row] && column == 0)
                    {
                        DrawTile(column, row, 0f, 1f, 1 * 5 + 1); //Portal blue
                    }
                    if (CellType.Empty == model.Grid[column, row] || CellType.Finish == model.Grid[column, row])
                    {
                        DrawTile(column, row, 0f, 1f, 0); //Weed :)
                    }
                    if (CellType.Empty == model.Grid[column, row] && column == 0 && !model.placed)
                    {
                        DrawTile(column, row, 0f, 1f, 1); //WeedIndicator :)
                    }
                    if (CellType.Finish == model.Grid[column, row] && column == 53 && !model.placed)
                    {
                        DrawTile(column, row, 0f, 1f, 1); //WeedIndicator :)
                    }
                    if (CellType.Path == model.Grid[column, row] && column == 53 && model.placed)
                    {
                        DrawTile(column, row, 0f, 1f, 1 * 5 + 2); //Portal red
                    }
                }
            }
        }

        private void DrawBullet(IReadOnlyCircle bullet, uint type)
        {
            GL.BindTexture(TextureTarget.Texture2D, tileSet.Handle);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            if (type == 0)
            {
                DrawTile(bullet.Center.X, bullet.Center.Y, .5f, .66f, ((type + 3) * 5) + 2);
            }
            if (type == 2)
            {
                DrawTile(bullet.Center.X, bullet.Center.Y, 0.25f, .5f, ((type + 3) * 5) + 2);
            }
            if (type == 1)
            { DrawTile(bullet.Center.X, bullet.Center.Y, 0.25f, .33f, ((type + 3) * 5) + 2); }
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
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(color);
            GL.Vertex2(min);
            GL.Vertex2(max.X, min.Y);
            GL.Vertex2(max);
            GL.Vertex2(min.X, max.Y);
            GL.End();
        }

        private void DrawQuadRectangle(Vector2 min, Vector2 size, Color4 color)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            var max = min + size;
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(color);
            GL.Vertex2(min);
            GL.Vertex2(max.X, min.Y);
            GL.Vertex2(max);
            GL.Vertex2(min.X, max.Y);
            GL.End();
            GL.Disable(EnableCap.Blend);
        }

        private void DrawHelpText(Model model)
        {
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            //text to help the player
            GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);
            switch (model.stage)
            {
                case 0:
                    // DrawText("To start the game,hold SPACEBAR", 11f, 30f, 1f);
                    DrawText("> > > > > To start the game,drag the mouse from left to right < < < < <", 8f, -1f, .5f);
                    break;
                case 1:
                    DrawText("ENEMIES ARE INCOMING,Place towers to kill them before they reach the end", 7f, -1f, .5f);
                    break;
                case 2:
                    DrawText("Pro tip 1: Every new stage you reach,enemies will increase in HP by 15%", 7.5f, -1f, .5f);
                    break;
                case 3:
                    DrawText("Pro tip: Every tenth stage you reach,the per kill bounty increases by 1$", 7.5f, -1f, .5f);
                    break;
                default:
                    break;
            }


            DrawText($":{model.cash}", 15f, 30.5f, 1f);
            DrawText($":{model.killCount}", 25f, 30.5f, 1f);
            DrawText($":{model.stage}", 35f, 30.5f, 1f);
            GL.BindTexture(TextureTarget.Texture2D, tileSet.Handle);
            //cash skull stage
            DrawTile(14f, 30.5f, 0f, 1f, 8 * 5);
            DrawTile(24f, 30.5f, 0f, 1f, 8 * 5 + 2);
            DrawTile(34f, 30.5f, 0f, 1f, 8 * 5 + 1);

            GL.BindTexture(TextureTarget.Texture2D, texFont.Handle);
            if(model.stage == 0)
            {
                DrawText("Undo last:", 54.15f, 1.25f, .5f);
                DrawText("Press R", 54.15f, .25f, .5f);
            }
            if (model.stage > 0)
            {
                DrawText("Towers:", 54.2f, 29.2f, 0.7f);
                DrawText($"{model.towerCount}/50", 54.2f, 28f, 1f);

               // DrawText("Press Button", 54.15f, 29.5f, 0.4f);
                //DrawText("and Click to", 54.15f, 29f, 0.4f);
               // DrawText("buy towers", 54.15f, 28.5f, 0.4f);
                //Rifle station
                DrawText("Rifle   5$", 54.15f, 27f, .5f);
                DrawText("|", 56.15f, 26f, .5f);
                DrawText("||", 58.15f, 26f, .4f);
                DrawText("+", 57.4f, 25f, .5f);
                DrawText("Buy:", 54.15f, 25f, .5f);
                //Sniper station
                DrawText("Sniper 20$", 54.15f, 23.85f, .5f);
                DrawText("||", 56.15f, 23f, .5f);
                DrawText("|", 58.15f, 23f, .4f);
                DrawText("+", 57.4f, 22f, .5f);
                DrawText("Buy:", 54.15f, 22f, .5f);
                //bouncer station
                DrawText("Bounce 40$", 54.15f, 20.75f, .5f);
                DrawText("|", 56.15f, 19.75f, .5f);
                DrawText("|", 58.15f, 19.75f, .4f);
                DrawText("+", 57.4f, 18.75f, .5f);
                DrawText("Buy:", 54.15f, 18.75f, .5f);
                //sell station
                DrawText("+", 57.6f, .25f, .5f);
                DrawText("Sell:", 54.15f, .25f, .5f);

                GL.BindTexture(TextureTarget.Texture2D, tileSet.Handle);
                //rifle station
                DrawRectangle(new Vector2(54.1f, 24.75f), new Vector2(5f, 3f), Color4.White);
                DrawRectangle(new Vector2(54.15f, 25.75f), new Vector2(1f, 1f), Color4.White);
                DrawTile(54.15f, 25.75f, 0f, 1f, 4 * 5 + 4);
                DrawTile(55.15f, 25.75f, 0f, 1f, 3);
                DrawTile(57.15f, 25.75f, 0f, 1f, 8);
                DrawTile(56.15f, 24.75f, 0f, 1f, 9 * 5);
                DrawTile(58.15f, 24.75f, 0f, 1f, 9 * 5 + 3);
                if(model.cash < 5)
                {
                    DrawQuadRectangle(new Vector2(54.0f, 24.75f), new Vector2(5.2f, 3f), new Color4 (80, 80, 80, 160));
                }
                //sniper station
                DrawRectangle(new Vector2(54.1f, 21.75f), new Vector2(5f, 2.75f), Color4.White);
                DrawRectangle(new Vector2(54.15f, 22.75f), new Vector2(1f, 1f), Color4.White);
                DrawTile(54.15f, 22.75f, 0f, 1f, 3 * 5 + 4);
                DrawTile(55.15f, 22.75f, 0f, 1f, 3);
                DrawTile(57.15f, 22.75f, 0f, 1f, 8);
                DrawTile(56.15f, 21.75f, 0f, 1f, 9 * 5 + 1);
                DrawTile(58.15f, 21.75f, 0f, 1f, 9 * 5 + 3);
                if (model.cash < 20)
                {
                    DrawQuadRectangle(new Vector2(54.0f, 21.75f), new Vector2(5.2f, 3f), new Color4(80, 80, 80, 160));
                }
                //bouncer station
                DrawRectangle(new Vector2(54.1f, 18.5f), new Vector2(5f, 3f), Color4.White);
                DrawRectangle(new Vector2(54.15f, 19.5f), new Vector2(1f, 1f), Color4.White);
                DrawTile(54.15f, 19.5f, 0f, 1f, 5 * 5 + 4);
                DrawTile(55.15f, 19.5f, 0f, 1f, 3);
                DrawTile(57.15f, 19.5f, 0f, 1f, 8);
                DrawTile(56.15f, 18.5f, 0f, 1f, 9 * 5 + 2);
                DrawTile(58.15f, 18.5f, 0f, 1f, 9 * 5 + 3);
                if (model.cash < 40)
                {
                    DrawQuadRectangle(new Vector2(54.0f, 18.5f), new Vector2(5.2f, 3f), new Color4(80, 80, 80, 160));
                }
                //Sell station
                DrawRectangle(new Vector2(54.1f, 0f), new Vector2(5f, 1f), Color4.White);
                DrawTile(56.5f, 0f, 0f, 1f, 9 * 5 + 4);
                DrawTile(58.2f, 0f, 0f, 1f, 9 * 5 + 3);
            }

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
            GameCamera.Resize(width, height);
            GuiCamera.Resize(width, height);

        }

        internal bool sampleSniper { get; set; } = false;
        internal bool sampleRifle { get; set; } = false;
        internal Vector2 sampleColRow { get; set; }

    }
}