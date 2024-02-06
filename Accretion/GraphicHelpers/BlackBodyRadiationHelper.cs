using Microsoft.Xna.Framework;

namespace Accretion.GraphicHelpers
{
    internal class BlackBodyRadiationHelper
    {
        //masses and associated colors
        private static readonly List<KeyValuePair<int, Color>> criticalMasses = new List<KeyValuePair<int, Color>>
        {
            new KeyValuePair<int, Color>(0, Color.LightGray), //rocks
            new KeyValuePair<int, Color>(60, Color.DarkGray), //asteroids
            new KeyValuePair<int, Color>(300, Color.DarkSlateGray), //moons
            new KeyValuePair<int, Color>(1000, Color.ForestGreen), //planets
            new KeyValuePair<int, Color>(3000, Color.Purple), //gas giants
            new KeyValuePair<int, Color>(7000, Color.Red), //red suns
            new KeyValuePair<int, Color>(14000, Color.Yellow), //yellow suns
            new KeyValuePair<int, Color>(25000, Color.White), //white suns
            new KeyValuePair<int, Color>(30000, Color.LightSkyBlue), //blue suns
        };

        public static Color chooseColor(int mass)
        {
            for (int i = 1; i < criticalMasses.Count; i++)
            {
                if (mass < criticalMasses[i].Key)
                {
                    float factor = 1 - ((criticalMasses[i].Key - mass) / (float)(criticalMasses[i].Key - criticalMasses[i - 1].Key));
                    return Color.Lerp(criticalMasses[i - 1].Value, criticalMasses[i].Value, factor);
                }
            }

            return criticalMasses.Last().Value;
        }
    }
}