using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TowerDefenseNew.GameObjects;
using TowerDefenseNew.Grid;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
	internal class Model
	{

		public Model(IGrid grid)
		{
			this._grid = grid;
			this.waypoints = new List<Vector2>();
			this.enemies = new List<Enemy>();
			this.towers = new List<Tower>();
			this.bullets = new List<Bullet>();
			this.explosions = new List<Explosion>();

			this.cash = 50;
			//this.life = 1;
			this.sniperCost = 20;
			this.rifleCost = 5;
			this.enemySpawnRate = 3000;
			this.enemyHealth = 500;
			this.gameOver = false;
			this.timer = new System.Timers.Timer(this.enemySpawnRate);
			this.killCount = 0;
			this.placed = false;
		}

		internal bool switchGameOver(bool lose)
        {
			if (lose)
            {
				this.gameOver = true;
            }
			else this.gameOver = false;
			return this.gameOver;
        }

        internal void giveCash()
        {
			this.cash += 1000;
			Console.Write("New balance: " + this.cash);
        }

        internal IReadOnlyGrid Grid => _grid;

		internal void Update(float deltaTime)
		{
			Time += deltaTime;
			UpdateExplosions(deltaTime);
			UpdateBullets(deltaTime);
			UpdateEnemies(deltaTime);
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
							if (enemy.wayPointIterator < waypoints.Count-1)
							{
								if (enemy.Center.X >= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y > enemy.Center.Y && enemy.dir == direction.right && enemy.dir != direction.down)
								{
									enemy.changeDirection(direction.up);
									enemy.wayPointIterator++;
									continue;
								}
								//new
								if (enemy.Center.X < waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y > enemy.Center.Y && enemy.dir == direction.left && enemy.dir != direction.down)
								{
									//if (enemy == enemies[0]) Console.WriteLine("up");
									enemy.changeDirection(direction.up);
									enemy.wayPointIterator++;
									break;
								}

								else if (enemy.Center.X >= waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y < enemy.Center.Y && enemy.dir == direction.right && enemy.dir != direction.up)
								{
									enemy.changeDirection(direction.down);
									enemy.wayPointIterator++;
									continue;
								}
								//new
								if (enemy.Center.X < waypoints[enemy.wayPointIterator].X && waypoints[enemy.wayPointIterator + 1].Y < enemy.Center.Y && enemy.dir == direction.left && enemy.dir != direction.up)
								{
									//if (enemy == enemies[0]) Console.WriteLine("up");
									enemy.changeDirection(direction.down);
									enemy.wayPointIterator++;
									break;
								}
								//new
								if (enemy.Center.X > waypoints[enemy.wayPointIterator + 1].X && waypoints[enemy.wayPointIterator].Y > enemy.Center.Y && enemy.dir == direction.down && enemy.dir != direction.left)
								{
									//if (enemy == enemies[0]) Console.WriteLine("up");
									enemy.changeDirection(direction.left);
									enemy.wayPointIterator++;
									break;
								}
								//new
								if (enemy.Center.X > waypoints[enemy.wayPointIterator + 1].X && waypoints[enemy.wayPointIterator].Y <= enemy.Center.Y && enemy.dir == direction.up && enemy.dir != direction.left)
								{
									//if (enemy == enemies[0]) Console.WriteLine("up");
									enemy.changeDirection(direction.left);
									enemy.wayPointIterator++;
									break;
								}
								
								else if (enemy.Center.X < waypoints[enemy.wayPointIterator + 1].X && enemy.Center.Y < waypoints[enemy.wayPointIterator].Y && enemy.dir == direction.down && enemy.dir != direction.right)
								{
									//if (enemy == enemies[0]) Console.WriteLine("right");
									enemy.changeDirection(direction.right);
									enemy.wayPointIterator++;
									continue;
								}
								else if (enemy.Center.X < waypoints[enemy.wayPointIterator + 1].X && enemy.Center.Y >= waypoints[enemy.wayPointIterator].Y && enemy.dir == direction.up && enemy.dir != direction.right)
								{
									enemy.changeDirection(direction.right);
									enemy.wayPointIterator++;
									continue;
								}
							}

							if (enemy.Center.X >= waypoints[waypoints.Count-1].X)
                            {
								enemies.Remove(enemy);
								switchGameOver(true);
								return;
							}

							/*//Check if end of lane is reached
							if (CheckRightFinish(enemy.Center))
                            {
								enemies.Remove(enemy);
								switchGameOver(true);
								return;
                            }
							else return;
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
			try {
				foreach (Bullet bullet in bullets.ToList())
				{
					if (bullet != null)
					{
						bullet.Center += new Vector2(frameTime * bullet.speedX, frameTime * bullet.speedY);
						if (bullet.checkHit())
						{
							//onEnemyKill
							this.killCount++;
							this.cash++;
							Explosion exp = new Explosion(bullet.Center+new Vector2(0.45f,1.5f), 2f, 0.75f);
							explosions.Add(exp);
							Console.WriteLine("Enemy killed. Cash: " + this.cash);
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
			if(CheckCell(column, row) == CellType.Rifle)
            {
				this.cash += Math.Floor(rifleCost) * 0.8;
				if (tower.Center.X == column && tower.Center.Y == row)
				{
					_grid[column, row] = CellType.Empty;
					towers.Remove(tower);
					tower.asTimer(false);
					Math.Floor(this.cash);
				}
				else return;
			}
			else if (CheckCell(column, row) == CellType.Sniper)
            {
				this.cash += Math.Floor(sniperCost) * 0.8;
				if (tower.Center.X == column && tower.Center.Y == row)
				{
					_grid[column, row] = CellType.Empty;
					towers.Remove(tower);
					tower.asTimer(false);
					Math.Floor(this.cash);
				}
				else return;
			}
			else
            {
				return;
            }
			
		}

		internal void PlaceSniper(int column, int row)
		{
			if (this.cash >= this.sniperCost)
			{
				Tower tower = new Tower(new Vector2(column, row), 12f, 20, 1000, this.enemies, this.bullets, 0);
				_grid[column, row] = CellType.Sniper;
				this.towers.Add(tower);
				this.cash -= this.sniperCost;
				Math.Floor(this.cash);
				Console.WriteLine("Sniper bought for: " + this.sniperCost + " || New balance: " + this.cash);
			}
			else return;
		}

		internal void PlaceRifle(int column, int row)
		{
			if (this.cash >= this.rifleCost)
			{
				Tower tower = new Tower(new Vector2(column, row), 3f, 10, 100, this.enemies, this.bullets, 1);
				_grid[column, row] = CellType.Rifle;
				this.towers.Add(tower);
				this.cash -= this.rifleCost;
				Math.Floor(this.cash);
				Console.WriteLine("Rifle bought for: " + this.rifleCost + " || New balance: " + this.cash);
			}
			else return;
		}
		bool placePoint = true;
		internal bool PlacePath(int column, int row)
		{
			//First is always placed left
			if (column == 0 && waypoints.Count == 0)
			{
				_grid[column, row] = CellType.Path;
				waypoints.Add(new Vector2(column, row));
				checkCol++;
				checkRow = row;
				spawnRow = row;
			}
			//right
			else if (column > checkCol && row == checkRow && column != 0)
			{
				if(_grid[column, row] != CellType.Path)
                {
					_grid[column, row] = CellType.Path;
				}
				else
				{
					checkCol++;
				}
				if (placePoint == false) waypoints.Add(new Vector2(column - 1, row)); placePoint = true;
				checkCol++;
			}
			//left
			else if (column < checkCol && row == checkRow && column != 0)
			{
				if (_grid[column, row] != CellType.Path)
				{
					_grid[column, row] = CellType.Path;
                }
                else
                {
					checkCol--;
				}
				if (placePoint == false) waypoints.Add(new Vector2(column - 1, row)); placePoint = true;
				checkCol--;
			}
			else if (column == checkCol && row == checkRow && column != 0) 
			{
				if (_grid[column, row] != CellType.Path)
				{
					_grid[column, row] = CellType.Path;
				}
			}
			//up+down
			else if(column == checkCol  && (row == checkRow + 1 || row == checkRow - 1) && CheckCell(checkCol, row) != CellType.Finish && column != 0)
            {
				if (_grid[column, row] != CellType.Path)
				{
					_grid[column, row] = CellType.Path;
				}
				if (placePoint == true) waypoints.Add(new Vector2(column, checkRow)); placePoint = false;
				checkRow = row;
			}
			if(CheckCell(checkCol, row) == CellType.Finish && this.placed == false)
            {
				_grid[checkCol, row] = CellType.Path;
				waypoints.Add(new Vector2(checkCol, row));
				this.placed = true;
				enemySpawnTimer(spawnRow);
            }
			Console.WriteLine($"checkCol = {checkCol}, {row} is " + CheckCell(checkCol, row));

			if (placed == true)
			{
				for (int i = 0; i < waypoints.Count; i++)
				{
					Console.WriteLine($"WayPoint at {i}\n X: {waypoints[i].X} Y: {waypoints[i].Y}\n");
				}
			}
			return placed;
		}



		private void enemySpawnTimer(int row)
		{
			// Creating timer with attackSpeed (millis) as interval
			// Hook up elapsed event for the timer
			//Übergabe von Parametern an OnTimedEvent: https://stackoverflow.com/questions/9977393/how-do-i-pass-an-object-into-a-timer-event
			this.timer.Elapsed += (sender,e) => OnTimedEvent(sender, e, row); 
			this.timer.AutoReset = true;
			this.timer.Enabled = true;
			
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e, int row)
		{
			spawnEnemy(row);
			this.timer.Interval = this.enemySpawnRate;
		}

		private void spawnEnemy(int row)
        {
			var rnd = new Random();
			int spot = rnd.Next(0, 3);
			float size = 0.35f;
			enemies.Add(new Enemy(new Vector2(0, row), size, enemyHealth * 2));
			/*switch (spot)
			{
				case 0:
					size = 0.3f;
					offset = 0.35f;
					enemies.Add(new Enemy(new Vector2(offset, row), size, enemyHealth));
					break;
				case 1:
					size = 0.4f;
					offset = 0.5f;
					enemies.Add(new Enemy(new Vector2(offset, row), size, enemyHealth * 2));
					break;
				case 2:
					size = 0.3f;
					offset = 0.65f;
					enemies.Add(new Enemy(new Vector2(offset, row), size, enemyHealth));
					break;
				default:
					return;
			}*/
			if (enemySpawnRate >= 2000)
            {
				this.enemySpawnRate = (int)Math.Pow(this.enemySpawnRate, 0.9964); // 0.9964);
			}
		}

		internal CellType CheckCell(int column, int row)
        {
			return _grid[column, row];
        }

		internal bool CheckRightPath(Vector2 vec)
		{
			try {
				if (_grid[(int)vec.X + 1, (int)vec.Y] == CellType.Path) return true;
				else return false;
			}
			catch (System.IndexOutOfRangeException)
			{
				return false;
			}
		}

		internal bool CheckRightFinish(Vector2 vec)
        {
			try
			{
				if (_grid[(int)vec.X, (int)vec.Y + 1] == CellType.Finish && _grid[(int)vec.X, (int)vec.Y - 1] == CellType.Finish) return true;
				else return false;
			}
			catch (System.IndexOutOfRangeException)
			{
				return false;
			}
		}

		internal bool CheckUpperPath(Vector2 vec)
		{
			try
			{
				if (_grid[(int)vec.X, (int)vec.Y + 1] == CellType.Path) return true;
				else return false;
			}
			catch (System.IndexOutOfRangeException)
			{
				return false;
			}
}
		internal bool CheckLowerPath(Vector2 vec)
		{
			try
			{
				if (_grid[(int)vec.X, (int)vec.Y - 1] == CellType.Path) return true;
				else return false;
			}
			catch (System.IndexOutOfRangeException)
			{
				return false;
			}
		}

		internal bool CheckCellPosition(Vector2 vec)
        {
			try
			{
				if (_grid[(int)vec.X, (int)vec.Y] != CellType.Empty) return true;
				else return false;
			}
			catch (System.IndexOutOfRangeException)
			{
				return false;
			}
		}

		internal void RemoveTower(Tower tower)
        {
			Console.WriteLine("Removing tower at " + $"{tower.Center}");
			this.towers.Remove(tower);
			tower.asTimer(false);
        }

		private bool placed;
		private int checkCol = 0, checkRow = 0, spawnRow; 
		internal int enemyHealth;
        private readonly IGrid _grid;
        private readonly List<Vector2> waypoints;
        internal double sniperCost;
		internal double rifleCost;
        private Timer timer;
		internal bool gameOver;
        //private int life;
        internal double cash;
        internal List<Enemy> enemies;
		internal List<Tower> towers;
		internal List<Bullet> bullets;
		internal List<Explosion> explosions;
		internal int enemySpawnRate;
        internal int killCount;
        internal float Time { get; private set; } = 0;
	}
}
