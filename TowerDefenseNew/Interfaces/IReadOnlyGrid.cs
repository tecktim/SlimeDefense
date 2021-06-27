namespace TowerDefenseNew.Grid
{
    public enum CellType { Empty, PathRight, PathUp, PathLeft, PathDown, PathCross, Path, Sniper, Rifle, Bouncer, Finish };

    public interface IReadOnlyGrid
    {
        CellType this[int column, int row] { get; }


        int Rows { get; }
        int Columns { get; }
    }
}