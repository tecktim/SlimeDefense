using System.Diagnostics;

namespace TowerDefenseNew.Structure
{
    public class FPScounter
    {
        public void NextFrame()
        {
            counter++;
            if (_time.ElapsedMilliseconds >= 1000)
            {
                Value = counter; 
                counter = 0;
                _time.Restart();
            }
        }

        public int Value { get; private set; }

        private readonly Stopwatch _time = Stopwatch.StartNew();
        private int counter = 0;
    }
}
