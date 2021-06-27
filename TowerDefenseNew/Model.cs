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
            cash = 30;
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

        internal bool SwitchGameOver(bool lose)
        {
            if (lose)
            {
                gameOver = true;
            }
            else gameOver = false;
            return gameOver;
        }

        internal IReadOnlyGrid Grid => _grid;

        internal void Update(float deltaTime)
        {
            Time += deltaTime;
            UpdateScaling();
            UpdateExplosions(deltaTime);
            UpdateBullets(deltaTime);
            UpdateEnemies(deltaTime);
            UpdateParticles(deltaTime);
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
            if (towerCount >= 50)
            {

            }
        }
        private float time = 0f;
        private Vector2 deathParticles = new(.3f, .3f);
        private void UpdateParticles(float frameTime)
        {
            time += frameTime;
            if (time > .3f)
            {
                deathParticles = new Vector2(RandomJiggleX(), Math.Abs(RandomJiggleX()));
                time = 0f;
            }
            try
            {
                foreach (var particle in particles.ToList())
                {
                    particle.ApplyForce(deathParticles);
                    UpdateParticle(particle, frameTime);
                    var lifeTime = 2f; //2s
                    particle.Age += frameTime / lifeTime;
                    if (!particle.IsAlive)
                    {
                        particles.Remove(particle);
                        continue;
                    }
                }
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("update particle exception");
            }
        }
        private void UpdateParticle(Particle particle, float frameTime)
        {
            particle.Velocity += particle.Acceleration * frameTime;
            particle.Location += particle.Velocity * frameTime;
            //force was spend reset Acceleration
            particle.Acceleration = Vector2.Zero;
        }

        private readonly Random rnd = new Random(10);
        private float Random01()
        {
            return (float)rnd.NextDouble();
        }

        private float RandomJiggleX()
        {
            return (Random01() - 0.5f) * 2.0f;
        }

        private void Seed(Vector2 pos, Particle particle)
        {
            var velocity = new Vector2(RandomJiggleX() * .1f, Random01()) * 0.1f; //move upward with light x jiggle
            particle.Seed(pos, velocity);
        }

        private void CreateParticles(int count, Vector2 pos)
        {
            for (int i = 0; i < count; i++)
            {
                var particle = new Particle();
                Seed(pos + new Vector2(.5f, .5f), particle);
                particle.Age = Random01();
                particles.Add(particle);
            }
        }
        private void UpdateEnemies(float frameTime)
        {
            if (enemies.Count != 0)
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
                                enemy.ChangeDirection(direction.up);
                                enemy.wayPointIterator++;
                                continue;
                            }
                            if (enemy.Center.X < waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y >= enemy.Center.Y && enemy.dir == direction.left && enemy.dir != direction.down)
                            {
                                enemy.ChangeDirection(direction.up);
                                enemy.wayPointIterator++;
                                continue;
                            }

                            else if (enemy.Center.X >= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y <= enemy.Center.Y && enemy.dir == direction.right && enemy.dir != direction.up)
                            {
                                enemy.ChangeDirection(direction.down);
                                enemy.wayPointIterator++;
                                continue;
                            }
                            if (enemy.Center.X <= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y < enemy.Center.Y && enemy.dir == direction.left && enemy.dir != direction.up)
                            {
                                enemy.ChangeDirection(direction.down);
                                enemy.wayPointIterator++;
                                continue;
                            }
                            if (enemy.Center.X >= waypoints[enemy.wayPointIterator + 1].X && waypoints[enemy.wayPointIterator].Y > enemy.Center.Y && enemy.dir == direction.down && enemy.dir != direction.left)
                            {
                                enemy.ChangeDirection(direction.left);
                                enemy.wayPointIterator++;
                                continue;
                            }
                            if (enemy.Center.X >= waypoints[enemy.wayPointIterator + 1].X && waypoints[enemy.wayPointIterator].Y <= enemy.Center.Y && enemy.dir == direction.up && enemy.dir != direction.left)
                            {
                                enemy.ChangeDirection(direction.left);
                                enemy.wayPointIterator++;
                                continue;
                            }

                            else if (enemy.Center.X <= waypoints[enemy.wayPointIterator + 1].X && enemy.Center.Y < waypoints[enemy.wayPointIterator].Y && enemy.dir == direction.down && enemy.dir != direction.right)
                            {
                                enemy.ChangeDirection(direction.right);
                                enemy.wayPointIterator++;
                                continue;
                            }
                            else if (enemy.Center.X <= waypoints[enemy.wayPointIterator + 1].X && enemy.Center.Y >= waypoints[enemy.wayPointIterator].Y && enemy.dir == direction.up && enemy.dir != direction.right)
                            {
                                enemy.ChangeDirection(direction.right);
                                enemy.wayPointIterator++;
                                continue;
                            }
                        }
                        if (enemy.Center.X >= waypoints[waypoints.Count - 1].X)
                        {
                            enemies.Remove(enemy);
                            SwitchGameOver(true);
                            return;
                        }
                    }
                }
            }
        }

        private void UpdateExplosions(float frameTime)
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
            
        private void UpdateBullets(float frameTime)
        {
            try
            {
                foreach (Bullet bullet in bullets.ToList())
                {
                    if (bullet != null)
                    {
                        bullet.Center += new Vector2(frameTime * bullet.speedX, frameTime * bullet.speedY);
                        if (bullet.CheckHit())
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
            switch (CheckCell(column, row))
            {
                case CellType.Rifle:
                    cash += Math.Floor(rifleCost) * 0.8;
                    if (tower.Center.X == column && tower.Center.Y == row)
                    {
                        RemoveTower(column, row, tower);
                        return;
                    }
                    break;
                case CellType.Sniper:
                    cash += Math.Floor(sniperCost) * 0.8;
                    if (tower.Center.X == column && tower.Center.Y == row)
                    {
                        RemoveTower(column, row, tower);
                        return;
                    }
                    break;
                case CellType.Bouncer:
                    cash += Math.Floor(bouncerCost) * 0.8;
                    if (tower.Center.X == column && tower.Center.Y == row)
                    {
                        RemoveTower(column, row, tower);
                        return;
                    }
                    break;
            }
        }

        private void RemoveTower(int column, int row, Tower tower)
        {
            _grid[column, row] = CellType.Empty;
            towers.Remove(tower);
            tower.AsTimer(false);
            Math.Floor(cash);
            CreateParticles(25, new Vector2(column, row));
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
                if (waypoints.Count >= 2)
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
                waypoints.Add(new Vector2(column + 1, row));
                waypoints.Add(new Vector2(checkCol, row));
                placed = true;
                enemySpawnTimer(spawnRow);
            }
            return placed;
        }

        internal void MakeEmpty()
        {
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

            CreateParticles(25, new Vector2((int)waypoints.Last().X, (int)waypoints.Last().Y));
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
            SpawnEnemy(row);
            timer.Interval = enemySpawnRate;
        }

        private void SpawnEnemy(int row)
        {
            var rnd = new Random();
            int spot = rnd.Next(0, 3);
            float size = 0.35f;
            var enemy = new Enemy(new Vector2(0, row), size, enemyHealth);
            enemies.Add(enemy);
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
        private readonly Timer timer;
        internal bool gameOver;
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
        internal float Time = 0;
    }
}
