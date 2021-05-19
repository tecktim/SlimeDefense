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
            asTimer();
        }

        private void asTimer()
        {
            // Creating timer with attackSpeed (millis) as interval
            System.Timers.Timer Timer = new System.Timers.Timer(attackSpeed);
            // Hook up elapsed event for the timer
            Timer.Elapsed += OnTimedEvent;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            checkRange();
        }

        private void checkRange()
        {
            if (Enemies.Count != 0)
            {
                foreach (Enemy enemy in Enemies.ToList())
                {
                    if (this.Intersects(enemy))
                    {
                        Bullet bullet = new Bullet(this.Center + new Vector2(0.5f, 0.5f), this.Radius/50, this, Bullets, Enemies);
                        Bullets.Add(bullet);
                        break;
                    }
                    else continue;
                }
            }
        }
        private int attackSpeed { get; set; }
        internal int damage { get; set; }
        private List<Enemy> Enemies { get; set; }
        private List<Bullet> Bullets { get; set; }
    }
}
