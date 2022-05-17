using System;
using System.Drawing;
using System.Windows.Forms;
using Tanks.v2.Models;

namespace Tanks.v2
{
	public partial class MainForm
	{
		partial void StartScreen()
		{
			ClearButtons();

			#region Body

			Button exit = new Button()
			{
				Text = "exit",
				Size = new Size(60, 25),
				Location = new Point(20, 20),
				BackColor = Color.DarkGray,
				ForeColor = Color.DarkSlateGray,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			exit.FlatAppearance.BorderSize = 0;
			exit.Click += new EventHandler(btnClick_Exit);

			Button levels = new Button()
			{
				Text = "LEVELS",
				Size = new Size(210, 55),
				Location = new Point(210, 180),
				BackColor = Color.Blue,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 28),
			};
			levels.Click += new EventHandler(btnClick_Maps);

			Button myMaps = new Button()
			{
				Text = "My levels",
				Size = new Size(140, 40),
				Location = new Point(210, 250),
				BackColor = Color.Orange,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 20),
			};
			myMaps.Click += new EventHandler(btnClick_Maps);

			Button createLevel = new Button()
			{
				Text = "+",
				Size = new Size(40, 40),
				Location = new Point(380, 250),
				BackColor = Color.Yellow,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 22),
			};
			createLevel.Click += new EventHandler(btnClick_CreateLevel);

			Button authors = new Button()
			{
				Text = "Authors",
				Size = new Size(150, 40),
				Location = new Point(270, 300),
				BackColor = Color.LimeGreen,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 20),
			};
			authors.Click += new EventHandler(btnClick_Authors);

			Button settings = new Button()
			{
				Size = new Size(40, 40),
				Location = new Point(210, 300),
				FlatStyle = (FlatStyle) ButtonBorderStyle.None,
				Image = (Image) (new Bitmap(MyImage.imageSettings, new Size(40, 40))),
			};
			settings.FlatAppearance.BorderSize = 0;
			settings.FlatAppearance.MouseOverBackColor = settings.BackColor;
			settings.FlatAppearance.MouseDownBackColor = settings.BackColor;
			settings.MouseHover += new EventHandler(btnHover_Settings);
			settings.MouseLeave += new EventHandler(btnLeave_Settings);
			settings.MouseDown += new MouseEventHandler(btnHover_Settings);
			settings.Click += new EventHandler(btnClick_Settings);

			Controls.AddRange(new Control[] {
				levels, myMaps, createLevel, authors, settings, exit
			});
			#endregion

			#region Click To Button

			void btnClick_Exit(object? sender, EventArgs e)
			{
				Application.Exit();
			}
			void btnHover_Settings(object? sender, EventArgs e)
			{
				Button button = (Button) sender;
				button.Image = (Image) (new Bitmap(MyImage.imageSettingsHover, new Size(40, 40)));
			}
			void btnLeave_Settings(object? sender, EventArgs e)
			{
				Button button = (Button) sender;
				button.Image = (Image) (new Bitmap(MyImage.imageSettings, new Size(40, 40)));
			}
			#endregion
		}
	}
}
