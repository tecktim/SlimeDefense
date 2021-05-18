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
        private int attackSpeed;
        private List<Enemy> Enemies { get; set; }


        internal Tower(Vector2 center, float attackRadius, int damage, int attackSpeed, List<Enemy> enemies) : base(center, attackRadius)
        {
            this.Enemies = enemies;
            asTimer();
        }

        private void asTimer()
        {
            // Creating timer with attackSpeed (millis) as interval
            System.Timers.Timer asTimer = new System.Timers.Timer(attackSpeed);
            // Hook up elapsed event for the timer
            asTimer.Elapsed += OnTimedEvent;
            asTimer.AutoReset = true;
            asTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            checkRange();
        }

        private void checkRange()
        {
             
            foreach(Enemy enemy in Enemies)
            {
                if (this.Intersects(enemy))
                {
                    //shoot;
                }
            }
        }
    }
}
