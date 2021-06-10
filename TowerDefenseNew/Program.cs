using OpenTK.Windowing.GraphicsLibraryFramework;
using TowerDefenseNew.Grid;

namespace TowerDefenseNew.Structure
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            var window = MainWindow.Create();
            var view = new View(window);
            var model = new Model(GridLoader.CreateGrid());
            var control = new Control(model, view);
            var keyboard = window.KeyboardState;
            var mB = new MouseButton();

            window.MouseMove += args => control.PlacePath(window.MousePosition.X, window.Size.Y - 1 - window.MousePosition.Y, mB);
            window.MouseMove += args => control.ShowTowerSample(window.MousePosition.X, window.Size.Y - 1 - window.MousePosition.Y, keyboard);
            window.MouseDown += args => control.Click(window.MousePosition.X, window.Size.Y - 1 - window.MousePosition.Y, keyboard);
            window.UpdateFrame += args =>
            {
                control.Update((float)args.Time, window.KeyboardState);
                model.Update((float)args.Time);
            }; // call update once each frame
            window.Resize += args => view.Resize(window.Bounds.Size.X, window.Bounds.Size.Y); // on window resize inform view
            window.RenderFrame += _ => view.Draw(model); // first draw the model
            window.RenderFrame += _ => window.SwapBuffers(); // buffer swap needed for double buffering
            window.Run();
        }
    }
}