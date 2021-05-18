using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseNew.GameObjects;
using TowerDefenseNew.Grid;

namespace TowerDefenseNew
{
	internal class Model
	{

		public Model(IGrid grid)
		{
			_grid = grid;
			pathway = new List<CellType>();
			enemies = new List<Enemy>();
			towers = new List<Tower>();
		}

		internal IReadOnlyGrid Grid => _grid;

		internal void Update(float deltaTime)
		{
			UpdateEnemies(deltaTime);
			UpdateTowers();
		}

		private void UpdateEnemies(float frameTime)
        {
			foreach(Enemy enemy in enemies)
            {
				enemy.Center += new Vector2(frameTime * enemy.Velocity.X, frameTime * enemy.Velocity.Y);
			}
		}

		private void UpdateTowers()
        {
			foreach(Tower tower in towers)
            {

            }
        }

		internal void ClearCell(int column, int row)
        {
			_grid[column, row] = CellType.Empty;
        }

		internal void PlaceSniper(int column, int row)
		{
			_grid[column, row] = CellType.Sniper;
		}

		internal void PlaceRifle(int column, int row)
		{
			_grid[column, row] = CellType.Rifle;
		}

		internal bool PlacePath(int column, int row)
		{
			bool placed = false;
			for(int i = 0; i < 54; i++)
            {
				_grid[i, row] = CellType.Path;
				pathway.Add(_grid[i, row]);
			}
			placed = true;
			Console.WriteLine(pathway.Count);
			enemies.Add(new Enemy(new Vector2(0 + 0.5f, row + 0.5f), .25f, 100));
			return placed;
		}

		internal CellType CheckCell(int column, int row)
        {
			return _grid[column, row];
        }

        private readonly IGrid _grid;

		private List<CellType> pathway;
        internal List<Enemy> enemies;
		internal List<Tower> towers;
    }
}
