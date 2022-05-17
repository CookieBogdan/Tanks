using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tanks.v2
{
	public partial class MainForm
	{
		private delegate void ButtonMethod(object? sender, EventArgs e);
		partial void Confirmation(ButtonMethod buttonMethod)
		{
			Control[] oldControls = Controls.Cast<Control>().ToArray();
			List<Control> childControls = new List<Control>();
			int number = -1;
			for (int i = 0; i < Controls.Count; i++)
			{
				if (Controls[i].Controls.Count > 0)
				{
					childControls = Controls[i].Controls.Cast<Control>().ToList();
					number = i;
				}
			}
			ClearButtons();

			#region Body

			Panel panel = new Panel()
			{
				Size = new Size(200, 100),
				Location = new Point(215, 180),
				BackColor = Color.DarkGray,
				AutoScroll = false,
			};
			panel.BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(panel);

			Label label = new Label()
			{
				Text = "are you sure?",
				Size = new Size(150, 30),
				Location = new Point(25, 10),
				Font = new Font("Bauhaus 93", 12),
				ForeColor = Color.Black,
				TextAlign = ContentAlignment.MiddleCenter,
			};

			Button btnYes = new Button()
			{
				Text = "Yes",
				Size = new Size(60, 25),
				Location = new Point(35, 50),
				BackColor = Color.Transparent,
				ForeColor = Color.GreenYellow,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnYes.FlatAppearance.BorderSize = 1;
			btnYes.FlatAppearance.MouseOverBackColor = Color.White;
			btnYes.FlatAppearance.MouseDownBackColor = Color.White;
			btnYes.Click += new EventHandler(btnClick_Yes);

			Button btnNo = new Button()
			{
				Text = "No",
				Size = new Size(60, 25),
				Location = new Point(105, 50),
				BackColor = Color.Transparent,
				ForeColor = Color.Red,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnNo.FlatAppearance.BorderSize = 1;
			btnNo.FlatAppearance.MouseOverBackColor = Color.White;
			btnNo.FlatAppearance.MouseDownBackColor = Color.White;
			btnNo.Click += new EventHandler(btnClick_No);
			panel.Controls.AddRange(new Control[]{
				label,  btnNo, btnYes
			});
			#endregion

			#region Click To Buttons

			void btnClick_Yes(object? sender, EventArgs e)
			{
				Finish();
				buttonMethod(btnYes, e);
				Refresh();
			}
			void btnClick_No(object? sender, EventArgs e)
			{
				Finish();
				buttonMethod(btnNo, e);
				Refresh();
			}
			void Finish()
			{
				ClearButtons();
				if (number != -1)
					oldControls[number].Controls.AddRange(childControls.ToArray());
				Controls.AddRange(oldControls);
			}
			#endregion
		}
	}
}
