namespace Accretion.Levels.LevelGenerationHelpers
{
    internal class MassDistributions
    {
        private static Random random = new Random();

        public static double hockeyStick()
        {
            return hockeyStick(random.NextDouble(), 10);
        }

        public static double hockeyStick(double max)
        {
            return hockeyStick(random.NextDouble(), max);
        }

        public static double hockeyStick(double input, double max)
        {
            if (input < 0 || input > 1)
            {
                throw new ArgumentOutOfRangeException("Input must be 0-1 inlcusive");
            }

            double magicNumber = 1 + 1 / max;

            return input / Math.Abs(input - magicNumber);
        }
    }
}