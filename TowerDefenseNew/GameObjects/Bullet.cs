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

        internal Bullet(Vector2 center, float radius, Tower tower, List<Bullet> bullets, List<Enemy> enemies) : base(center, radius)
        {
            this.Center = center;
            this.Radius = radius;
            this.Bullets = bullets;
            this.Enemies = enemies;
            this.Tower = tower;
            this.Bullets.Add(this);

        }

        internal bool checkHit()
        {
            bool isDead = false;
            try
            {
                foreach (Enemy enemy in Enemies.ToList())
                {
                    if (this.Intersects(enemy))
                    {
                        if (enemy.isShot(Tower.damage))
                        {
                            //normal hit if true
                            isDead = false;
                        }
                        else
                        {
                            //he dead if false
                            isDead = true;
                            Enemies.Remove(enemy);
                        }
                        Bullets.Remove(this);
                        continue;
                    }
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("checkHit exception");
            }
            return isDead;
        }
        
        internal void bulletVelocity(Enemy enemy)
        {
            //Distance tower to enemy
            float dx = enemy.Center.X - this.Center.X;
            float dy = enemy.Center.Y - this.Center.Y;

            //normalize sodass man bulletspeed miteinbeziehen kann
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length == 0) { length = 1; dx = 1; dy = 0; }
            this.bulletSpeed = 30;
            dx /= length;
            dy /= length;
            this.speedX = bulletSpeed * dx;
            this.speedY = bulletSpeed * dy;
        }
   

        internal float bulletSpeed;
        internal List<Bullet> Bullets;
        internal List<Enemy> Enemies;
        internal Tower Tower;
        internal float speedX;
        internal float speedY;
    }
}
