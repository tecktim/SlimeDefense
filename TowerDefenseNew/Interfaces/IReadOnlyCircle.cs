using OpenTK.Mathematics;

namespace TowerDefenseNew.GameObjects
{
    public interface IReadOnlyCircle
    {
        public Vector2 Center { get; }
        public float Radius { get; }
    }
}
