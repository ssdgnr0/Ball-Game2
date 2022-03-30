using System;

namespace BallGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BallGameGame game = new BallGameGame())
            {
                game.Run();
            }
        }
    }
}

