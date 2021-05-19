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

        internal bool isShot(Tower tower)
        {
            health = health - tower.damage;
            if(health < tower.damage)
            {
                this.alive = false;
            }
            Console.WriteLine("enemy hit for " + tower.damage + " , enemy life: " + this.health);
            return true;
        }


        public Vector2 Velocity = new Vector2(1f, 0f);

        
    }
}
