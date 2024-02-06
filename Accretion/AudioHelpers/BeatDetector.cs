using Microsoft.Xna.Framework.Media;

namespace Accretion.AudioHelpers
{
    internal class BeatDetector
    {
        private static double previousBassIntensity = 1;

        /// <summary>
        /// Returns a value from 0 to 1 representing the intensity of the bass frequencies of
        /// the currently playing music. The value jumps up instantly but falls slowly.
        /// </summary>
        /// <returns>a double from 0 to 1 representing the bass intensity</returns>
        public static double getDecayingBassPower()
        {
            double bassPower = getBassPower();

            //gradually reduce the power when the base isn't thumpin
            if (bassPower < previousBassIntensity)
            {
                bassPower = Math.Max(previousBassIntensity * 0.9, bassPower);
            }

            previousBassIntensity = bassPower;

            return bassPower;
        }

        /// <summary>
        /// Returns a value from 0 to 1 representing the intensity of the bass frequencies of
        /// the currently playing music.
        /// </summary>
        /// <returns>a double from 0 to 1 representing the bass intensity</returns>
        private static double getBassPower()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                VisualizationData vizData = new VisualizationData();
                MediaPlayer.GetVisualizationData(vizData);
                double musicPower = (vizData.Frequencies.Take(20).Average()); //takes the intensity of the lowest frequencies
                return musicPower;
            }
            else
            {
                return 1;
            }

        }
    }
}