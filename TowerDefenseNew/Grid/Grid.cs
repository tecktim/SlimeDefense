using System;

namespace TowerDefenseNew.Grid
{
    
    public class Grid : IGrid
    {
        private CellType[,] grid = new CellType[54, 30];

        public Grid()
        {
        }

        public CellType this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
        }

        public int Columns => grid.GetLength(0);
        public int Rows => grid.GetLength(1);
    }
}
