using System;

namespace ParticleSystemV._3
{
    public static class GameStorage
    {
        public static Game1 game = new Game1();
    }

#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            GameStorage.game.Run();
        }
    }
#endif
}

