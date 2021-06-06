namespace TowerDefenseNew.Grid
{
    public enum CellType { Empty, Path, Sniper, Rifle, Finish };

    public interface IReadOnlyGrid
    {
        CellType this[int column, int row] { get; }

        int Rows { get; }
        int Columns { get; }
    }
}