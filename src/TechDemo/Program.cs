using System;
using VerticeEnginePort.Base;

namespace VerticeEnginePort.Base
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
            using (MetricRacerBaseGame game = new MetricRacerBaseGame())
            {
                game.Run();
            }
        }
    }
}

