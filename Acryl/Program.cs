using System;

namespace Acryl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new AcrylGame())
                game.Run();
        }
    }
}