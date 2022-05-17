using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tanks.v2.Enums;

namespace Tanks.v2.Models
{
	public class EvilTank : Tanks
	{
		public bool IsShooting { get; set; }
		public List<Bullet> EvilBullets;
		private bool canMove; // 0 -> must choose orientation; 1 -> mustn't
		private Dictionary<string, OrientationType> dictionary = new Dictionary<string, OrientationType>();
		public EvilTank(Game game)
		{
			_game = game;
			//IsShooting = true;
			EvilBullets = new List<Bullet>();
			TankImage = "Tanks.v2.Images.eviltank.png";
			CountLive = 1;
			bullets = 1;
		}
		public void Draw(Graphics g, int offsetX, int offsetY)
		{
			base.Draw(g, offsetX, offsetY, TankImage);
		}

		public void Move()
		{
			List<OrientationType> variantsOrientation = new List<OrientationType>();
			Random r = new Random();
			while (true)
			{
				if(variantsOrientation.Count > 3)
					break;
				if (NeedShoot(out OrientationType orOR, XPos, YPos))
				{
					Orientation = orOR;
					IsShooting = true;
					dictionary = new Dictionary<string, OrientationType>();
				}
				else
				{
					if (!canMove)
					{
						while (true)
						{
							int i = r.Next(100);
							if(i < 15)
								Orientation = OrientationType.Up;
							else
							{
								if (i < 27 + (16 - XPos / 5) * 2)
									Orientation = OrientationType.Right;
								else
								{
									Orientation = i < 69 ?
										OrientationType.Left : OrientationType.Down;
								}
							}
							if(!variantsOrientation.Contains(Orientation))
								break;
						}

						canMove = true;
					}
					else
					{
						RecursionСheckCellAround(XPos, YPos, X, Y, new Dictionary<string, OrientationType>());
						if (dictionary.Count != 0)
							canMove = true;
					}

					if (dictionary.Count != 0)
					{
						if (!dictionary.ContainsKey($"{XPos}-{YPos}"))
						{
							dictionary = new Dictionary<string, OrientationType>();
							canMove = false;
						}
						else
						{
							if (CanMoveToMove(dictionary[$"{XPos}-{YPos}"]))
							{
								Move(dictionary[$"{XPos}-{YPos}"]);
							}
						}

						break;
					}
					if(CanMoveToMove(Orientation))
						Move(Orientation);
					else
					{
						canMove = false;
						variantsOrientation.Add(Orientation);
						continue;
					}
				}
				break;
			}
		}

		private void RecursionСheckCellAround(int xPos, int yPos, int x, int y, Dictionary<string, OrientationType> dict, int step = 0, List<int[]> whereWas = null)
		{
			if(step>6)
				return;

			whereWas ??= new List<int[]>();

			List<int[]> newList = new List<int[]>();
			foreach (int[] arr in whereWas)
			{
				newList.Add(arr);
			}
			newList.Add(new[] {xPos, yPos});

			if (NeedShoot(out OrientationType orientation, xPos, yPos))
			{
				if (dictionary.Count == 0)
					dictionary = dict;
				else
				{
					if (dict.Count < dictionary.Count)
						dictionary = dict;
				}
				return;
			}

			if (CanMoveToMove(OrientationType.Up, x, y, xPos, yPos))
			{
				Dictionary<string, OrientationType> newDict = new Dictionary<string, OrientationType>();
				foreach (KeyValuePair<string, OrientationType> pair in dict)
				{
					if (pair.Key == $"{xPos}-{yPos}")
						return;
					newDict.Add(pair.Key, pair.Value);
				}
				newDict.Add($"{xPos}-{yPos}", OrientationType.Up);
				RecursionСheckCellAround(xPos, yPos - 1, x, y - 6, newDict, step+1, newList);
			}
			if (CanMoveToMove(OrientationType.Down, x, y, xPos, yPos))
			{
				Dictionary<string, OrientationType> newDict = new Dictionary<string, OrientationType>();
				foreach (KeyValuePair<string, OrientationType> pair in dict)
				{
					if (pair.Key == $"{xPos}-{yPos}")
						return;
					newDict.Add(pair.Key, pair.Value);
				}
				newDict.Add($"{xPos}-{yPos}", OrientationType.Down);
				RecursionСheckCellAround(xPos, yPos + 1, x, y+6, newDict,  step+1, newList);
			}
			if (CanMoveToMove(OrientationType.Left, x, y, xPos, yPos))
			{
				Dictionary<string, OrientationType> newDict = new Dictionary<string, OrientationType>();
				foreach (KeyValuePair<string, OrientationType> pair in dict)
				{
					if (pair.Key == $"{xPos}-{yPos}")
						return;
					newDict.Add(pair.Key, pair.Value);
				}
				newDict.Add($"{xPos}-{yPos}", OrientationType.Left);
				RecursionСheckCellAround(xPos - 1, yPos, x - 6, y, newDict, step+1, newList);
			}
			if (CanMoveToMove(OrientationType.Right, x, y, xPos, yPos))
			{
				Dictionary<string, OrientationType> newDict = new Dictionary<string, OrientationType>();
				foreach (KeyValuePair<string, OrientationType> pair in dict)
				{
					if (pair.Key == $"{xPos}-{yPos}")
						return;
					newDict.Add(pair.Key, pair.Value);
				}
				newDict.Add($"{xPos}-{yPos}", OrientationType.Right);
				RecursionСheckCellAround(xPos + 1, yPos, x + 6, y, newDict, step+1 , newList);
			}
		}

		private bool NeedShoot(out OrientationType or, int xPos, int yPos)
		{
			int countBricks = 0;
			for (int i = yPos; i < 80; i+=5)
			{
				if(_game.Field.MapInBigPixels[xPos + 2, i] == LandscapeType.Concrete)
					break;
				if (_game.Field.MapInBigPixels[xPos + 2, i] == LandscapeType.Brick)
					countBricks++;
				if (_game.Field.MapInBigPixels[xPos + 2, i] == LandscapeType.Base)
				{
					if (countBricks < 2)
					{
						or = OrientationType.Down;
						return true;
					}
				}
				if (!_game.TankIsLive) continue;
				if (_game.Tank.Position[xPos + 2, i] != 1) continue;
				if (countBricks > 0) continue;
				or = OrientationType.Down;
				return true;
			}
			countBricks = 0;
			for (int i = yPos; i > 0; i -= 5)
			{
				if (_game.Field.MapInBigPixels[xPos + 2, i] == LandscapeType.Concrete)
					break;
				if (_game.Field.MapInBigPixels[xPos + 2, i] == LandscapeType.Brick)
					countBricks++;
				if (_game.Field.MapInBigPixels[xPos + 2, i] == LandscapeType.Base)
				{
					if (countBricks < 2)
					{
						or = OrientationType.Up;
						return true;
					}
				}
				if (!_game.TankIsLive) continue;
				if (_game.Tank.Position[xPos + 2, i] != 1) continue;
				if (countBricks > 0) continue;
				or = OrientationType.Up;
				return true;
			}
			countBricks = 0;
			for (int i = xPos; i < 80; i += 5)
			{
				if (_game.Field.MapInBigPixels[i, yPos + 2] == LandscapeType.Concrete)
					break;
				if (_game.Field.MapInBigPixels[i, yPos + 2] == LandscapeType.Brick)
					countBricks++;
				if (_game.Field.MapInBigPixels[i, yPos + 2] == LandscapeType.Base)
				{
					if (countBricks < 2)
					{
						or = OrientationType.Right;
						return true;
					}
				}
				if (!_game.TankIsLive) continue;
				if (_game.Tank.Position[i, yPos + 2] != 1) continue;
				if (countBricks > 0) continue;
				or = OrientationType.Right;
				return true;
			}
			countBricks = 0;
			for (int i = xPos; i > 0; i -= 5)
			{
				if (_game.Field.MapInBigPixels[i, yPos + 2] == LandscapeType.Concrete)
					break;
				if (_game.Field.MapInBigPixels[i, yPos + 2] == LandscapeType.Brick)
					countBricks++;
				if (_game.Field.MapInBigPixels[i, yPos + 2] == LandscapeType.Base)
				{
					if (countBricks < 2)
					{
						or = OrientationType.Left;
						return true;
					}
				}
				if (!_game.TankIsLive) continue;
				if (_game.Tank.Position[i, yPos + 2] != 1) continue;
				if(countBricks > 0) continue;
				or = OrientationType.Left;
				return true;
			}
			or = OrientationType.Left;
			return false;
		}
	}
}