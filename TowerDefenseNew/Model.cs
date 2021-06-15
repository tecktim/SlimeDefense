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
            if (column == 0 && waypoints.Count == 0)
            {
                _grid[column, row] = CellType.PathRight;
                waypoints.Add(new Vector2(column, row));
                checkCol++;
                checkRow = row;
                spawnRow = row;
            }
            //left to right
            else if (column == checkCol && row == checkRow && column != 0)
            {
                if (CheckCell(column - 1 , row - 1) == CellType.PathUp || CheckCell(column - 1, row - 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                {
                    _grid[column - 1, row] = CellType.PathRight;
                }
                if (CheckCell(column - 1, row + 1) == CellType.PathDown || CheckCell(column - 1, row + 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                {
                    _grid[column - 1, row] = CellType.PathRight;
                }

                if (CellTypeIsAnyPath(column + 1, row))
                {
                    for (int i = column; i < 51; i++)
                    {
                        if (CheckCell(i + 2, row) == CellType.Empty)
                        {
                            //Check if dead end loch (dein lieblings dead end, max)
                            if (CheckCell(i + 3, row) == CellType.Path)
                            {
                                _grid[column, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                _grid[i + 1, row] = CellType.PathCross; // Path für Eckpfeile, PathRight für alle Pfeile
                                _grid[i + 2, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                continue;
                            }
                            else
                            {
                                _grid[column, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                _grid[i + 1, row] = CellType.PathCross;
                                _grid[i + 2, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                checkCol = i + 3;
                                break;
                            }
                        }
                        else continue;
                    }
                }
                else if (CheckCell(checkCol, row) == CellType.Empty)
                {
                    _grid[column, row] = CellType.Path;
                    checkCol++;
                }
                if (placePoint == false)
                {
                    waypoints.Add(new Vector2(column - 1, row));
                    placePoint = true;
                }
            }

            //right to left
            else if (column == checkCol - 2 && row == checkRow && column != 0)
            {
                if (CheckCell(column + 1, row - 1) == CellType.PathUp || CheckCell(column + 1, row - 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                {
                    _grid[column + 1, row] = CellType.PathLeft;
                }
                if (CheckCell(column + 1, row + 1) == CellType.PathDown || CheckCell(column + 1, row + 1) == CellType.PathDown) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                {
                    _grid[column + 1, row] = CellType.PathLeft;
                }
                if (CellTypeIsAnyPath(column - 1, row))
                {
                    for (int i = column; i > 1; i--)
                    {
                        if (CheckCell(i - 1, row) == CellType.Empty)
                        {   
                            //Check if dead end loch (dein lieblings dead end, max)
                            if (CheckCell(i - 2, row) == CellType.Path)
                            {
                                _grid[column, row] = CellType.Path;
                                _grid[i, row] = CellType.PathCross;
                                _grid[i - 1, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                continue;
                            }
                            else
                            {
                                
                                _grid[column, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                _grid[i, row] = CellType.PathCross;
                                _grid[i - 1, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                                                   //_grid[i - 2, row] = CellType.Path; // Path für Eckpfeile, PathRight für alle Pfeile
                                checkCol = i;
                                break;
                            }
                        }
                        else continue;
                    }
                }
                else
                {
                    _grid[column, row] = CellType.Path;
                    checkCol--;
                }
                if (placePoint == false)
                {
                    waypoints.Add(new Vector2(column - 1, row));
                    placePoint = true;
                }
            }
            else if (column == checkCol - 1 && row == checkRow - 1 && CheckCell(checkCol, row) != CellType.Finish && column != 0)
            {
                if (row > 0)
                { 
                if (CheckCell(column - 1, row + 1) == CellType.PathRight || CheckCell(column - 1, row + 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                    {
                _grid[column, row + 1] = CellType.PathDown;
                }
                if (CheckCell(column + 1, row + 1) == CellType.PathLeft || CheckCell(column + 1, row + 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                    {
                _grid[column, row + 1] = CellType.PathDown;
                }
                    if (CellTypeIsAnyPath(column, row - 1) && !CellTypeIsAnyPath(column, row - 2) && !CellTypeIsAnyPath(column, row - 3))
                    {
                        _grid[column, row] = CellType.Path; // Path für Eckpfeile, PathDown für alle Pfeile
                        _grid[column, row - 1] = CellType.PathCross;
                        _grid[column, row - 2] = CellType.Path; // Path für Eckpfeile, PathDown für alle Pfeile
                        checkRow -= 3;
                    }
                    else
                    {
                        _grid[column, row] = CellType.Path;
                        checkRow = row;
                    }
                    if (placePoint == true)
                    {
                        waypoints.Add(new Vector2(column, checkRow));
                        placePoint = false;
                    }

                }
            }
            else if (column == checkCol - 1 && row == checkRow + 1 && CheckCell(checkCol, row) != CellType.Finish && column != 0)
            {
                if (row < 29)
                {
                if (CheckCell(column - 1, row - 1) == CellType.PathRight || CheckCell(column - 1, row - 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                    {
                    _grid[column, row - 1] = CellType.PathUp;
                }
                if (CheckCell(column + 1, row - 1) == CellType.PathLeft || CheckCell(column + 1, row - 1) == CellType.Path) //Vorne ist für alle durchgehende Pfeile, hinten für nur Eckpfeile, hinten ist entfernbar wenn man alle Pfeile möchte
                    {
                    _grid[column, row - 1] = CellType.PathUp;
                }
                
                    if (CellTypeIsAnyPath(column, row + 1) && !CellTypeIsAnyPath(column, row + 2) && !CellTypeIsAnyPath(column, row + 3))
                    {
                        _grid[column, row] = CellType.Path; // Path für Eckpfeile, PathUp für alle Pfeile
                        _grid[column, row + 1] = CellType.PathCross;
                        _grid[column, row + 2] = CellType.Path; // Path für Eckpfeile, PathUp für alle Pfeile
                        checkRow += 3;
                    }
                    else
                    {
                        _grid[column, row] = CellType.Path;
                        checkRow = row;
                    }
                    if (placePoint == true)
                    {
                        waypoints.Add(new Vector2(column, checkRow));
                        placePoint = false;
                    }
                }
            }
            if (CheckCell(checkCol, row) == CellType.Finish && placed == false)
            {
                stage = 1;
                _grid[checkCol, row] = CellType.PathRight;
                waypoints.Add(new Vector2(checkCol, row));
                placed = true;
                enemySpawnTimer(spawnRow);
            }
            return placed;
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
            enemies.Add(new Enemy(new Vector2(0, row), size, enemyHealth));
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
        private bool placePoint = true;
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
        internal int enemySpawnRate;
        internal int killCount;
        private int bounty;
        internal int towerCount;

        internal float Time { get; private set; } = 0;
    }
}
