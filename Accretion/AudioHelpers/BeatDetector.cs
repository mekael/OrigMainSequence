using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace Accretion.AudioHelpers
{
    class BeatDetector
    {
        private static double previousBassIntensity = 1;

#if WINDOWS_PHONE
        private static Random random = new Random();
#endif

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
#if WINDOWS || XBOX
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
#elif WINDOWS_PHONE
            //windows phone doesn't support the music visualizer api. try putting some random junk here to make stuff still pulsate.
            float fakePower = (DateTime.Now.Millisecond % 750) / 750f;
            if (fakePower < 0.3f)
            { 
                fakePower = 0.45f; 
            }
            else if (fakePower > 0.65f)
            {
                fakePower = 0.65f;
            }

            return fakePower;
#endif
        }
    }
}
