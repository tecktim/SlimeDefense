using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TowerDefenseNew.Grid;
using TowerDefenseNew.Structure;

namespace TowerDefenseNew.Structure 
{
	internal class Program 
	{
		private static void Main(string[] _)
		{
			var window = MainWindow.Create();
			var model = new Model(GridLoader.CreateGrid());
			var view = new View();
			var control = new Control(model, view);
			var keyboard = window.KeyboardState;

			window.MouseDown += args => control.Click(window.MousePosition.X, window.Size.Y - 1 - window.MousePosition.Y);
			window.UpdateFrame += args =>
			{
				control.Update((float)args.Time, window.KeyboardState);
				model.Update((float)args.Time);
			}; // call update once each frame
			window.Resize += args => view.Resize(args.Width, args.Height); // on window resize inform view
			window.RenderFrame += _ => view.Draw(model); // first draw the model
			window.RenderFrame += _ => window.SwapBuffers(); // buffer swap needed for double buffering
			window.Run();
		}
	}
}