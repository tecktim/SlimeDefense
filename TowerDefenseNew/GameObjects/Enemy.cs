using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.GameObjects
{
    internal class Enemy : GameObject
    {
        internal Enemy(Vector2 center, float radius, int health) : base(center, radius)
        {

        }

        public Vector2 Velocity = new Vector2(1f, 0f);

        
    }
}
