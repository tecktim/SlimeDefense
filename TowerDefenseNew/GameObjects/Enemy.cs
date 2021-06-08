using OpenTK.Mathematics;

namespace TowerDefenseNew.GameObjects
{
    public enum direction { up, down, left, right, none };


    internal class Enemy : GameObject
    {
        internal int health;
        internal Enemy(Vector2 center, float radius, int health) : base(center, radius)
        {
            this.health = health;
        }

        internal bool isShot(int damage)
        {
            health = health - damage;
            if (health <= 0)
            {
                IsAlive = false;
            }
            return IsAlive;
        }

        internal void changeSpeed(float factor)
        {
            Velocity = new Vector2(Velocity.X * factor, Velocity.Y * factor);
        }

        internal Vector2 changeDirection(direction dir)
        {
            //Originally 1.1
            if (dir == direction.up)
            {
                this.dir = direction.up;
                return Velocity = new Vector2(0f, 3.9f);
            }
            else if (dir == direction.down)
            {
                this.dir = direction.down;
                return Velocity = new Vector2(0f, -3.9f);
            }
            else if (dir == direction.left)
            {
                this.dir = direction.left;
                return Velocity = new Vector2(-3.9f, 0f);
            }
            else if (dir == direction.right)
            {
                this.dir = direction.right;
                return Velocity = new Vector2(3.9f, 0f);
            }
            else
            {
                this.dir = direction.none;
                return Velocity;
            }
        }

        internal int wayPointIterator { get; set; } = 0;
        internal direction dir { get; set; }
        internal Vector2 Velocity { get; set; }
    }
}
