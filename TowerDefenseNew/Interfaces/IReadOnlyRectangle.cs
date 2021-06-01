namespace TowerDefenseNew.Interfaces
{
	public interface IReadOnlyRectangle
	{
		float MaxX { get; }
		float MaxY { get; }
		float MinX { get; }
		float MinY { get; }
		float SizeX { get; }
		float SizeY { get; }
	}
}