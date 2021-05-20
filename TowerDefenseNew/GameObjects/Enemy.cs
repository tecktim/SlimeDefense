using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.GameObjects
{
    internal enum direction { up, down, left, right};
    internal class Enemy : GameObject
    {
        internal int health;
        internal bool alive;
        internal Enemy(Vector2 center, float radius, int health) : base(center, radius)
        {
            this.health = health;
            this.alive = true;
            this.Velocity = changeDirection(direction.right);
        }

        internal bool isShot(int damage)
        {
            this.health = this.health - damage;
            if (health <= 0)
            {
                this.alive = false;
            }
            return alive;
        }

        internal void changeSpeed(float factor)
        {
            this.Velocity = new Vector2(Velocity.X * factor, Velocity.Y * factor);
        }

        internal Vector2 changeDirection(direction dir)
        {
            if (dir == direction.up)
            {
                return this.Velocity = new Vector2(0f, 5f);
            }
            else if (dir == direction.down)
            {
                return this.Velocity = new Vector2(0f, -5f);
            }
            else if (dir == direction.left)
            {
                return this.Velocity = new Vector2(-5f, 0f);
            }
            else if (dir == direction.right)
            {
                return this.Velocity = new Vector2(5f, 0f);
            }
            else return this.Velocity;
        }

        internal Vector2 Velocity;
    }
}
