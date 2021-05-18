using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseNew.Grid;

namespace TowerDefenseNew
{
	internal class Model
	{
		public Model(IGrid grid)
		{
			_grid = grid;
		}

		internal IReadOnlyGrid Grid => _grid;

		internal void Update(float deltaTime)
		{
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

		internal void PlacePath(int column, int row)
		{
			_grid[column, row] = CellType.Path;
		}


        private readonly IGrid _grid;
    }
}
