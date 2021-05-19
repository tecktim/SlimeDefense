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
        internal int health;
        internal bool alive;
        internal Enemy(Vector2 center, float radius, int health) : base(center, radius)
        {
            this.health = health;
            this.alive = true;
        }

        internal bool isShot(int damage)
        {
            health = health - damage;
            if(health <= 0)
            {
                this.alive = false;
            }
            Console.WriteLine("enemy hit for " + damage + " , enemy life: " + this.health);
            return alive;
        }

        public Vector2 Velocity = new Vector2(1f, 0f);
    }
}
