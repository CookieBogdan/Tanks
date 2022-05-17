// Decompiled with JetBrains decompiler
// Type: Tanks.v2.GameField
// Assembly: Tanks.v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A682BDB5-0425-49B3-A206-61CFFE0B3FF4
// Assembly location: C:\Users\yg\AppData\Local\Temp\Halysos\c2835aee50\Tanks.v2.exe

using System;
using System.Drawing;
using Tanks.v2.Enums;
using Tanks.v2.Models;

namespace Tanks.v2
{
	public class GameField
	{
		public int Width = 480;
		public int Height = 480;
		public LandscapeType[,] MapInBlocks = new LandscapeType[16, 16];
		public LandscapeType[,] MapInBigPixels = new LandscapeType[80, 80];
		public int[,] Strength = new int[16, 16]; // 0 -целый  3- почти сломан
		public const int STEP = 6;
		public const int BLOCK_SIZE = 5;
		public const int BLOCK_COUNT = 16;
		public const int MAP_SIZE = 80;

		public GameField()
		{

		}

		public void CreateMap(LandscapeType[,] field)
		{
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					PutBlock(i, j, 1, 1, field[i, j]);
				}
			}
		}

		public void PutBlock(int x, int y, int w, int h, LandscapeType type)
		{
			for (int width = x; width < x + w; ++width)
			{
				for (int height = y; height < y + h; ++height)
				{
					MapInBlocks[width, height] = type;
					if (type == LandscapeType.Brick)
							Strength[width, height] = 0;
					for (int width_pix = 0; width_pix < 5; ++width_pix)
					{
						for (int height_pix = 0; height_pix < 5; ++height_pix)
							MapInBigPixels[width * 5 + width_pix, height * 5 + height_pix] = type;
					}
				}
			}
		}
		public void Draw(Graphics g, int level)
		{
			if (level == 1)
			{
				Rectangle rect = new Rectangle()
				{
					X = 30,
					Y = 30,
					Width = this.Width,
					Height = this.Height
				};
				g.FillRectangle(Brushes.Black, rect);
			}
			for (int i = 0; i < 16; ++i)
			{
				for (int j = 0; j < 16; ++j)
				{
					if (this.GetDrawingLevel(i, j) == level)
					{
						Image image = this.GetImage(i, j);
						if (image != null)
							g.DrawImage(image, 30 + i * 5 * 6, 30 + j * 5 * 6);
					}
				}
			}
		}
		private Image GetImage(int i, int j)
		{
			switch (MapInBlocks[i, j])
			{
				case LandscapeType.Brick:
					return (Strength[i, j]) switch
					{
						0 => MyImage.imageBrick,
						1 => MyImage.imageBrokenBrick1,
						2 => MyImage.imageBrokenBrick2,
						3 => MyImage.imageBrokenBrick3,
						_ => (Image)null,
					};
				case LandscapeType.Water:
					return MyImage.imageWater;
				case LandscapeType.Explosion:
					return MyImage.imageExplosion;
				case LandscapeType.Concrete:
					return MyImage.imageConcrete;
				case LandscapeType.Forest:
					return MyImage.imageForest;
				case LandscapeType.Base:
					return MyImage.imageBase;
				default:
					return (Image) null;
			}
		}
		private int GetDrawingLevel(int i, int j)
		{
			switch (this.MapInBlocks[i, j])
			{
			case LandscapeType.None:
				return 0;
			case LandscapeType.Brick:
				return 1;
			case LandscapeType.Water:
				return 1;
			case LandscapeType.Concrete:
				return 1;
			case LandscapeType.Base:
				return 1;
			case LandscapeType.Forest:
				return 3;
			case LandscapeType.Explosion:
				return 3;
			default:
				throw new NotSupportedException(MapInBlocks[i, j].ToString());
			}
		}
	}
}
