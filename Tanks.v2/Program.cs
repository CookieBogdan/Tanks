// Decompiled with JetBrains decompiler
// Type: Tanks.v2.Program
// Assembly: Tanks.v2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A682BDB5-0425-49B3-A206-61CFFE0B3FF4
// Assembly location: C:\Downloads\Tanks.v2.exe\Tanks.v2.01.exe

using System;
using System.Windows.Forms;

namespace Tanks.v2
{
	internal static class Program
	{
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run((Form) new MainForm());
		}
	}
}
