using System;

namespace Virtex.vxGame.VerticesTechDemo
{
    /// <summary>
    /// The main program. Allows access to the game instance by the static property Game
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {	
            using (VerticesTechDemoGame game = new VerticesTechDemoGame())
            {
                game.Run();
            }
        }
    }
}

