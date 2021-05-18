using System;

namespace TowerDefenseNew.Grid
{
    internal static class GridLoader
	{
		internal static IGrid CreateGrid()
		{
			// if the file was not found or was of wrong type generate a new grid
			var rnd = new Random();
			var grid = new Grid();
			for (int x = 0; x < grid.Columns; ++x)
			{
				for (int y = 0; y < grid.Rows; ++y)
				{
					grid[x, y] = CellType.Empty;
				}
			}
			return grid;
		}
	}
}
