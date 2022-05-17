using System;
using System.IO;
using System.Media;

namespace Tanks.v2.Models
{
	class Sound
	{
		private SoundPlayer player;
		public Sound()
		{
			player = new SoundPlayer();
		}
		public void Play(Sounds sound)
		{
			string filename = GetSoundFileName(sound);
			player.SoundLocation = Path.Combine(Environment.CurrentDirectory, $"Sounds\\{filename}");
			player.Play();
		}

		private string GetSoundFileName(Sounds sound)
		{
			switch (sound)
			{
				case Sounds.Explosion:
					return "explosion.wav";

				case Sounds.Shoot:
					return "shoot.wav";

				case Sounds.Take:
					return "take.wav";

				case Sounds.Spawn:
					return "spawn.wav";

				case Sounds.GameOver:
					return "gameover.wav";

				default:
					return string.Empty;
			}
		}
	}

	public enum Sounds
	{
		Explosion,
		Shoot,
		Take,
		Spawn,
		GameOver,
	}
}
