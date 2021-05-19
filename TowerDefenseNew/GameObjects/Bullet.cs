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
            this.Enemy = Enemies[0];

            //Distance tower to enemy
            float dx = this.Enemy.Center.X - this.Center.X;
            float dy = this.Enemy.Center.Y - this.Center.Y;
            //normalize sodass man bulletspeed miteinbeziehen kann
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if(length == 0) { length = 1; dx = 1; dy = 0; }
            this.bulletSpeed = 10;
            dx /= length;
            dy /= length;
            speedX = bulletSpeed * dx;
            speedY = bulletSpeed * dy;
        }

        internal void checkHit()
        {
            foreach (Enemy enemy in Enemies.ToList())
            {
                if (this.Intersects(enemy))
                {
                    if (enemy.isShot(Tower.damage))
                    {
                        //normal hit if true
                    }
                    else Enemies.Remove(enemy); //he dead if false
                    Bullets.Remove(this);
                }
            }
         }

        internal float bulletSpeed;
        internal List<Bullet> Bullets;
        internal List<Enemy> Enemies;
        internal Tower Tower;

        private Enemy Enemy;

        internal float speedX;
        internal float speedY;
    }
}
