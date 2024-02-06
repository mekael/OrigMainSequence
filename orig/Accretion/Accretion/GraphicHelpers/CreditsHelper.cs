using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Accretion.GraphicHelpers
{
    class CreditsHelper
    {
        public static int increment = 0;

        private static readonly List<String> credits = new List<string>()
        {
            "~~~~~~~~~~~~~~~~~~~~~",
            "~design and programming~",
            "~~~~~~~~~~~~~~~~~~~~~",
            "David Abrahams",
            "",
            "",
            "~~~~~~~",
            "~music~",
            "~~~~~~~",
            "",
            "\"Night Owl\"",
            "\"Day Bird\"",
            "\"The Great\"",
            "\"My Luck\"",
            "\"Something Elated\"",
            "\"High School Snaps\"",
            "\"Our Ego feat. Different Visitor\"",
            "",
            "From the album \"Slam Funk\" by Broke For Free,",
            "http://brokeforfree.com/",
            "Licensed under the Creative Commons Attribution license",
            "",
            "~~~~~~~~~~~~~~~~~~~~~",
            "",
            "\"Divider\"",
            "\"Wonder Cycle\"",
            "\"Candle Power\"",
            "",
            "From the album \"Divider\" by Chris Zabriskie",
            "http://chriszabriskie.com/",
            "Licensed under the Creative Commons Attribution license",
            "",
            "~~~~~~~~~~~~~~~~~~~~~",
            "",
            "\"Scattered Knowledge\"",
            "",
            "From the album \"The Politics of Desire\" by Revolution Void",
            "http://www.revolutionvoid.com/",
            "Licensed under the Creative Commons Attribution license",
            "",
            "~~~~~~~~~~~~~~~~~~~~~",
            "",
            "Using Creative Commons Attribution or Sampling+ licensed sound effects from freesound:",
            "",
            "Reverse Snare 3 by VEXST",
            "http://www.freesound.org/people/VEXST/",
            "",
            "btn029 by junggle",
            "http://www.freesound.org/people/junggle/",
            "",
            "laser by THE_bizniss",
            "http://www.freesound.org/people/THE_bizniss/",
            "",
            "BEAM02 by NoiseCollector",
            "http://www.freesound.org/people/NoiseCollector/",
            "",
            "flash by edwin_p_manchester",
            "http://www.freesound.org/people/edwin_p_manchester/",
            "",
            "Button Click by KorgMS2000B",
            "http://www.freesound.org/people/KorgMS2000B/",
            "",
            "",
            "~~~~~~~~~~~~~~",
            "~special thanks~",
            "~~~~~~~~~~~~~~",
            "Thanks to my family - their vacation time became my development time.",
            "Thanks to FreeMusicArchive.org for introducing me to a bunch of talented artists."
        };

        public static void draw(SpriteFont font, SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            displayScrollingMessage(credits, font,  spriteBatch, screenWidth, screenHeight);
        }

        public static void displayScrollingMessage(List<string> messages, SpriteFont font, SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            int lines = 0;
            foreach (string message in messages)
            {
                List<string> splitMessage = MessageWriter.splitForWindowSize(message, font, screenWidth);
                for (int i = 0; i < splitMessage.Count; i++)
                {
                    Vector2 messageSize = font.MeasureString(splitMessage[i]);
                    spriteBatch.DrawString(font, splitMessage[i], new Vector2((screenWidth - messageSize.X) / 2, messageSize.Y * lines++ - increment + screenHeight), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }
        }
    }
}
