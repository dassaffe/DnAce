using System;

namespace DnAce.Engine
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (DnAce game = new DnAce())
                game.Run();
        }
    }
#endif
}
