using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tanks.v2.Enums;

namespace Tanks.v2.Models
{
	public abstract class Tanks
	{
		public int Size = 30;
		public Game _game;
		public int X;
		public int Y;
		public int XPos; // 80
		public int YPos; // 80
		public int[,] Position = new int[80, 80]; // 1 - есть танк, 0 - нет
		public OrientationType Orientation;
		public int CountLive { get; set; }
		public bool fastSpeed = false;
		protected string TankImage;
		public int bullets;

		//public Tanks Name = new Tanks();

		public virtual void Draw(Graphics g, int offsetX, int offsetY, string tImage)
		{
			Image image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(tImage));
			if (Orientation == OrientationType.Up)
				image.RotateFlip(RotateFlipType.Rotate270FlipNone);
			if (Orientation == OrientationType.Down)
				image.RotateFlip(RotateFlipType.Rotate90FlipNone);
			if (Orientation == OrientationType.Left)
				image.RotateFlip(RotateFlipType.Rotate180FlipNone);
			g.DrawImage(image, offsetX + X, offsetY + Y);
		}

		public bool CanMoveToMove(OrientationType orientation, int myX = -1, int myY = -1, int xPos = -1, int yPos = -1)
		{
			if (myX == -1)
			{
				myX = X;
				myY = Y;
				xPos = XPos;
				yPos = YPos;
			}
			List<Tanks> allTanks = _game.TankIsLive ?
				_game.EvilTanks.Cast<Tanks>().Union(new List<Tanks> { _game.Tank }).ToList() :
				_game.EvilTanks.Cast<Tanks>().ToList();

			switch (orientation)
			{
				#region Up
				case OrientationType.Up:
					if (myY - 6 < 0)
						return false;
					for (int i = 0; i < 5; ++i)
					{
						int x = myX / 6;
						int y = myY / 6 - 1;
						switch (_game.Field.MapInBigPixels[x + i, y])
						{
							case LandscapeType.Brick:
							case LandscapeType.Base:
							case LandscapeType.Water:
							case LandscapeType.Concrete:
								return false;
						}
					}
					for (int i = 0; i < 5; i++)
					{
						if (allTanks.Any(tank => tank.Position[xPos + i, yPos - 1] == 1))
						{
							return false;
						}
					}
					return true;
				#endregion

				#region Down
				case OrientationType.Down:
					if (_game.Field.Width < myY + 6 + Size)
						return false;
					for (int i = 0; i < 5; ++i)
					{
						int x = myX / 6;
						int y = myY / 6 + 5;
						switch (_game.Field.MapInBigPixels[x + i, y])
						{
							case LandscapeType.Brick:
							case LandscapeType.Base:
							case LandscapeType.Water:
							case LandscapeType.Concrete:
								return false;
						}
					}
					for (int i = 0; i < 5; i++)
					{
						if (yPos + 6 >= 80) continue;
						if (allTanks.Any(tank => tank.Position[xPos + i,yPos + 5] == 1))
						{
							return false;
						}
					}
					return true;
				#endregion

				#region Left
				case OrientationType.Left:
					if (myX - 6 < 0)
						return false;
					for (int i = 0; i < 5; ++i)
					{
						int x = myX / 6 - 1;
						int y = myY / 6;
						switch (_game.Field.MapInBigPixels[x, y + i])
						{
							case LandscapeType.Brick:
							case LandscapeType.Base:
							case LandscapeType.Water:
							case LandscapeType.Concrete:
								return false;
						}
					}
					for (int i = 0; i < 5; i++)
					{
						if (allTanks.Any(tank => tank.Position[xPos - 1, yPos + i] == 1))
						{
							return false;
						}
					}
					return true;
				#endregion

				#region Right
				case OrientationType.Right:
					if (_game.Field.Width < myX + 6 + Size)
						return false;
					for (int i = 0; i < 5; ++i)
					{
						switch (_game.Field.MapInBigPixels[myX / 6 + 5, myY / 6 + i])
						{
							case LandscapeType.Brick:
							case LandscapeType.Base:
							case LandscapeType.Water:
							case LandscapeType.Concrete:
								return false;
						}
					}
					for (int i = 0; i < 5; i++)
					{
						if (allTanks.Any(tank => tank.Position[xPos + 5, yPos + i] == 1))
						{
							return false;
						}
					}
					return true;
				#endregion

				default:
					throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
			}
		}
		public void Move(OrientationType orientation)
		{
			if (Orientation != orientation)
				Orientation = orientation;
			else
			{
				List<Tanks> allTanks = _game.TankIsLive ? 
					_game.EvilTanks.Cast<Tanks>().Union(new List<Tanks> {_game.Tank}).ToList() : 
					_game.EvilTanks.Cast<Tanks>().ToList();

				switch (orientation)
				{
					case OrientationType.Up:
						Y -= 6;
						YPos -= 1;
						for (int i = 0; i < 5; i++)
						{
							Position[XPos + i, YPos] = 1;
							Position[XPos + i, YPos + 5] = 0;
						}
						break;

					case OrientationType.Down:
						Y += 6;
						YPos += 1;
						for (int i = 0; i < 5; i++)
						{
							Position[XPos + i, YPos + 4] = 1;
							Position[XPos + i, YPos - 1] = 0;
						}
						break;

					case OrientationType.Left:
						X -= 6;
						XPos -= 1;
						for (int i = 0; i < 5; i++)
						{
							Position[XPos, YPos + i] = 1;
							Position[XPos + 5, YPos + i] = 0;
						}
						break;

					case OrientationType.Right:
						X += 6;
						XPos += 1;
						for (int i = 0; i < 5; i++)
						{
							Position[XPos + 4, YPos + i] = 1;
							Position[XPos - 1, YPos + i] = 0;
						}
						break;
				}
			}
		}
		public void Shoot(List<Bullet> lstBullets, int maxBullets)
		{
			if (lstBullets.Count >= maxBullets)
				return;
			int num1 = X / 6;
			int num2 = Y / 6;
			int num3;
			int num4;
			switch (Orientation)
			{
				case OrientationType.Up:
					num3 = num1 + 2;
					num4 = num2 - 1;
					break;
				case OrientationType.Down:
					num3 = num1 + 2;
					num4 = num2 + 5;
					break;
				case OrientationType.Left:
					num3 = num1 - 1;
					num4 = num2 + 2;
					break;
				case OrientationType.Right:
					num3 = num1 + 5;
					num4 = num2 + 2;
					break;
				default:
					throw new NotSupportedException(Orientation.ToString());
			}
			Task.Run(() => _game.PlaySound(Sounds.Shoot));
			lstBullets.Add(new Bullet(_game)
			{
				X = num3 * 6,
				Y = num4 * 6,
				Orientation = Orientation,
			});
		}
	}
}
