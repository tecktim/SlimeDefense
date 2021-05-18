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
        public float Radius { get; }

        public bool Intersects(IReadOnlyCircle obj)
        {
            bool result = false;
            //circlecollider
            return result;
        }
    }
}
