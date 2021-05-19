using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.GameObjects
{
    internal class GameObject : IReadOnlyCircle
    {
        public GameObject(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public bool Intersects(IReadOnlyCircle obj)
        {
            //circlecollider
            float radius = this.Radius + obj.Radius;
            float deltaX = Center.X - obj.Center.X;
            float deltaY = Center.Y - obj.Center.Y;
            float distance = (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            if (distance < radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
