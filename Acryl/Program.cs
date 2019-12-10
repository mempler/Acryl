namespace Acryl
{
    public static class Program
    {
        public static void Main()
        {
            using var game = new AcrylGame();
            
            game.Run();
        }
    }
}