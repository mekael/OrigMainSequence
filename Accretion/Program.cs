namespace Accretion
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AccretionGame game = new AccretionGame())
            {
                game.Run();
            }
        }
    }
#endif
}