using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tanks.v2
{
	public partial class MainForm
	{
		partial void PauseScreen()
		{
			#region Body

			PictureBox all = new PictureBox()
			{
				Size = new Size(630, 540),
				Location = new Point(0, 0),
				BackColor = Color.FromArgb(200, 60, 60, 60),
			};
			Controls.Add(all);
			PictureBox panel = new PictureBox()
			{
				Size = new Size(400, 300),
				Location = new Point(110, 95),
				BackColor = Color.AntiqueWhite,
			};
			Button btnContinue = new Button()
			{
				Text = "Continue",
				Size = new Size(120, 30),
				Location = new Point(140, 105),
				BackColor = Color.LimeGreen,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 14),
			};
			btnContinue.FlatAppearance.BorderSize = 0;
			btnContinue.FlatAppearance.MouseOverBackColor = Color.ForestGreen;
			all.Controls.Add(panel);
			Button btnExit = new Button()
			{
				Text = "Exit",
				Size = new Size(120, 30),
				Location = new Point(140, 150),
				BackColor = Color.Red,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 14),
			};
			btnExit.FlatAppearance.BorderSize = 0;
			btnExit.FlatAppearance.MouseOverBackColor = Color.DarkRed;
			panel.Controls.Add(btnContinue);
			panel.Controls.Add(btnExit);
			btnContinue.Click += new EventHandler(btnClick_Continue);
			btnExit.Click += new EventHandler(btnClick_Exit);
			#endregion

			#region Click To Buttons

			void btnClick_Continue(object? sender, EventArgs e)
			{
				Controls.Remove(all);
				_game.Stop = false;
				_timer.Start();
				_game.stopWatch.Start();
				this.Focus();
				Refresh();
			}
			void btnClick_Exit(object? sender, EventArgs e)
			{
				_timer.Stop();
				_game = null;
				Refresh();
				StartScreen();
			}
			#endregion
		}
	}
}
