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
			this.pathway = new List<CellType>();
			this.enemies = new List<Enemy>();
			this.towers = new List<Tower>();
			this.bullets = new List<Bullet>();
			this.cash = 50;
			//this.life = 1;
			this.sniperCost = 20;
			this.rifleCost = 5;
			this.enemySpawnRate = 1000;
			this.enemyHealth = 100;
			this.timer = new System.Timers.Timer(this.enemySpawnRate);
		}

        internal void giveCash()
        {
			this.cash += 1000;
			Console.Write("New balance: " + this.cash);
        }

        internal IReadOnlyGrid Grid => _grid;

		internal void Update(float deltaTime)
		{
			UpdateBullets(deltaTime);
			UpdateEnemies(deltaTime);
		}

		private void UpdateEnemies(float frameTime)
		{
			if (enemies.Count != 0)
			{
				try
				{
					foreach (Enemy enemy in enemies.ToList())
					{
						enemy.Center += new Vector2(frameTime * enemy.Velocity.X, frameTime * enemy.Velocity.Y);

						if (enemy != null)
						{
							//Check if moving up or down or right, then move right
							if (CheckRightPath(enemy.Center + new Vector2(-0.5f, 0)) && enemy.dir == direction.right)
							{
								enemy.changeDirection(direction.right);
								continue;
							}
							else if (CheckRightPath(enemy.Center + new Vector2(-0.5f, -0.5f)) && enemy.dir == direction.up)
							{
								enemy.changeDirection(direction.right);
								continue;
							}
							else if (CheckRightPath(enemy.Center + new Vector2(0.5f, 0.5f)) && enemy.dir == direction.down)
							{
								enemy.changeDirection(direction.right);
								continue;
							}
							//Check if moving right or up, then move up
							else if (CheckUpperPath(enemy.Center + new Vector2(-0.5f, 0)) && enemy.dir == direction.right)
							{
								enemy.changeDirection(direction.up);
								continue;
							}
							else if (CheckUpperPath(enemy.Center + new Vector2(-0.5f, -0.5f)) && enemy.dir == direction.up)
							{
								enemy.changeDirection(direction.up);
								continue;
							}
							//Check if moving right or down, then move down
							else if (CheckLowerPath(enemy.Center + new Vector2(-0.5f, 0f)) && enemy.dir == direction.right)
							{
								enemy.changeDirection(direction.down);
								continue;
							}
							else if (CheckLowerPath(enemy.Center + new Vector2(0f, 0.5f)) && enemy.dir == direction.down)
							{
								enemy.changeDirection(direction.down);
								continue;
							}
							else return;
						}
					}
				}
				catch (System.ArgumentException)
				{
					Console.WriteLine("UpdateEnemies ArgumentException");
				}
			}
		}

		private void UpdateBullets(float frameTime)
		{
			try {
				foreach (Bullet bullet in bullets.ToList())
				{
					bullet.Center += new Vector2(frameTime * bullet.speedX, frameTime * bullet.speedY);
					if (bullet.checkHit())
					{
						//onEnemyKill
						this.cash = this.cash + 1;
						Console.WriteLine("Enemy killed. Cash: " + this.cash);
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

		internal void ClearCell(int column, int row, double towerCost)
        {
			_grid[column, row] = CellType.Empty;
			this.cash += Math.Floor(towerCost) * 0.8;
			Math.Floor(this.cash);
		}

		internal void PlaceSniper(int column, int row)
		{
			if (this.cash >= this.sniperCost)
			{
				Tower tower = new Tower(new Vector2(column, row), 9f, 10, 1000, this.enemies, this.bullets);
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
				Tower tower = new Tower(new Vector2(column, row), 3f, 5, 100, this.enemies, this.bullets);
				_grid[column, row] = CellType.Rifle;
				this.towers.Add(tower);
				this.cash -= this.rifleCost;
				Math.Floor(this.cash);
				Console.WriteLine("Rifle bought for: " + this.rifleCost + " || New balance: " + this.cash);
			}
			else return;
		}
		internal bool PlacePath(int column, int row)
		{
			bool placed = false;
			for (int i = 0; i < 10; i++)
			{
				_grid[i, 0] = CellType.Path;
				pathway.Add(_grid[i, 0]);
			}
			for (int i = 0; i < 9; i++)
			{
				_grid[9, i] = CellType.Path;
				pathway.Add(_grid[9, i]);
			}
			for (int i = 9; i < 19; i++)
			{
				_grid[i, 9] = CellType.Path;
				pathway.Add(_grid[i, 9]);
			}
			for (int i = 9; i > 3; i--)
			{
				_grid[19, i] = CellType.Path;
				pathway.Add(_grid[19, i]);
			}
			for (int i = 19; i < 54; i++)
			{
				_grid[i, 3] = CellType.Path;
				pathway.Add(_grid[i, 3]);
			}
			placed = true;
			Console.WriteLine(pathway.Count);
			Console.WriteLine("placed path");
			enemySpawnTimer(0);
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
			switch (spot)
			{
				case 0:
					enemies.Add(new Enemy(new Vector2(0 + 0.35f, row + 0.35f), .25f, enemyHealth));
					break;
				case 1:
					enemies.Add(new Enemy(new Vector2(0 + 0.5f, row + 0.5f), .25f, enemyHealth));
					break;
				case 2:
					enemies.Add(new Enemy(new Vector2(0 + 0.65f, row + 0.65f), .25f, enemyHealth));
					break;
				default:
					return;
			}
			if (enemySpawnRate >= 500)
            {
				this.enemySpawnRate = (int)Math.Pow(this.enemySpawnRate, 0.8); // 0.9964);
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

		internal bool checkCellTower(Vector2 vec)
		{
			try
			{
				if (_grid[(int)vec.X, (int)vec.Y] == CellType.Rifle || _grid[(int)vec.X, (int)vec.Y] == CellType.Sniper) return true;
				else return false;
			}
			catch (System.IndexOutOfRangeException)
			{
				return false;
			}
		}

		internal void RemoveTower(Tower tower)
        {
			this.towers.RemoveAt(this.towers.IndexOf(tower));
			tower.asTimer(false);
        }

		internal int enemyHealth;
        private readonly IGrid _grid;
		internal double sniperCost;
		internal double rifleCost;
        private Timer timer;

        //private int life;
        internal double cash;
		private List<CellType> pathway;
        internal List<Enemy> enemies;
		internal List<Tower> towers;
		internal List<Bullet> bullets;
		internal int enemySpawnRate;
	}
}
