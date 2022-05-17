using System;
using System.Drawing;

namespace Tanks.v2.Models
{
	public class Tank : Tanks
	{
		public Tank(Game game)
		{
			_game = game;
			TankImage = "Tanks.v2.Images.tank.png";
			CountLive = 2;
			bullets = 2;
		}
		public void Draw(Graphics g, int offsetX, int offsetY)
		{
			Draw(g, offsetX, offsetY, TankImage);
		}
	}
}