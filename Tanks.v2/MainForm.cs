// Decompiled with JetBrains decompiler
// Type: Tanks.v2.MainForm
// Assembly: Tanks.v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A682BDB5-0425-49B3-A206-61CFFE0B3FF4
// Assembly location: C:\Downloads\Tanks.v2.exe\Tanks.v2.01.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tanks.v2.Models;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace Tanks.v2
{
	public partial class MainForm : Form
	{
		//== fields ==
		private Game _game;
		private Timer _timer;
		private IContainer components; //?
		private Random random = new Random();
		private bool needKeyDown = false;
		private Button selectedButton;

		//== constructor ==
		public MainForm()
		{
			InitializeComponent();
			typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic
														  | System.Reflection.BindingFlags.Instance
														  | System.Reflection.BindingFlags.SetProperty)
				?.SetValue(this, true, null);
			//SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			//_game = new Game();
			Width = Width - ClientRectangle.Width + 30 + 480 + 10 + 100 + 10;//630
			Height = Height - ClientRectangle.Height + 30 + 480 + 30;//540
			Paint += new PaintEventHandler(MainForm_Paint);
			KeyDown += new KeyEventHandler(MainForm_KeyDown);
			Resize += new EventHandler(LeaveEventGame);
			_timer = new Timer();
			_timer.Interval = 100;
			_timer.Tick += new EventHandler(_timer_Tick);
			//check all pressKey if(there are controls), but (up, down, left, right) - change focus on controls
			//KeyPreview = true; 
			StartScreen();
		}

		//== methods ==
		private void _timer_Tick(object sender, EventArgs e)
		{
			if (_game.IsWaitBack)
			{
				_timer.Stop();
				_game = null;
				Refresh();
				StartScreen();
				Refresh();
				return;
			}

			if (_game.Bullets.Count > 0)
			{
				List<Bullet> delBullets = (List<Bullet>)null;
				foreach (Bullet bullet in _game.Bullets)
				{
					if (bullet.Move())
					{
						if (delBullets == null)
							delBullets = new List<Bullet>();
						delBullets.Add(bullet);
					}
				}
				if (delBullets != null)
				{
					foreach (Bullet bullet in delBullets)
						_game.Bullets.Remove(bullet);
				}
			}

			if(_game.EvilTanks.Count > 0)
			{
				foreach (EvilTank evilTank in _game.EvilTanks)
				{
					List<Bullet> delBullets = (List<Bullet>)null;
					foreach (Bullet bullet in evilTank.EvilBullets)
					{
						if (bullet.Move())
						{
							if (delBullets == null)
								delBullets = new List<Bullet>();
							delBullets.Add(bullet);
						}
					}
					if (delBullets != null)
					{
						foreach (Bullet bullet in delBullets)
							evilTank.EvilBullets.Remove(bullet);
					}

					if (!evilTank.IsShooting)
					{
						for(int i = 0; i < (evilTank.fastSpeed ? 2 : 1); i++)
							evilTank.Move();
					}
					else
					{
						evilTank.Shoot(evilTank.EvilBullets, evilTank.bullets);
						evilTank.IsShooting = false;
					}
				}
			}

			if (_game.TemporaryAddEvilTank != null)
			{
				foreach (EvilTank evilTank in _game.TemporaryAddEvilTank)
					_game.EvilTanks.Add(evilTank);
				_game.TemporaryAddEvilTank = (List<EvilTank>)null;
			}
			if (_game.TemporaryRemoveEvilTank != null)
			{
				foreach (EvilTank evilTank in _game.TemporaryRemoveEvilTank)
				{
					_game.EvilTanks.Remove(evilTank);
					int x = (int)Math.Round(Convert.ToDouble(evilTank.XPos) / 5, MidpointRounding.AwayFromZero);
					int y = (int)Math.Round(Convert.ToDouble(evilTank.YPos) / 5, MidpointRounding.AwayFromZero);
					Task.Run(() => _game.ExplosionTimer(x, y));
				}
				_game.TemporaryRemoveEvilTank = (List<EvilTank>)null;
			}

			if (_game.Bonuses.Count > 0)
			{
				Bonus delBonus = null;
				foreach (Bonus bonus in _game.Bonuses)
				{
					Models.Tanks takingTank = bonus.IsTake();
					if (takingTank != null)
					{
						bonus.Using(takingTank);
						delBonus = bonus;
					}
				}

				_game.Bonuses.Remove(delBonus);
			}

			Invalidate();
		}
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		 {
			if (_game == null)
				return;
			if (!_game.TankIsLive)
				return;
			if (_game.Stop)
			{
				if (e.KeyCode == Keys.Escape)
				{
					ClearButtons();
					_game.Stop = false;
					_game.stopWatch.Start();
					_timer.Start();
					Focus();
					Refresh();
				}
				return;
			}
			if (e.KeyCode == Settings.Default.Shoot)
				_game.Tank.Shoot(_game.Bullets, _game.Tank.bullets);
			else
			{
				bool pressedKeyMove = false;
				OrientationType wantedOrientation = OrientationType.Left;
				if (e.KeyCode == Settings.Default.Left)
				{
					pressedKeyMove = true;
					wantedOrientation = OrientationType.Left;
				}
				else
				{
					if (e.KeyCode == Settings.Default.Right)
					{
						pressedKeyMove = true;
						wantedOrientation = OrientationType.Right;
					}
					else
					{
						if (e.KeyCode == Settings.Default.Up)
						{
							pressedKeyMove = true;
							wantedOrientation = OrientationType.Up;
						}
						else
						{
							if (e.KeyCode == Settings.Default.Down)
							{
								pressedKeyMove = true;
								wantedOrientation = OrientationType.Down;
							}
						}
					}
				}
				if(pressedKeyMove)
				{
					for (int i = 0; i < (_game.Tank.fastSpeed ? 2 : 1); i++)
					{
						if (_game.Tank.CanMoveToMove(wantedOrientation))
						{
							_game.Tank.Move(wantedOrientation);
						}
						else
						{
							_game.Tank.Orientation = wantedOrientation;
						}
					}
				}
				else
				{
					if (e.KeyCode == Keys.Escape)
					{
						_game.Stop = true;
						_timer.Stop();
						_game.stopWatch.Stop();
						PauseScreen();
						//_game.IsPlaying = false;
						//_game.TankIsLive = false;
						//_game.Tank = null;
						//_game.IsWaitBack = true;
					}
				}
			}
			Invalidate();
		}
		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			if (_game == null) return;
			Rectangle rect = new Rectangle()
			{
				X = 30 + _game.Field.Width + 10,
				Y = 40,
				Width = 100,
				Height = 200
			};
			e.Graphics.FillRectangle(Brushes.LightGray, rect);
			_game.Draw(e.Graphics);
		}

		partial void PauseScreen();
		partial void StartScreen();
		partial void btnClick_Maps(object? sender, EventArgs e);
		partial void btnClick_CreateLevel(object? sender, EventArgs e);
		partial void btnClick_Authors(object? sender, EventArgs e);
		partial void btnClick_Settings(object? sender, EventArgs e);

		partial void Confirmation(ButtonMethod buttonMethod);

		private void btnClick_Back(object? sender, EventArgs e)
		{
			if(needKeyDown)
				return;
			StartScreen();
		}
		private void ClearButtons()
		{
			List<Control> delButtons = Controls.Cast<Control>().ToList();
			foreach (Control control in delButtons)
			{
				if (control.Controls.Count > 0)
				{
					List<Control> delControls = control.Controls.Cast<Control>().ToList();
					foreach (Control c in delControls)
						control.Controls.Remove(c);
				}
				Controls.Remove(control);
			}
			Refresh();
		}

		private void LeaveEventGame(object? sender, EventArgs e)
		{
			if(_game == null) return;
			if (_game.Stop) return;
			_game.Stop = true;
			_timer.Stop();
			_game.stopWatch.Stop();
			PauseScreen();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			SuspendLayout();
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(759, 671);
			Font = new Font("Tahoma", 8.25f);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			Name = nameof(MainForm);
			StartPosition = FormStartPosition.CenterScreen;
			Text = "ТАНЧИКИ";
			ResumeLayout(false);
		}
	}
}