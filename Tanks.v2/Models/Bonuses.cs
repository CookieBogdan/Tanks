using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tanks.v2.Enums;

namespace Tanks.v2.Models
{
	public abstract class Bonus
	{
		protected string BonusImage;
		public Game _game;

		protected Bonus(Game game)
		{
			_game = game;
			Spawn();
		}

		public int X { get; private set; }
		public int Y { get; private set; }

		public virtual void Draw(Graphics g, int offsetX, int offsetY)
		 {
			Image image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(BonusImage));
			g.DrawImage(image, offsetX + X, offsetY + Y);
		}

		public Tanks IsTake()
		{
			List<Tanks> allTanks = _game.TankIsLive ?
				_game.EvilTanks.Cast<Tanks>().Union(new List<Tanks> { _game.Tank }).ToList() :
				_game.EvilTanks.Cast<Tanks>().ToList();
			for (int i = 0; i < 5; i++)
			for (int j = 0; j < 5; j++)
			{
				foreach (Tanks tank in allTanks)
				{
					if (tank.Position[X / 6 + i, Y / 6 + j] == 1)
					{
						Task.Run(() => _game.PlaySound(Sounds.Take));
						return tank;
					}
				}
			}

			return null;
		}

		public abstract void Using(Tanks tank);

		private void Spawn()
		{
			Random r = new Random();
			List<List<int>> none = new List<List<int>>();
			for (int i = 0; i < 15; i++)
			for (int j = 0; j < 15; j++)
			{
				if (_game.Field.MapInBlocks[i, j] == LandscapeType.None)
					none.Add(new List<int>() {i, j});
			}

			int ran = r.Next(none.Count);
			X = none[ran][0] * 30;
			Y = none[ran][1] * 30;
		}
	}

	public class HeartBonus : Bonus
	{
		public HeartBonus(Game game) : base(game)
		{
			BonusImage = "Tanks.v2.Images.heart_bonus.png";
		}

		public override void Using(Tanks tank)
		{
			tank.CountLive++;
		}
	}
	public class SpeedBonus : Bonus
	{
		public SpeedBonus(Game game) : base(game)
		{
			BonusImage = "Tanks.v2.Images.speed_bonus.png";
		}

		public override void Using(Tanks tank)
		{
			Task.Run(() => _game.TimerSpeed(tank));
		}
	}
	public class BulletBonus : Bonus
	{
		public BulletBonus(Game game) : base(game)
		{
			BonusImage = "Tanks.v2.Images.bullet_bonus.png";
		}

		public override void Using(Tanks tank)
		{
			Task.Run(() => _game.TimerBullet(tank));
		}
	}
	public class KillBonus : Bonus
	{
		public KillBonus(Game game) : base(game)
		{
			BonusImage = "Tanks.v2.Images.kill_bonus.png";
		}

		public override void Using(Tanks tank)
		{
			if (tank == _game.Tank)
			{
				foreach (EvilTank evilTank in _game.EvilTanks)
				{
					_game.TemporaryRemoveEvilTank ??= new List<EvilTank>();
					_game.TemporaryRemoveEvilTank.Add(evilTank);
				}
			}
			else
			{
				Task.Run(_game.DeadTimer);
			}
		}
	}
}
