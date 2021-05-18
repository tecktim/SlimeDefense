﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;

namespace TowerDefenseNew.Structure
{
	public class MainWindow
	{
		public static FPScounter fpsCounter; // frames per second counter
		public static GameWindow window;
        public static GameWindow Create()
		{
			// window with immediate mode rendering enabled
			window = new GameWindow(GameWindowSettings.Default, new NativeWindowSettings { Profile = ContextProfile.Compatability });
			fpsCounter = new FPScounter();
            window.RenderFrame += Window_RenderFrame;
			window.VSync = VSyncMode.Off;
			window.Size = new Vector2i(1280, 720);
			window.CenterWindow();
			// set window to halve monitor size
			if (Monitors.TryGetMonitorInfo(0, out var info))
			{
				window.Size = new Vector2i(info.HorizontalResolution, info.VerticalResolution) / 2;
			}
			//for easy screen capture
			//window.WindowState = WindowState.Normal;
			//window.WindowBorder = WindowBorder.Hidden;
			//window.Bounds = new Rectangle(200, 20, 1024, 1024);
			window.KeyDown += args =>
			{
				if (Keys.Escape == args.Key)
				{
					window.Close();
				}
			};
			window.WindowBorder = WindowBorder.Hidden;
			window.WindowState = WindowState.Maximized;
			return window;
		}

        private static void Window_RenderFrame(FrameEventArgs obj)
        {
			fpsCounter.NextFrame();
			window.Title = $"TowerDefenseNew {fpsCounter.Value} FPS";
        }
    }
}
