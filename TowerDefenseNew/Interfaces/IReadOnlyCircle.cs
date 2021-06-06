using OpenTK.Mathematics;

namespace TowerDefenseNew.GameObjects
{
    internal interface IReadOnlyCircle
    {
        Vector2 Center { get; }
        float Radius { get; }
    }
}
