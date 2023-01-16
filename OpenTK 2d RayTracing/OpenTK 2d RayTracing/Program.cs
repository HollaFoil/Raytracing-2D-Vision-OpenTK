using OpenTK.Windowing.Common;

namespace OpenTK_2d_RayTracing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using Game game = new Game(600, 600, "RayTracing Vision");
            game.Run();
        }
    }
}