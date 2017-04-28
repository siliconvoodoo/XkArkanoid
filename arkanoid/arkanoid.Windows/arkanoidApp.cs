using SiliconStudio.Xenko.Engine;

namespace arkanoid
{
    class arkanoidApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
