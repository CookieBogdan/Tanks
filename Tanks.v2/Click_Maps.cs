using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tanks.v2.Enums;
using Tanks.v2.Models;

namespace Tanks.v2
{
	public partial class MainForm
	{
		partial void btnClick_Maps(object? sender, EventArgs e)
		{
			ClearButtons();

			#region Body

			Button button = (Button) sender;
			bool myMaps = button.Text == "LEVELS" ? false : true;

			string selectedMap = null;
			List<Button> allMyMaps = new List<Button>();
			string path =
				myMaps ? @"..\..\..\maps" : @"..\..\..\basemaps";
			List<string> linkDirectories = Directory.GetFiles(path).ToList();
			Button btnBack = new Button()
			{
				Text = "back",
				Size = new Size(60, 25),
				Location = new Point(550, 20),
				BackColor = Color.DarkGray,
				ForeColor = Color.DarkSlateGray,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnBack.FlatAppearance.BorderSize = 0;
			btnBack.Click += new EventHandler(btnClick_Back);
			Controls.Add(btnBack);
			Button btnShow = new Button()
			{
				Text = "this will selected map",
				Size = new Size(250, 250),
				Location = new Point(360, 65),
				BackColor = Color.White,
				ForeColor = Color.DarkGray,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnShow.FlatAppearance.BorderSize = 0;
			btnShow.FlatAppearance.MouseOverBackColor = Color.White;
			btnShow.FlatAppearance.MouseDownBackColor = Color.White;
			Controls.Add(btnShow);

			Button btnPlay = new Button()
			{
				Text = "Play",
				Size = new Size(90, 30),
				Location = new Point(385, 330),
				BackColor = Color.HotPink,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 14),
			};
			btnPlay.FlatAppearance.BorderSize = 0;
			btnPlay.Click += new EventHandler(btn_Click_Play);
			Controls.Add(btnPlay);
			if (myMaps)
			{
				Button btnDelete = new Button()
				{
					Text = "Delete",
					Size = new Size(90, 30),
					Location = new Point(485, 330),
					BackColor = Color.DarkRed,
					ForeColor = Color.White,
					FlatStyle = (FlatStyle) ButtonBorderStyle.None,
					Font = new Font("Bauhaus 93", 14),
				};
				btnDelete.FlatAppearance.BorderSize = 0;
				btnDelete.Click += new EventHandler(btn_Click_Delete);
				Controls.Add(btnDelete);
			}

			Panel panel = new Panel()
			{
				Size = new Size(300, 400),
				Location = new Point(30, 65),
				AutoScroll = true,
			};
			Controls.Add(panel);

			Label labelRecord = new Label()
			{
				Text = "-",
				Size = new Size(190, 30),
				Location = new Point(390, 360),
				ForeColor = Color.Black,
				TextAlign = ContentAlignment.MiddleCenter,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			Controls.Add(labelRecord);

			for (int i = 0; i < linkDirectories.Count; i++)
			{
				int x = 10 * ((i / 4 > 0) ? i - 4 * (i / 4) : i) + ((i / 4 > 0) ? i - 4 * (i / 4) : i) * 60;
				int y = (i / 4 * 50);
				Button btnMap = new Button()
				{
					Text = linkDirectories[i][(linkDirectories[i].IndexOf("maps") + 5)..].Split(".").ToList()[0],
					Size = new Size(60, 40),
					Location = new Point(x, y),
					BackColor = Color.FromArgb(random.Next(200, 215), random.Next(200, 215), random.Next(200, 215)),
					ForeColor = Color.Black,
					FlatStyle = (FlatStyle) ButtonBorderStyle.None,
					Font = new Font("Times New Roman", 10),
				};
				panel.Controls.Add(btnMap);
				btnMap.FlatAppearance.BorderSize = 0;
				btnMap.Click += new EventHandler(btn_Click_SelectMyMap);
				allMyMaps.Add(btnMap);
			}
			#endregion

			#region Click To Button
			void btn_Click_Play(object? sender, EventArgs e)
			{
				if (selectedMap == null)
					return;
				List<LandscapeType> collection = DeserializeXML(@$"..\..\..\{selectedMap}").Collection;
				LandscapeType[,] field = new LandscapeType[16, 16];
				int k = 0;
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						field[i, j] = collection[k];
						k++;
					}
				}
				ClearButtons();
				_game = new Game(field, selectedMap);
				_timer.Start();
			}
			void btn_Click_Delete(object? sender, EventArgs e)
			{
				if (selectedMap == null)
					return;
				Button send = (Button)sender;
				if (send.Text != "Yes")
				{
					if (send.Text != "No")
						Confirmation(btn_Click_Delete);
				}
				else
				{
					Button delButton = null;
					foreach (Button button in allMyMaps)
					{
						if (button.Text == selectedMap[(selectedMap.IndexOf("maps") + 5)..].Split(".").ToList()[0])
							delButton = button;
					}

					File.Delete(@$"..\..\..\{selectedMap}");

					string recordPath = @"..\..\..\records.txt";
					List<string> records = File.ReadAllLines(recordPath).ToList();
					int number = -1;
					for (int i = 0; i < records.Count; i++)
						if (records[i].Contains(selectedMap[(selectedMap.IndexOf("Tanks.v2") + 8)..]))
							number = i;
					if (number >= 0)
					{
						records.RemoveAt(number);
						File.WriteAllLines(recordPath, records);
					}

					btnClick_Maps(sender, e);
				}
			}
			void btn_Click_SelectMyMap(object? sender, EventArgs e)
			{
				Button button = (Button)sender;
				if (selectedMap != null)
				{
					if (button.Text == selectedMap.Substring(selectedMap.IndexOf("maps") + 5).Split(".").ToList()[0])
					{
						selectedMap = null;
						button.BackColor =
							Color.FromArgb(random.Next(200, 215), random.Next(200, 215), random.Next(200, 215));
						btnShow.Image = null;
						btnShow.Text = "this will selected map";
						labelRecord.Text = "-";
						return;
					}
				}
				selectedMap = myMaps ? @$"maps\{button.Text}.map" :
									   @$"basemaps\{button.Text}.map";
				string[] records = File.ReadAllLines(@$"..\..\..\records.txt");
				int number = -1;
				for (int i = 0; i < records.Length; i++)
					if (records[i].Contains(selectedMap))
						number = i;
				if (number != -1)
				{
					string minSec = records[number].Substring(records[number].IndexOf(".map") + 5);
					labelRecord.Text = $"record time {minSec.Split(" ").ToList()[0]}:{minSec.Split(" ").ToList()[1]}";
				}
				else
				{
					labelRecord.Text = "record time 0:0";
				}
				foreach (Button btn in allMyMaps)
				{
					if (btn.BackColor == Color.OrangeRed)
					{
						btn.BackColor =
							Color.FromArgb(random.Next(200, 215), random.Next(200, 215), random.Next(200, 215));
					}
				}
				button.BackColor = Color.OrangeRed;
				Show();
			}
			void Show()
			{
				int imageWidth = 30;
				int imageHeight = 30;
				int matrixSize = 16;
				Image newImage = new Bitmap(imageWidth * matrixSize, imageHeight * matrixSize);

				List<Bitmap> images = new List<Bitmap>();
				List <LandscapeType> collection = DeserializeXML(@$"..\..\..\{selectedMap}").Collection;
				foreach (LandscapeType coll in collection)
				{
					switch (coll)
					{
						case LandscapeType.None:
							images.Add((Bitmap)MyImage.imageNone);
							break;
						case LandscapeType.Concrete:
							images.Add((Bitmap)MyImage.imageConcrete);
							break;
						case LandscapeType.Brick:
							images.Add((Bitmap)MyImage.imageBrick);
							break;
						case LandscapeType.Base:
							images.Add((Bitmap)MyImage.imageBase);
							break;
						case LandscapeType.Forest:
							images.Add((Bitmap)MyImage.imageForest);
							break;
						default:
							images.Add((Bitmap)MyImage.imageWater);
							break;
					}
				}
				Graphics g = Graphics.FromImage(newImage);
				for (int y = 0, count = 0; y < imageHeight * matrixSize; y += 30)
				{
					for (int x = 0; x < imageWidth * matrixSize; x += 30)
					{
						g.DrawImage(images[count], y, x);
						count++;
					}
				}
				newImage = ResizeImage(newImage, 250, 250);
				Bitmap ResizeImage(Image image, int width, int height)
				{
					var destRect = new Rectangle(0, 0, width, height);
					var destImage = new Bitmap(width, height);

					destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

					using (var graphics = Graphics.FromImage(destImage))
					{
						graphics.CompositingMode = CompositingMode.SourceCopy;
						graphics.CompositingQuality = CompositingQuality.HighQuality;
						graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
						graphics.SmoothingMode = SmoothingMode.HighQuality;
						graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

						using (var wrapMode = new ImageAttributes())
						{
							wrapMode.SetWrapMode(WrapMode.TileFlipXY);
							graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
						}
					}

					return destImage;
				}

				//newImage.Save(@"D:\for_job\C#\tanks.v2\Tanks.v2\Images\show.png", ImageFormat.Png);
				btnShow.Image = newImage;
				btnShow.Text = "";
			}

			LandscapeTypeCollection DeserializeXML(string path)
			{
				XmlSerializer xml = new XmlSerializer(typeof(LandscapeTypeCollection));
				using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
				{
					return (LandscapeTypeCollection) xml.Deserialize(fs);
				}
			}
			#endregion
		}
	}
}
