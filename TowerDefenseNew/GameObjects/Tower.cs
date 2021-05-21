using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace TowerDefenseNew.GameObjects
{
    internal class Tower : GameObject
    {


        internal Tower(Vector2 center, float attackRadius, int damage, int attackSpeed, List<Enemy> enemies, List<Bullet> bullets) : base(center, attackRadius)
        {
            this.Center = center;
            this.Enemies = enemies;
            this.attackSpeed = attackSpeed;
            this.damage = damage;
            this.Radius = attackRadius;
            this.Bullets = bullets;
            this.Timer = new Timer(attackSpeed);
            asTimer(true);
        }

        public void asTimer(bool active)
        {
            // Creating timer with attackSpeed (millis) as interval
            if (active)
            {
                // Hook up elapsed event for the timer
                Timer.Elapsed += OnTimedEvent;
                Timer.AutoReset = true;
                Timer.Enabled = true;
            }
            else
            {
                Timer.Enabled = false;
                Timer.Stop();
                Timer.Close();
                Timer.Dispose();
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            checkRange();
        }

        private void checkRange()
        {
            try
            {
                foreach (Enemy enemy in Enemies.ToList())
                {
                    if (this.Intersects(enemy))
                    {
                        Bullet bullet = new Bullet(this.Center + new Vector2(0.5f, 0.5f), this.Radius / 35, this.damage, this.Bullets, this.Enemies);
                        bullet.bulletVelocity(enemy);
                        return;
                    }
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("checkRange exception");
                return;
            }
        }

        private int attackSpeed { get; set; }
        internal int damage { get; set; }
        private List<Enemy> Enemies { get; set; }
        private List<Bullet> Bullets { get; set; }
        public Timer Timer { get; }
    }
}
