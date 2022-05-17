using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Tanks.v2
{
	public partial class MainForm
	{
		partial void btnClick_Settings(object? sender, EventArgs e)
		{
			ClearButtons();

			#region Body

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
			btnBack.Click += new EventHandler(btnClick_Back);

			Label labelW = new Label()
			{
				Location = new Point(180, 130),
				Text = "forward\nback\nto the left\nto the right\nfire",
				Size = new Size(200, 300),
				ForeColor = Color.Black,
				Font = new Font("Bauhaus 93", 18),
			};
			Button btnW = new Button()
			{
				Text = $"{Settings.Default.Up}",
				Location = new Point(400, 130),
				Tag = 1.ToString(),
			}; //1
			Button btnS = new Button()
			{
				Text = $"{Settings.Default.Down}",
				Location = new Point(400, 158),
				Tag = 2.ToString(),
			}; //2
			Button btnA = new Button()
			{
				Text = $"{Settings.Default.Left}",
				Location = new Point(400, 186),
				Tag = 3.ToString(),
			}; //3
			Button btnD = new Button()
			{
				Text = $"{Settings.Default.Right}",
				Location = new Point(400, 214),
				Tag = 4.ToString(),
			}; //4
			Button btnSpc = new Button()
			{
				Text = $"{Settings.Default.Shoot}",
				Location = new Point(400, 242),
				Tag = 0.ToString(),
			}; //0
			Button[] controlButtons = new Button[] { btnW, btnS, btnD, btnA, btnSpc };

			foreach (Button button in controlButtons)
			{
				button.Height = 27;
				button.AutoSize = true;
				button.ForeColor = Color.Black;
				button.Font = new Font("Bauhaus 93", 14);
				button.FlatStyle = (FlatStyle)ButtonBorderStyle.None;
				button.FlatAppearance.BorderSize = 1;
				button.Click += new EventHandler(btnClick_SelectedButton);
				button.PreviewKeyDown += new PreviewKeyDownEventHandler(PreviewKeyDown);
				button.KeyDown += new KeyEventHandler(KeyDown);
				button.ContextMenuStrip = new ContextMenuStrip();
			}

			Button btnClearRecords = new Button()
			{
				Text = "Clear all records",
				Size = new Size(200, 30),
				Location = new Point(215, 400),
				BackColor = Color.DarkSalmon,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnClearRecords.FlatAppearance.BorderSize = 0;
			btnClearRecords.Click += new EventHandler(btnClick_ClearRecords);
			Button btnReturnDefault = new Button()
			{
				Text = "Set default settings",
				Size = new Size(200, 30),
				Location = new Point(215, 360),
				BackColor = Color.DarkSalmon,
				ForeColor = Color.White,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
			};
			btnReturnDefault.FlatAppearance.BorderSize = 0;
			btnReturnDefault.Click += new EventHandler(btnClick_ReternDefault);

			Controls.AddRange(new Control[] {
				btnClearRecords, btnBack, btnReturnDefault, labelW, btnS, btnA, btnD, btnW, btnSpc,
			});
			#endregion

			#region Click To Buttons

			void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
			{
				switch (e.KeyCode)
				{
					case Keys.Down:
					case Keys.Tab:
					case Keys.Left:
					case Keys.Right:
					case Keys.Up:
					case Keys.Space:
						e.IsInputKey = true;
						break;
				}
			}
			void btnClick_SelectedButton(object? sender, EventArgs e)
			{
				if (controlButtons.Any(b => b.Text == "?"))
					return;
				Button button = (Button)sender;

				button.Text = "?";
				selectedButton = button;
				needKeyDown = true;
			}
			void KeyDown(object sender, KeyEventArgs e)
			{
				Button button = (Button)sender;

				if (button.ContextMenuStrip != null)
				{
					if (Settings.Default.Right == e.KeyCode ||
						Settings.Default.Left == e.KeyCode ||
						Settings.Default.Down == e.KeyCode ||
						Settings.Default.Up == e.KeyCode ||
						Settings.Default.Shoot == e.KeyCode ||
						e.KeyCode == Keys.Escape)
					{
						(button.BackColor, button.ForeColor) = (button.ForeColor, button.BackColor);
						Refresh();
						Thread.Sleep(50);
						(button.BackColor, button.ForeColor) = (button.ForeColor, button.BackColor);
						return;
					}
					if (selectedButton == null) return;
					selectedButton.Text = e.KeyCode.ToString();
					switch (selectedButton.Tag)
					{
						case "1":
							Settings.Default.Up = e.KeyCode;
							break;
						case "2":
							Settings.Default.Down = e.KeyCode;
							break;
						case "3":
							Settings.Default.Left = e.KeyCode;
							break;
						case "4":
							Settings.Default.Right = e.KeyCode;
							break;
						//0
						default:
							Settings.Default.Shoot = e.KeyCode;
							break;
					}
					Settings.Default.Save();
					selectedButton = null;
					needKeyDown = false;
					button.AutoSize = false;
					button.AutoSize = true;
				}
			}
			void btnClick_ClearRecords(object? sender, EventArgs e)
			{
				if (needKeyDown)
					return; ;
				Button button = (Button)sender;
				if (button.Text != "Yes")
				{
					if (button.Text != "No")
						Confirmation(btnClick_ClearRecords);
				}
				else
				{
					File.WriteAllLines(@"..\..\..\records.txt", new List<string>());
				}
			}
			void btnClick_ReternDefault(object? sender, EventArgs e)
			{
				if (needKeyDown)
					return; ;
				Button button = (Button)sender;
				if (button.Text != "Yes")
				{
					if (button.Text != "No")
						Confirmation(btnClick_ReternDefault);
				}
				else
				{
					Settings.Default.Shoot = Keys.Space;
					Settings.Default.Up = Keys.Up;
					Settings.Default.Down = Keys.Down;
					Settings.Default.Left = Keys.Left;
					Settings.Default.Right = Keys.Right;
					btnW.Text = $"{Settings.Default.Up}";
					btnS.Text = $"{Settings.Default.Down}";
					btnA.Text = $"{Settings.Default.Left}";
					btnD.Text = $"{Settings.Default.Right}";
					btnSpc.Text = $"{Settings.Default.Shoot}";
					Settings.Default.Save();
				}
			}
			#endregion
		}
	}
}
