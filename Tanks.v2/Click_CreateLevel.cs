using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Tanks.v2.Enums;
using Tanks.v2.Models;

namespace Tanks.v2
{
	public partial class MainForm
	{
		partial void btnClick_CreateLevel(object? sender, EventArgs e)
		{
			ClearButtons();

			#region Body

			const int proportionsImage = 40;
			LandscapeType selectedType = LandscapeType.None;

			Button[,] newField = new Button[16, 16];
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					newField[i, j] = new Button()
					{
						Text = $"",
						Size = new Size(30, 30),
						Location = new Point(30 + i * 30, 30 + j * 30),
						FlatStyle = (FlatStyle)ButtonBorderStyle.None,
						BackColor = Color.Black,
					};
					Controls.Add(newField[i, j]);
					if ((j == 15 && (i == 7 || i == 8 || i == 6)) ||
						(j == 14 && (i == 7 || i == 8 || i == 6)))
					{
						newField[i, j].Image = (j == 15 && i == 7) ? MyImage.imageBase : MyImage.imageBrick;
					}
					else
					{
						if (j != 0 && j != 15)
						{
							newField[i, j].Click += new EventHandler(btn_Click);
						}
					}
					Controls.Add(newField[i, j]);
				}
			}

			Button btnNone = new Button()
			{
				Location = new Point(550, 90),
				BackColor = Color.Black,
				TabStop = false,
				Tag = 0.ToString(),
				FlatStyle = FlatStyle.Flat,
			}; //0
			Button btnBrick = new Button()
			{
				Location = new Point(550, 160),
				Image = MyImage.imageBrick,
				Tag = 1.ToString(),
				TabStop = false,
				FlatStyle = FlatStyle.Flat,
			}; //1
			Button btnConcrete = new Button()
			{
				Location = new Point(550, 230),
				Image = MyImage.imageConcrete,
				Tag = 2.ToString(),
				TabStop = false,
				FlatStyle = FlatStyle.Flat,
			}; //2
			Button btnWater = new Button()
			{
				Location = new Point(550, 300),
				Image = MyImage.imageWater,
				TabStop = false,
				Tag = 3.ToString(),
				FlatStyle = FlatStyle.Flat,
			}; //3
			Button btnForest = new Button()
			{
				Location = new Point(550, 370),
				Image = MyImage.imageForest,
				TabStop = false,
				Tag = 4.ToString(),
				FlatStyle = FlatStyle.Flat,
			}; //4

			Button btnBack = new Button()
			{
				Text = "back",
				Size = new Size(60, 25),
				Location = new Point(550, 20),
				BackColor = Color.DarkGray,
				ForeColor = Color.DarkSlateGray,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnBack.FlatAppearance.BorderSize = 0;
			Controls.Add(btnBack);
			btnBack.Click += new EventHandler(btnClick_Back);
			Button btnLoad = new Button()
			{
				Text = "load",
				Size = new Size(60, 30),
				Location = new Point(540, 460),
				BackColor = Color.CornflowerBlue,
				ForeColor = Color.Black,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 14),
			};
			btnLoad.FlatAppearance.BorderSize = 0;
			Controls.Add(btnLoad);
			btnLoad.Click += new EventHandler(btn_Click_Load);
			Button btnSave = new Button()
			{
				Text = "save",
				Size = new Size(60, 30),
				Location = new Point(540, 500),
				BackColor = Color.OrangeRed,
				ForeColor = Color.Black,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 14),
			};
			btnSave.FlatAppearance.BorderSize = 0;
			Controls.Add(btnSave);
			TextBox nameLevel = new TextBox()
			{
				Size = new Size(130, 30),
				Location = new Point(400, 520),
				Text = "",
				BackColor = Color.LightGoldenrodYellow,
				ForeColor = Color.OrangeRed,
				Font = new Font("Bauhaus 93", 12),
			};
			nameLevel.BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(nameLevel);
			btnSave.Click += new EventHandler(btn_Click_Save);

			List<Button> selectingButtons = new List<Button>()
			{
				btnNone, btnBrick, btnConcrete, btnWater, btnForest,
			};
			foreach (Button button in selectingButtons)
			{
				button.Size = new Size(40, 40);
				if (button.Image != null)
					button.Image = (Image)(new Bitmap(button.Image, new Size(proportionsImage, proportionsImage)));
				button.FlatAppearance.BorderColor = Color.Red;
				button.FlatAppearance.BorderSize = 0;
				Controls.Add(button);
				button.Click += new EventHandler(btn_Click_Select);
			}
			btnNone.FlatAppearance.BorderSize = 3;
			#endregion

			#region Click To Button

			void btn_Click(object? sender, EventArgs e)
			{
				Button button = (Button)sender;
				switch (selectedType)
				{
					case LandscapeType.Brick:
						button.Image = MyImage.imageBrick;
						break;
					case LandscapeType.Concrete:
						button.Image = MyImage.imageConcrete;
						break;
					case LandscapeType.Water:
						button.Image = MyImage.imageWater;
						break;
					case LandscapeType.Forest:
						button.Image = MyImage.imageForest;
						break;
					default:
						button.Image = null;
						button.BackColor = Color.Black;
						break;
				}
			}
			void btn_Click_Select(object? sender, EventArgs e)
			{
				Button button = (Button)sender;
				foreach (Button btn in selectingButtons)
				{
					btn.FlatAppearance.BorderSize = 0;
				}
				button.FlatAppearance.BorderSize = 3;
				if (button.Tag == 0.ToString())
					selectedType = LandscapeType.None;
				else
				{
					if (button.Tag == 1.ToString())
						selectedType = LandscapeType.Brick;
					else
					{
						if (button.Tag == 2.ToString())
							selectedType = LandscapeType.Concrete;
						else
						{
							if (button.Tag == 4.ToString())
								selectedType = LandscapeType.Forest;
							else
								selectedType = LandscapeType.Water;
						}
					}
				}
			}
			void btn_Click_Load(object? sender, EventArgs e)
			{
				ClearButtons();

				_game = new Game(IntArrToLandscapeArr(), null);
				_timer.Start();
			}

			void btn_Click_Save(object? sender, EventArgs e)
			{
				Button send = (Button) sender;
				if (send.Text != "Yes")
				{
					if (send.Text != "No")
						Confirmation(btn_Click_Save);
				}
				else
				{
					string name = nameLevel.Text;
					string path = @$"..\..\..\maps\{name}.map";
					LandscapeTypeCollection collection = new LandscapeTypeCollection();
					for (int i = 0; i < 16; i++)
					{
						for (int j = 0; j < 16; j++)
						{
							if (newField[i, j].Image == null)
								collection.Collection.Add(LandscapeType.None);
							else
							{
								if (newField[i, j].Image == MyImage.imageBrick)
									collection.Collection.Add(LandscapeType.Brick);
								else
								{
									if (newField[i, j].Image == MyImage.imageConcrete)
										collection.Collection.Add(LandscapeType.Concrete);
									else
									{
										if (newField[i, j].Image == MyImage.imageForest)
											collection.Collection.Add(LandscapeType.Forest);
										else
										{
											if (newField[i, j].Image == MyImage.imageBase)
												collection.Collection.Add(LandscapeType.Base);
											else
												collection.Collection.Add(LandscapeType.Water);
										}
									}
								}
							}
						}
					}
					SerializeXML(path, collection);
					btnClick_Maps(sender, e);
				}
			}

			#endregion

			void SerializeXML(string path, LandscapeTypeCollection collection)
			{
				XmlSerializer xml = new XmlSerializer(typeof(LandscapeTypeCollection));
				using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
				{
					xml.Serialize(fs, collection);
				}
			}
			LandscapeType[,] IntArrToLandscapeArr()
			{
				LandscapeType[,] field = new LandscapeType[16, 16];

				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						if (newField[i, j].Image == null)
							field[i, j] = LandscapeType.None;
						else
						{
							if (newField[i, j].Image == MyImage.imageBrick)
								field[i, j] = LandscapeType.Brick;
							else
							{
								if (newField[i, j].Image == MyImage.imageConcrete)
									field[i, j] = LandscapeType.Concrete;
								else
								{
									if (newField[i, j].Image == MyImage.imageForest)
										field[i, j] = LandscapeType.Forest;
									else
									{
										if (newField[i, j].Image == MyImage.imageBase)
											field[i, j] = LandscapeType.Base;
										else
											field[i, j] = LandscapeType.Water;
									}
								}
							}
						}
					}
				}

				return field;
			}
		}
	}

	[Serializable]
	public class LandscapeTypeCollection
	{
		public List<LandscapeType> Collection { get; set; } = new List<LandscapeType>();
	}
}
