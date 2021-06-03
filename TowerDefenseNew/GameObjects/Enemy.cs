using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenseNew.Grid;

namespace TowerDefenseNew.GameObjects
{
    public enum direction { up, down, left, right, none };

   
    internal class Enemy : GameObject
    {
        internal int health;
        internal Enemy(Vector2 center, float radius, int health) : base(center, radius)
        {
            this.health = health;
            this.Velocity = changeDirection(direction.right);
        }

        internal bool isShot(int damage)
        {
            this.health = this.health - damage;
            if (health <= 0)
            {
                IsAlive = false;
            }
            return IsAlive;
        }

    internal void changeSpeed(float factor)
        {
            this.Velocity = new Vector2(Velocity.X * factor, Velocity.Y * factor);
        }

        internal Vector2 changeDirection(direction dir)
        {
            //Originally 1.1
            if (dir == direction.up)
            {
                this.dir = direction.up;
                return this.Velocity = new Vector2(0f, 5f);
            }
            else if (dir == direction.down)
            {
                this.dir = direction.down;
                return this.Velocity = new Vector2(0f, -5f);
            }
            else if (dir == direction.left)
            {
                this.dir = direction.left;
                return this.Velocity = new Vector2(-5f, 0f);
            }
            else if (dir == direction.right)
            {
                this.dir = direction.right;
                return this.Velocity = new Vector2(5f, 0f);
            }
            else
            {
                this.dir = direction.none;
                return this.Velocity;
            }
        }

        internal direction dir { get; set; }
        internal Vector2 Velocity { get; set; }
    }
}
