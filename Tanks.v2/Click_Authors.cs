using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Tanks.v2
{
	public partial class MainForm
	{
		partial void btnClick_Authors(object? sender, EventArgs e)
		{
			ClearButtons();

			#region Body

			Label label = new Label()
			{
				Text = "",
				Size = new Size(330, 100),
				Location = new Point(150, 170),
				Font = new Font("Bauhaus 93", 16),
				TextAlign = ContentAlignment.MiddleCenter,
			};
			Label labelLink = new Label()
			{
				Text = "Open source project:\nhttps://bitbucket.org/silverstormlab/tanks.v2",
				Size = new Size(330, 50),
				Location = new Point(150, 490),
				Font = new Font("Bauhaus 93", 12),
				TextAlign = ContentAlignment.MiddleCenter,
			};
			Button btnBack = new Button()
			{
				Text = "back",
				Size = new Size(60, 25),
				Location = new Point(550, 20),
				BackColor = Color.DarkGray,
				ForeColor = Color.DarkSlateGray,
				FlatStyle = (FlatStyle)ButtonBorderStyle.None,
				Font = new Font("Bauhaus 93", 12),
				Visible = false,
			};
			btnBack.FlatAppearance.BorderSize = 0;
			Controls.AddRange(new Control[] {
				btnBack, label, labelLink,
			});
			PrintAuthors(label, btnBack);
			#endregion

			void PrintAuthors(Control label, Button button)
			{
				Timer _t = new Timer();
				_t.Interval = 100;
				_t.Tick += new EventHandler(_t_Tick);
				_t.Start();
				Task.Run(() => StopTimer(_t));
			}

			void _t_Tick(object sender, EventArgs e)
			{
				string text = "Creators:\nBogdan Dubinin,\nYuri Gavrishov.";
				int length = label.Text.Length;
				if (length == text.Length)
				{
					btnBack.Visible = true;
					btnBack.Click += new EventHandler(btnClick_Back);
					return;
				}
				label.Text += text[length];
			}

			void StopTimer(Timer timer)
			{
				Thread.Sleep(5000);
				timer.Stop();
			}
		}
	}
}
