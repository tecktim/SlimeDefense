namespace TowerDefenseNew.Grid
{
    internal static class GridLoader
    {
        internal static IGrid CreateGrid()
        {
            var grid = new Grid();
            for (int x = 0; x < grid.Columns - 1; ++x)
            {
                for (int y = 0; y < grid.Rows; ++y)
                {
                    grid[x, y] = CellType.Empty;
                }
            }
            for (int x = grid.Columns - 1; x < grid.Columns; ++x)
            {
                for (int y = 0; y < grid.Rows; ++y)
                {
                    grid[x, y] = CellType.Finish;
                }
            }
            return grid;
        }
    }
}
