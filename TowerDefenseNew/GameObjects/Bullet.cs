using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

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
            bounceCount = 0;
        }

        internal bool CheckHit()
        {
            lifeTime++;
            bool isDead = false;
            try
            {
                for (int i = 0; i < Enemies.Count - 1; i++)
                {
                    if (Intersects(Enemies[i]) && TowerType != 2)
                    {
                        if (Enemies[i].IsShot(Damage))
                        {
                            //normal hit if true
                            isDead = false;
                        }
                        else
                        {
                            //he dead if false
                            isDead = true;
                            Enemies.Remove(Enemies[i]);

                        }
                        Bullets.Remove(this);
                        continue;
                    }
                    if (Intersects(Enemies[i]) && TowerType == 2)
                    {
                        bounceCount++;
                        BulletVelocity(Enemies[i + 1]);
                        if (Enemies[i].IsShot(Damage))
                        {
                            //normal hit if true
                            isDead = false;
                        }
                        else
                        {
                            //he dead if false
                            isDead = true;
                            Enemies.Remove(Enemies[i]);
                        }
                    }
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("checkHit exception");
            }
            if (Center.X <= -0.5f || Center.X >= 53f || Center.Y <= -0.5f || Center.Y >= 29f)
            {
                Bullets.Remove(this);
            }
            if (lifeTime >= 60 && TowerType != 2)
            {
                Bullets.Remove(this);
            }
            else if ((lifeTime >= 60 || bounceCount > 15) && TowerType == 2) { Bullets.Remove(this); }
            return isDead;
        }

        internal void BulletVelocity(Enemy enemy)
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


        private float bulletSpeed;
        private List<Bullet> Bullets;
        private List<Enemy> Enemies;
        private int Damage { get; }
        internal uint TowerType { get; set; }
        private int bounceCount;
        private int lifeTime;
        internal float speedX;
        internal float speedY;
    }
}
