using System;

namespace CookTorrance_RobDavis_411Final
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Final())
                game.Run();
        }
    }
}