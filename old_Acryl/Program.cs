using System;

namespace Acryl
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new AcrylGame())
                game.Run();
        }
    }
}