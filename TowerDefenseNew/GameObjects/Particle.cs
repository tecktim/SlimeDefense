using OpenTK.Mathematics;
using TowerDefenseNew.Interfaces;

namespace TowerDefenseNew.GameObjects
{
    internal class Particle : IParticle
    {
        public float Age { get; set; }

        public Vector2 Location { get; set; }
        internal bool IsAlive => Age < 1;
        internal Vector2 Acceleration { get; set; }
        internal Vector2 Velocity { get; set; }

        public void ApplyForce(Vector2 force)
        {
            Acceleration += force;
        }

        public void Seed(Vector2 location, Vector2 velocity)
        {
            Age = 0f;
            Location = location;
            Velocity = velocity;
            Acceleration = Vector2.Zero;
        }
    }
}
