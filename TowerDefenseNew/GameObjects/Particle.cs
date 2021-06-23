using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenseNew.Interfaces;

namespace TowerDefenseNew.GameObjects
{
    internal class Particle : IParticle
    {
        public float Age { get; set; }

        public Vector2 Location { get; set; } = Vector2.Zero;
        public bool IsAlive => Age < 1;
        public float Mass { get; set; } = 1f;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public void ApplyForce(Vector2 force)
        {
            //force = mass * acceleration
            Acceleration += force / Mass;
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
