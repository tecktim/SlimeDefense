using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TowerDefenseNew.GameObjects;
using TowerDefenseNew.Grid;

namespace TowerDefenseNew
{
    internal class Model
    {
        public Model(IGrid grid)
        {
            _grid = grid;
            waypoints = new List<Vector2>();
            enemies = new List<Enemy>();
            towers = new List<Tower>();
            bullets = new List<Bullet>();
            explosions = new List<Explosion>();
            particles = new List<Particle>();

            
            cash = 80;
            bouncerCost = 40;
            sniperCost = 20;
            rifleCost = 5;
            enemySpawnRate = 3000;
            enemyHealth = 1000;
            gameOver = false;
            timer = new System.Timers.Timer(enemySpawnRate);
            killCount = 0;
            placed = false;
            applyScaling = true;
            stage = 0;
            bounty = 1;
        }

        internal bool switchGameOver(bool lose)
        {
            if (lose)
            {
                gameOver = true;
            }
            else gameOver = false;
            return gameOver;
        }

        internal void giveCash()
        {
            cash += 10000;
        }

        internal IReadOnlyGrid Grid => _grid;

        internal void Update(float deltaTime)
        {
            Time += deltaTime;
            UpdateScaling();
            UpdateExplosions(deltaTime);
            UpdateBullets(deltaTime);
            UpdateEnemies(deltaTime);
            foreach(var e in enemies.ToList())
            {
                UpdateParticles(deltaTime, e);
            }
        }

        private void UpdateScaling()
        {
            if (killCount % 25 == 0 && applyScaling == true && killCount != 0)
            {
                enemyHealth += (enemyHealth / 100) * 15;
                applyScaling = false;
                stage++;
                if (stage % 10 == 0)
                {
                    bounty++;
                }
            }
            if (!(killCount % 25 == 0))
            {
                applyScaling = true;
            }
        }
        private Vector2 smoke = new(0f, 1f);
        private void UpdateParticles(float frameTime, Enemy enemy)
        {
            foreach (var particle in particles.ToList())
            {
                particle.ApplyForce(smoke);
                UpdateParticle(particle, frameTime);
                var lifeTime = 1.2f;  //particles life 1.5 seconds
                particle.Age += frameTime / lifeTime;

                if (!particle.IsAlive)
                {
                    Seed(particle, enemy);
                }
                if (!enemy.IsAlive)
                {

                    particles.Remove(particle);
                    continue;
                }
            }
        }
        private void UpdateParticle(Particle particle, float frameTime)
        {
            particle.Velocity += particle.Acceleration * frameTime;
            particle.Location += particle.Velocity * frameTime;
            //particle.Location += new Vector2(frameTime * particle.Velocity.X, frameTime * particle.Velocity.Y);
            //force was spend reset Acceleration
            particle.Acceleration = Vector2.Zero;
        }

        private readonly Random rnd = new Random(10);
        private float Rnd01() => (float)rnd.NextDouble();
		private float RndM11() => (Rnd01() - 0.5f) * 2.0f;
        private void Seed(Particle particle, Enemy enemy)
        {
            var velocity = new Vector2(RndM11() * .1f, Rnd01()) * 0.1f; //moving mainly upward
            if(enemy.dir == direction.right || enemy.dir == direction.up) particle.Seed(enemy.Center + new Vector2(1f - 2*0.0294117647058824f, 0.3f), velocity);
            if(enemy.dir == direction.left || enemy.dir == direction.down) particle.Seed(enemy.Center + new Vector2(0f + 2*0.0294117647058824f, 0.3f), velocity);
        }

        private void CreateParticles (int count, Enemy enemy)
        {
            for (int i = 0; i < count; i++)
            {
                var particle = new Particle();
                Seed(particle, enemy);
                particle.Age = Rnd01();
                particles.Add(particle);
            }
        }
        private void UpdateEnemies(float frameTime)
        {
            if (enemies.Count != 0)
            {
                {
                    foreach (Enemy enemy in enemies.ToList())
                    {
                        enemy.Center += new Vector2(frameTime * enemy.Velocity.X, frameTime * enemy.Velocity.Y);
                        if (enemy != null)
                        {
                            if (enemy.wayPointIterator < waypoints.Count - 1)
                            {
                                if (enemy.Center.X >= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y >= enemy.Center.Y && enemy.dir == direction.right && enemy.dir != direction.down)
                                {
                                    enemy.changeDirection(direction.up);
                                    enemy.wayPointIterator++;
                                    continue;
                                }
                                //new
                                if (enemy.Center.X < waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y >= enemy.Center.Y && enemy.dir == direction.left && enemy.dir != direction.down)
                                {
                                    //if (enemy == enemies[0]) Console.WriteLine("up");
                                    enemy.changeDirection(direction.up);
                                    enemy.wayPointIterator++;
                                    continue;
                                }

                                else if (enemy.Center.X >= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y <= enemy.Center.Y && enemy.dir == direction.right && enemy.dir != direction.up)
                                {
                                    enemy.changeDirection(direction.down);
                                    enemy.wayPointIterator++;
                                    continue;
                                }
                                //new
                                if (enemy.Center.X <= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y < enemy.Center.Y && enemy.dir == direction.left && enemy.dir != direction.up)
                                {
                                    //if (enemy == enemies[0]) Console.WriteLine("up");
                                    enemy.changeDirection(direction.down);
                                    enemy.wayPointIterator++;
                                    continue;
                                }
                                //new
                                if (enemy.Center.X >= waypoints[enemy.wayPointIterator + 1].X && waypoints[enemy.wayPointIterator].Y > enemy.Center.Y && enemy.dir == direction.down && enemy.dir != direction.left)
                                {
                                    //if (enemy == enemies[0]) Console.WriteLine("up");
                                    enemy.changeDirection(direction.left);
                                    enemy.wayPointIterator++;
                                    continue;
                                }
                                //new
                                if (enemy.Center.X >= waypoints[enemy.wayPointIterator + 1].X && waypoints[enemy.wayPointIterator].Y <= enemy.Center.Y && enemy.dir == direction.up && enemy.dir != direction.left)
                                {
                                    //if (enemy == enemies[0]) Console.WriteLine("up");
                                    enemy.changeDirection(direction.left);
                                    enemy.wayPointIterator++;
                                    continue;
                                }

                                else if (enemy.Center.X <= waypoints[enemy.wayPointIterator + 1].X && enemy.Center.Y < waypoints[enemy.wayPointIterator].Y && enemy.dir == direction.down && enemy.dir != direction.right)
                                {
                                    //if (enemy == enemies[0]) Console.WriteLine("right");
                                    enemy.changeDirection(direction.right);
                                    enemy.wayPointIterator++;
                                    continue;
                                }
                                else if (enemy.Center.X <= waypoints[enemy.wayPointIterator + 1].X && enemy.Center.Y >= waypoints[enemy.wayPointIterator].Y && enemy.dir == direction.up && enemy.dir != direction.right)
                                {
                                    enemy.changeDirection(direction.right);
                                    enemy.wayPointIterator++;
                                    continue;
                                }
                            }
                            if (enemy.Center.X >= waypoints[waypoints.Count - 1].X)
                            {
                                enemies.Remove(enemy);
                                switchGameOver(true);
                                return;
                            }
                        }
                    }
                }

            }
        }
        private void UpdateExplosions(float frameTime)
        {
            try
            {
                foreach (Explosion exp in explosions.ToList())
                {
                    exp.Update(frameTime);
                    if (!exp.IsAlive)
                    {
                        explosions.Remove(exp);
                    }
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("UpdateExplosions exception, ArgumentException");
            }
            catch (System.NullReferenceException)
            {
                Console.WriteLine("UpdateExplosions exception, NullReferenceException");
            }
        }
        private void UpdateBullets(float frameTime)
        {
            try
            {
                foreach (Bullet bullet in bullets.ToList())
                {
                    if (bullet != null)
                    {
                        bullet.Center += new Vector2(frameTime * bullet.speedX, frameTime * bullet.speedY);
                        if (bullet.checkHit())
                        {
                            //onEnemyKill
                            killCount++;

                            cash += bounty;
                            Explosion exp = new Explosion(bullet.Center + new Vector2(0.45f, 1.5f), 2f, 0.75f);
                            explosions.Add(exp);
                        }
                    }
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("UpdateBullet exception, ArgumentException");
            }
            catch (System.NullReferenceException)
            {
                Console.WriteLine("UpdateBullet exception, NullReferenceException");
            }
        }
        internal void ClearCell(int column, int row, Tower tower)
        {
            switch(CheckCell(column, row))
            {
                case CellType.Rifle:
                    cash += Math.Floor(rifleCost) * 0.8;
                    if (tower.Center.X == column && tower.Center.Y == row)
                    {
                        _grid[column, row] = CellType.Empty;
                        towers.Remove(tower);
                        tower.asTimer(false);
                        Math.Floor(cash);
                        return;
                    }
                    break;
                case CellType.Sniper:
                    cash += Math.Floor(sniperCost) * 0.8;
                    if (tower.Center.X == column && tower.Center.Y == row)
                    {
                        _grid[column, row] = CellType.Empty;
                        towers.Remove(tower);
                        tower.asTimer(false);
                        Math.Floor(cash);
                        return;
                    }
                    break;
                case CellType.Bouncer:
                    cash += Math.Floor(bouncerCost) * 0.8;
                    if (tower.Center.X == column && tower.Center.Y == row)
                    {
                        _grid[column, row] = CellType.Empty;
                        towers.Remove(tower);
                        tower.asTimer(false);
                        Math.Floor(cash);
                        return;
                    }
                    break;
            }
        }

        internal void PlaceSniper(int column, int row)
        {
            if (cash >= sniperCost)
            {
                Tower tower = new Tower(new Vector2(column, row), 5f, 450, 3000, enemies, bullets, 0);
                _grid[column, row] = CellType.Sniper;
                towers.Add(tower);
                cash -= sniperCost;
                Math.Floor(cash);
                towerCount++;
            }
            else return;
        }

        internal void PlaceRifle(int column, int row)
        {
            if (cash >= rifleCost)
            {
                Tower tower = new Tower(new Vector2(column, row), 2f, 25, 333, enemies, bullets, 1);
                _grid[column, row] = CellType.Rifle;
                towers.Add(tower);
                cash -= rifleCost;
                Math.Floor(cash);
                towerCount++;
            }
            else return;
        }
        internal void PlaceBouncer(int column, int row)
        {
            if (cash >= bouncerCost)
            {
                Tower tower = new Tower(new Vector2(column, row), 3f, 40, 2000, enemies, bullets, 2);
                _grid[column, row] = CellType.Bouncer;
                towers.Add(tower);
                cash -= bouncerCost;
                Math.Floor(cash);
                towerCount++;
            }
            else return;
        }

        internal bool CellTypeIsAnyPath(int column, int row)
        {
            if (CheckCell(column, row) == CellType.PathRight ||
                CheckCell(column, row) == CellType.PathUp ||
                CheckCell(column, row) == CellType.PathLeft ||
                CheckCell(column, row) == CellType.PathDown ||
                CheckCell(column, row) == CellType.PathCross ||
                CheckCell(column, row) == CellType.Path
                )
            {
                return true;
            }else
            {
                return false;
            }
        }


        internal bool PlacePath(int column, int row)
        {
            //First is always placed left
            if (column == 0 && waypoints.Count == 0 && checkCol == 0)
            {
                _grid[column, row] = CellType.Path;
                checkCol++;
                checkRow = row;
                spawnRow = row;
            }
            //right
            else if (column == checkCol && row == checkRow && column != 0)
            {
                if(waypoints.Count >= 2)
                {
                    _grid[(int)waypoints[waypoints.Count - 1].X, (int)waypoints[waypoints.Count - 1].Y] = CellType.PathRight;
                }
                if (CheckCell(checkCol, row) == CellType.Empty)
                {
                    _grid[column, row] = CellType.PathRight;
                    waypoints.Add(new Vector2(column, row));
                    checkCol++;
                }
            }

            //left
            else if (column == checkCol - 2 && row == checkRow && column != 0)
            {
                _grid[(int)waypoints[waypoints.Count - 1].X, (int)waypoints[waypoints.Count - 1].Y] = CellType.PathLeft;
                _grid[column, row] = CellType.PathLeft;
                waypoints.Add(new Vector2(column, row));
                checkCol--;
            }

            //down
            else if (column == checkCol - 1 && row == checkRow - 1 && CheckCell(checkCol, row) != CellType.Finish && column != 0)
            {
                _grid[(int)waypoints[waypoints.Count - 1].X, (int)waypoints[waypoints.Count - 1].Y] = CellType.PathDown;
                _grid[column, row] = CellType.PathDown;
                waypoints.Add(new Vector2(column, row));
                checkRow = row;
            }

            //up
            else if (column == checkCol - 1 && row == checkRow + 1 && CheckCell(checkCol, row) != CellType.Finish && column != 0)
            {
                _grid[(int)waypoints[waypoints.Count - 1].X, (int)waypoints[waypoints.Count - 1].Y] = CellType.PathUp;
                _grid[column, row] = CellType.PathUp;
                waypoints.Add(new Vector2(column, row));
                checkRow = row;
            }
            if (CheckCell(checkCol, row) == CellType.Finish && placed == false)
            {
                stage = 1;
                _grid[checkCol, row] = CellType.Path;
                waypoints.Add(new Vector2(column, row));
                waypoints.Add(new Vector2(column+1, row));
                waypoints.Add(new Vector2(checkCol, row));
                placed = true;
                enemySpawnTimer(spawnRow);
            }

            if (waypoints.Count != 0)
            {
                Console.WriteLine($"X: {waypoints[waypoints.Count-1].X} Y: {waypoints[waypoints.Count-1].Y}");
            }

            return placed;
        }

        internal void makeEmpty()
        {
            //Problem bei Undo: wenn wir um die Ecke gehen bei PlacePath, setzten wir nachträglich den Pfeil um
            //wenn wir nun also bei makeEmpty uns den letzten anschauen, und dieser ist eine Ecke, spackt es rum
            //vielleicht int cast auf grid zuerst kontrollieren ob er dann aúch das richtige anschaut

            if (_grid[(int)waypoints.Last().X, (int)waypoints.Last().Y] == CellType.PathRight)
            {
                checkCol--;
            }
            if (_grid[(int)waypoints.Last().X, (int)waypoints.Last().Y] == CellType.PathLeft)
            {
                checkCol++;
            }
            if (_grid[(int)waypoints.Last().X, (int)waypoints.Last().Y] == CellType.PathUp)
            {
                checkRow--;
            }
            if (_grid[(int)waypoints.Last().X, (int)waypoints.Last().Y] == CellType.PathDown)
            {
                checkRow++;
            }
            _grid[(int)waypoints.Last().X, (int)waypoints.Last().Y] = CellType.Empty;

            //_grid[(int)waypoints[waypoints.Count - 1].X, (int)waypoints[waypoints.Count - 1].Y] = CellType.Empty;
            waypoints.Remove(waypoints.Last());
            
        }

        private void enemySpawnTimer(int row)
        {
            // Creating timer with attackSpeed (millis) as interval
            // Hook up elapsed event for the timer
            //Übergabe von Parametern an OnTimedEvent: https://stackoverflow.com/questions/9977393/how-do-i-pass-an-object-into-a-timer-event
            timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, row);
            timer.AutoReset = true;
            timer.Enabled = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e, int row)
        {
            spawnEnemy(row);
            timer.Interval = enemySpawnRate;
        }

        private void spawnEnemy(int row)
        {
            var rnd = new Random();
            int spot = rnd.Next(0, 3);
            float size = 0.35f;
            var enemy = new Enemy(new Vector2(0, row), size, enemyHealth);
            enemies.Add(enemy);
            CreateParticles(25, enemy);
            if (enemySpawnRate >= 2200)
            {
                enemySpawnRate = (int)Math.Pow(enemySpawnRate, 0.9964); // 0.9964);
            }
        }

        internal CellType CheckCell(int column, int row)
        {
            return _grid[column, row];
        }

        internal bool placed;
        private bool applyScaling;
        internal int stage;
        private int checkCol = 0, checkRow = 0, spawnRow;
        internal int enemyHealth;
        private readonly IGrid _grid;
        internal double sniperCost;
        internal double rifleCost;
        internal double bouncerCost;
        private Timer timer;
        internal bool gameOver;
        //private int life;
        internal double cash;
        internal List<Vector2> waypoints;
        internal List<Enemy> enemies;
        internal List<Tower> towers;
        internal List<Bullet> bullets;
        internal List<Explosion> explosions;
        internal List<Particle> particles;
        internal int enemySpawnRate;
        internal int killCount;
        private int bounty;
        internal int towerCount;

        internal float Time { get; private set; } = 0;
    }
}
