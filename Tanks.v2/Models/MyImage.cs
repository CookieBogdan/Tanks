using System;
using System.Drawing;
using System.Reflection;

namespace Tanks.v2.Models
{
	public static class MyImage
	{
		public static Image imageWater = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.water.png"));
		public static Image imageForest = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.forest.png"));
		public static Image imageBase = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.base.png"));
		public static Image imageBrick = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.brick.png"));
		public static Image imageConcrete = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.concrete.png"));
		public static Image imageNone = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.none.png"));
		public static Image imageSettings = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.settings.png"));
		public static Image imageSettingsHover = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.settings_hover.png"));
		public static Image imageExplosion = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.explosion.png"));
		public static Image imageBrokenBrick1 = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.brick1.png"));
		public static Image imageBrokenBrick2 = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.brick2.png"));
		public static Image imageBrokenBrick3 = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Tanks.v2.Images.brick3.png"));
	}
}
