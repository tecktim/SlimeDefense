using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.GameObjects
{
    internal class Bullet : GameObject
    {

        internal Bullet(Vector2 center, float radius, List<Bullet> bullets, List<Enemy> enemies) : base(center, radius)
        {
            this.Center = center;
            this.Radius = radius;
            this.Bullets = bullets;
            this.Enemies = enemies;
        }

        internal void checkHit()
        {
            foreach (Enemy enemy in Enemies.ToList())
            {
                if (this.Intersects(enemy))
                {
                    Bullets.Remove(this);
                }
            }
         }
        

        internal Vector2 Velocity { get; set; }
        internal List<Bullet> Bullets;
        internal List<Enemy> Enemies;
    }
}
