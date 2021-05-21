using System;
using TowerDefenseNew.Grid;
using OpenTK.Mathematics;

namespace TowerDefenseNew.Grid
{
	/// <summary>
	/// Implementation for serialization only. Only use interface to access data in rest of program.
	/// </summary>
	[Serializable]
	public class Grid : IGrid
    {
		private CellType[,] grid = new CellType[54, 30];

		public Grid()
        {
        }

		public CellType this[int x, int y]
        {
			get { return grid[x, y]; }
			set { grid[x, y] = value; }
        }

		public int Columns { get { return grid.GetLength(0); } }
		public int Rows { get { return grid.GetLength(1); } }
	}
}
