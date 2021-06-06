using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefenseNew.GameObjects
{
    internal class Bullet : GameObject
    {

        internal Bullet(Vector2 center, float radius, int damage, List<Bullet> bullets, List<Enemy> enemies, uint towerType) : base(center, radius)
        {
            Center = center;
            Radius = radius;
            Bullets = bullets;
            Enemies = enemies;
            Damage = damage;
            Bullets.Add(this);
            TowerType = towerType;
            lifeTime = 0;
        }

        internal bool checkHit()
        {
            lifeTime++;
            bool isDead = false;
            try
            {
                foreach (Enemy enemy in Enemies.ToList())
                {
                    if (Intersects(enemy))
                    {
                        if (enemy.isShot(Damage))
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
            if (lifeTime >= 120)
            {
                Bullets.Remove(this);
            }
            return isDead;
        }

        internal void bulletVelocity(Enemy enemy)
        {
            //Distance tower to enemy
            float dx = enemy.Center.X - Center.X;
            float dy = enemy.Center.Y - Center.Y;

            //normalize sodass man bulletspeed miteinbeziehen kann
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length == 0) { length = 1; dx = 1; dy = 0; }
            bulletSpeed = 10;
            dx /= length;
            dy /= length;
            speedX = bulletSpeed * dx;
            speedY = bulletSpeed * dy;
        }


        internal float bulletSpeed;
        internal List<Bullet> Bullets;
        internal List<Enemy> Enemies;

        public int Damage { get; }
        public uint TowerType { get; private set; }

        private int lifeTime;
        internal float speedX;
        internal float speedY;
    }
}
