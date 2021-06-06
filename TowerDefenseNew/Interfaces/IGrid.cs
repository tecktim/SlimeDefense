namespace TowerDefenseNew.Grid
{
    public interface IGrid : IReadOnlyGrid
    {
        new CellType this[int column, int row] { set; get; }
    }
}
