using System;

namespace FallingWorld
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (FallingWorld game = new FallingWorld())
            {
                game.Run();
            }
        }
    }
#endif
}

