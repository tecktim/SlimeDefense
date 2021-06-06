using OpenTK.Mathematics;

namespace TowerDefenseNew.GameObjects
{
    internal class Circle : IReadOnlyCircle
    {
        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector2 Center { get; private set; }
        public float Radius { get; private set; }
    }
}
