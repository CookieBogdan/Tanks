// Decompiled with JetBrains decompiler
// Type: Tanks.v2.Models.Bullet
// Assembly: Tanks.v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A682BDB5-0425-49B3-A206-61CFFE0B3FF4
// Assembly location: C:\Downloads\Tanks.v2.exe\Tanks.v2.01.exe

using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using Tanks.v2.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace Tanks.v2.Models
{
	public class Bullet
	{
		public readonly int Size = 6;
		private readonly Game _game;
		public int X;
		public int Y;
		public OrientationType Orientation;

		public Bullet(Game game)
		{
			_game = game;
		}

		public void Draw(Graphics g, int offsetX, int offsetY)
		{
			Image image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.bullet.png"));
			if (Orientation == OrientationType.Up)
				image.RotateFlip(RotateFlipType.Rotate270FlipNone);
			if (Orientation == OrientationType.Down)
				image.RotateFlip(RotateFlipType.Rotate90FlipNone);
			if (Orientation == OrientationType.Left)
				image.RotateFlip(RotateFlipType.Rotate180FlipNone);
			g.DrawImage(image, offsetX + X, offsetY + Y);
		}

		public bool Move() // false - живёт
		{
			List<Tanks> allTanks = _game.TankIsLive ?
				_game.EvilTanks.Cast<Tanks>().Union(new List<Tanks> { _game.Tank }).ToList() :
				_game.EvilTanks.Cast<Tanks>().ToList();
			switch (Orientation)
			{
				case OrientationType.Up:
					if (this.Y - 6 < 0)
						return true;
					bool flag1 = false;
					int y1 = Y / 6;
					if (y1 > 3)
						y1 -= 3;
					int x1 = X / 6;
					switch (_game.Field.MapInBigPixels[x1, y1])
					{
						case LandscapeType.Brick:
						case LandscapeType.Base:
						case LandscapeType.Concrete:
							BreakBrick(x1, y1);
							flag1 = true;
							break;
					}
					for (int i = 0; i < 2; i++)
					{
						if (X / 6 + i < 80)
							if (Y / 6 - 3 > -1)
							{
								foreach (Tanks tank in allTanks)
								{
									if (tank.Position[X / 6 + i, Y / 6 - 3] == 1)
									{
										DeadOfTank(tank);
										flag1 = true;
										break;
									}
								}
								if (flag1)
									break;
							}
					}
					if (flag1)
						return true;
					Y -= 18; 
					break;

				case OrientationType.Down:
					if (_game.Field.Width < Y + 6 + Size)
						return true;
					bool flag2 = false;
					int y2 = Y / 6;
					if (y2 < 77)
						y2 += 3;
					int x2 = X / 6;
					switch (_game.Field.MapInBigPixels[x2, y2])
					{
						case LandscapeType.Brick:
						case LandscapeType.Base:
						case LandscapeType.Concrete:
							BreakBrick(x2, y2);
							flag2 = true;
							break;
					}
					for (int i = 0; i < 2; i++)
					{
						if (X / 6 + i < 80)
							if (Y / 6 + 3 < 80)
							{
								foreach (Tanks tank in allTanks)
								{
									if (tank.Position[X / 6 + i, Y / 6 + 3] == 1)
									{
										DeadOfTank(tank);
										flag2 = true;
										break;
									}
								}
								if (flag2)
									break;
							}
					}
					if (flag2)
						return true;
					Y += 18;
					break;

				case OrientationType.Left:
					if (X - 6 < 0)
						return true;
					bool flag3 = false;
					int y3 = Y / 6;
					int x3 = X / 6;
					if (x3 > 3)
						x3 -= 3;
					switch (_game.Field.MapInBigPixels[x3, y3])
					{
						case LandscapeType.Brick:
						case LandscapeType.Base:
						case LandscapeType.Concrete:
							BreakBrick(x3, y3);
							flag3 = true;
							break;
					}
					for (int i = 0; i < 2; i++)
					{
						if (X / 6 - 3 > -1)
						{
							if (Y / 6 + i < 80)
							{
								foreach (Tanks tank in allTanks)
								{
									if (tank.Position[X / 6 - 3, Y / 6 + i] == 1)
									{
										DeadOfTank(tank);
										flag3 = true;
										break;
									}
								}
								if (flag3)
									break;
							}
						}
					}
					if (flag3)
						return true;
					X -= 18;
					break;

				case OrientationType.Right:
					if (_game.Field.Width < X + 6 + Size)
						return true;
					bool flag4 = false;
					int y4 = Y / 6;
					int x4 = X / 6;
					if (x4 < 77)
						x4 += 3;
					switch (_game.Field.MapInBigPixels[x4, y4])
					{
						case LandscapeType.Brick:
						case LandscapeType.Base:
						case LandscapeType.Concrete:
							BreakBrick(x4, y4);
							flag4 = true;
							break;
					}
					for (int i = 0; i < 2; i++)
					{
						if (X / 6 + 4 < 80)
							if (Y / 6 + i < 80)
							{
								foreach (Tanks tank in allTanks)
								{
									if (tank.Position[X / 6 + 4, Y / 6 + i] == 1)
									{
										DeadOfTank(tank);
										flag4 = true;
										break;
									}
								}
								if(flag4)
									break;
							}
					}
					if (flag4)
						return true;
					X += 18;
					break;
				}
			return false;
		}
		void BreakBrick(int x, int y)
		{
			x /= 5;
			y /= 5;
			switch (_game.Field.MapInBlocks[x, y])
			{
				case LandscapeType.Brick:
					if (_game.Field.Strength[x, y] != 3)
						_game.Field.Strength[x, y]++;
					else
						_game.Field.PutBlock(x, y, 1, 1, LandscapeType.None);
					break;
				case LandscapeType.Base:
					_game.Field.PutBlock(x, y, 1, 1, LandscapeType.None);
					Task.Run(_game.GameOver);
					break;
				default:
					break;
			}
		}
		private void DeadOfTank(Tanks tank)
		{
			if (tank == _game.Tank)
			{
				if (_game.Tank.CountLive - 1 == 0)
					Task.Run(_game.DeadTimer);
				else
					_game.Tank.CountLive--;
			}
			else
			{
				if (tank.CountLive - 1 == 0)
				{
					_game.TemporaryRemoveEvilTank ??= new List<EvilTank>();
					_game.TemporaryRemoveEvilTank.Add((EvilTank)tank);
				}
				else 
					tank.CountLive--;
			}
		}
	}
}
