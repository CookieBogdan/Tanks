// Decompiled with JetBrains decompiler
// Type: Tanks.v2.Models.Game
// Assembly: Tanks.v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A682BDB5-0425-49B3-A206-61CFFE0B3FF4
// Assembly location: C:\Downloads\Tanks.v2.exe\Tanks.v2.01.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tanks.v2.Enums;


namespace Tanks.v2.Models
{
	public class Game
	{
		public GameField Field;
		public Tank Tank;
		public List<EvilTank> EvilTanks;
		public List<Bullet> Bullets;
		public List<Bonus> Bonuses = new List<Bonus>();
		public List<EvilTank> TemporaryAddEvilTank = (List<EvilTank>)null;
		public List<EvilTank> TemporaryRemoveEvilTank = (List<EvilTank>)null;
		public bool TankIsLive = true;
		public bool IsPlaying = true;
		public bool Stop = false;
		public bool IsWaitBack = false;
		private int Wave;
		private int countTanksInWave;
		private int spawningTanksInWave;
		private string nameMap;
		private int timeBeforeRespawn = 5;
		public Stopwatch stopWatch = new Stopwatch();
		public Game(LandscapeType[,] field, string name)
		{
			nameMap = name;
			PreparingToCreate();
			Field.CreateMap(field);
		}
		private void PreparingToCreate()
		{
			Field = new GameField();
			Tank = new Tank(this);
			Tank.Orientation = OrientationType.Up;
			Tank.X = 108;
			Tank.Y = Field.Height - Tank.Size;
			Tank.XPos = Tank.X / 6;
			Tank.YPos = Tank.Y / 6;
			for (int i = 0; i < 5; i++)
			for (int j = 0; j < 5; j++)
				Tank.Position[Tank.XPos + i, Tank.YPos + j] = 1;

			EvilTanks = new List<EvilTank>();
			Task.Run(SpawnEvilTanks);
			Task.Run(SpawnBonuses);

			Bullets = new List<Bullet>();
			stopWatch.Start();
		}
		public void Draw(Graphics g)
		{
			Field.Draw(g, 1);
			if(TankIsLive)
				Tank.Draw(g, 30, 30);
			if (EvilTanks != null)
			{
				foreach (EvilTank tank in EvilTanks)
				{
					tank.Draw(g, 30, 30);
					if (tank.EvilBullets == null) continue;
					foreach (Bullet bullet in tank.EvilBullets)
						bullet.Draw(g, 30, 30);
				}
			}
			foreach (Bullet bullet in Bullets)
				bullet.Draw(g, 30, 30);
			foreach (Bonus bonus in Bonuses)
				bonus.Draw(g, 30, 30);
			Font roboto = new Font("Roboto", 14);
			Field.Draw(g, 3);
			g.DrawString($"Wave: {Wave}",
						   roboto,
						   Brushes.Black,
						   30 + Field.Width + 10,
						   40);
			g.DrawString($"{spawningTanksInWave}/{countTanksInWave}",
						   roboto,
						   Brushes.Black,
						   30 + Field.Width + 10,
						   60);
			g.DrawString($"Lives: {Tank?.CountLive ?? 0}",
						   roboto,
						   Brushes.Black,
						   30 + Field.Width + 10,
						   80);
			g.DrawString($"{stopWatch.Elapsed.Minutes}:{stopWatch.Elapsed.Seconds}",
						   roboto,
						   Brushes.Black,
						   30 + Field.Width + 10,
						   100);
			if (!TankIsLive && (timeBeforeRespawn == 3 || timeBeforeRespawn == 2 || timeBeforeRespawn == 1))
			{
				Brush color = Brushes.White;
				if (timeBeforeRespawn == 3)
					color = Brushes.Red;
				if (timeBeforeRespawn == 2)
					color = Brushes.Green;
				if (timeBeforeRespawn == 1)
					color = Brushes.Yellow;

				g.DrawString($"{timeBeforeRespawn}",
					new Font("Bauhaus 93", 56),
					color,
					230,
					230
					);
			}

		}
		private void SpawnEvilTanks()
		{
			Random random = new Random();

			Wave = 1;
			int timeBeforeWave = 6;
			countTanksInWave = 5;
			int timeAfterSpawnOneTank = 5;
			while (true)
			{
				for (int i = 0; i < timeBeforeWave; i++)
				{
					Thread.Sleep(1000);
					WaitStop();
				}
				for (int index = 0; index < countTanksInWave; index++)
				{
					if (!IsPlaying)
						return;
					int rPos = RandomPosition();
					EvilTank tank = new EvilTank(this)
					{
						Orientation = OrientationType.Up,
						X = rPos * 30,
						Y = 0
					};
					tank.XPos = tank.X / 6;
					tank.YPos = tank.Y / 6;
					for (int i = 0; i < 5; i++)
					for (int j = 0; j < 5; j++)
						tank.Position[tank.XPos + i, tank.YPos + j] = 1;
					TemporaryAddEvilTank ??= new List<EvilTank>();
					TemporaryAddEvilTank.Add(tank);
					Task.Run(() => PlaySound(Sounds.Spawn));
					spawningTanksInWave++;
					for (int i = 0; i < timeAfterSpawnOneTank; i++)
					{
						Thread.Sleep(1000);
						WaitStop();
					}
				}
				while (true) // finish wave? or not
				{
					Thread.Sleep(1000);
					if (EvilTanks.Count == 0)
						break;
				}
				Wave++;
				spawningTanksInWave = 0;
				countTanksInWave = Convert.ToInt32(countTanksInWave * 1.31);
				if(timeAfterSpawnOneTank > 500)
					timeAfterSpawnOneTank -= 50;
				if (timeBeforeWave > 2000)
					timeBeforeWave -= 20;
			}
			int RandomPosition()
			{
				List<Tanks> allTanks = TankIsLive ?
					EvilTanks.Cast<Tanks>().Union(new List<Tanks> { Tank }).ToList() :
					EvilTanks.Cast<Tanks>().ToList();
				int rPos = random.Next(15);
				foreach (Tanks t in allTanks)
				{
					if (t.Position[rPos * 5, 0] == 1)
					{
						return RandomPosition();
					}
				}
				return rPos;
			}
		}
		private void SpawnBonuses()
		{
			Random r = new Random();
			for (int i = 0; i < 20; i++)
			{
				Thread.Sleep(1000);
				WaitStop();
			}
			while (true)
			{
				if(!IsPlaying)
					return;
				List<int> maybeBonus = new List<int>() { 0, 1, 2, 3 };
				switch (r.Next(maybeBonus.Count)) // !!!
				{
					case 0:
						Bonuses.Add(new HeartBonus(this));
						break;
					case 1:
						Bonuses.Add(new SpeedBonus(this));
						break;
					case 2:
						Bonuses.Add(new KillBonus(this));
						break;
					case 3:
						Bonuses.Add(new BulletBonus(this));
						break;
					default:
						break;
				}

				int randomSec = r.Next(20, 30);
				for (int i = 0; i < randomSec; i++)
				{
					Thread.Sleep(1000);
					WaitStop();
				}
			}
		}
		public void ExplosionTimer(int x, int y)
		{
			LandscapeType type = Field.MapInBlocks[x, y];
			if(type == LandscapeType.Explosion)
				return;
			Field.MapInBlocks[x, y] = LandscapeType.Explosion;
			Task.Run(() => PlaySound(Sounds.Explosion));
			Thread.Sleep(500);
			Field.MapInBlocks[x, y] = type;
		}
		public void PlaySound(Sounds s)
		{
			Sound sound = new Sound();
			sound.Play(s);
		}
		public void DeadTimer()
		{
			if (!IsPlaying)
				return;
			TankIsLive = false;
			int x = (int)Math.Round(Convert.ToDouble(Tank.XPos) / 5, MidpointRounding.AwayFromZero);
			int y = (int)Math.Round(Convert.ToDouble(Tank.YPos) / 5, MidpointRounding.AwayFromZero);
			Task.Run(() => ExplosionTimer(x, y));
			Tank = null;
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(1000);
				timeBeforeRespawn--;
			}

			timeBeforeRespawn = 5;
			if (!IsPlaying)
				return;
			Tank = new Tank(this);
			Tank.Orientation = OrientationType.Up;
			Tank.X = 108;
			Tank.Y = Field.Height - Tank.Size;
			Tank.XPos = Tank.X / 6;
			Tank.YPos = Tank.Y / 6;
			for (int i = 0; i < 5; i++)
			for (int j = 0; j < 5; j++)
				Tank.Position[Tank.XPos + i, Tank.YPos + j] = 1;
			TankIsLive = true;
			Task.Run(() => PlaySound(Sounds.Spawn));
		}
		public void GameOver()
		{
			IsPlaying = false;
			TankIsLive = false;
			stopWatch.Stop();
			Record();
			Tank = null;
			Thread.Sleep(500);
			Task.Run(() => PlaySound(Sounds.GameOver));
			Thread.Sleep(5500);
			IsWaitBack = true;
		}
		public void Record()
		{
			string recordPath = @"..\..\..\records.txt";
			List<string> records = File.ReadAllLines(recordPath).ToList();
			if(nameMap == null) return;
			int number = -1;
			for(int i = 0; i < records.Count; i++)
				if (records[i].Contains(nameMap))
					number = i;
			if (number != -1)
			{
				string minSec = records[number][(records[number].IndexOf(".map") + 5)..];
				int min = Convert.ToInt32(minSec.Split(" ").ToList()[0]);
				int sec = Convert.ToInt32(minSec.Split(" ").ToList()[1]);
				if (min <= stopWatch.Elapsed.Minutes && sec < stopWatch.Elapsed.Seconds)
				{
					records[number] = $"{nameMap} {stopWatch.Elapsed.Minutes} {stopWatch.Elapsed.Seconds}";
					File.WriteAllLines(recordPath, records);
				}
			}
			else
			{
				records.Add($"{nameMap} {stopWatch.Elapsed.Minutes} {stopWatch.Elapsed.Seconds}");
				File.WriteAllLines(recordPath, records);
			}
		}
		public void TimerSpeed(Tanks tank)
		{
			tank.fastSpeed = true;
			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(1000);
				WaitStop();
			}
			tank.fastSpeed = false;
		}
		public void TimerBullet(Tanks tank)
		{
			tank.bullets *= 2;
			for (int i = 0; i < 15; i++)
			{
				Thread.Sleep(1000);
				WaitStop();
			}
			tank.bullets /= 2;
		}
		private void WaitStop()
		{
			while (true)
			{
				if(!Stop)
					return;
				Thread.Sleep(500);
			}
		}
	}
}
