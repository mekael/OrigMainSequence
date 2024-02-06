using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Text.RegularExpressions;

namespace Accretion.GraphicHelpers
{
    internal class MessageWriter
    {
        private const int MAX_STRING_CACHE_SIZE = 1000;
        private static Dictionary<string, List<String>> cache = new Dictionary<string, List<String>>();

#if XBOX
        private const double MAX_USABLE_SCREEN_FRACTION = 0.80;
#else
        private const double MAX_USABLE_SCREEN_FRACTION = 0.95;

#endif
        private const double BORDER_SCREEN_FRACTION = (1 - MAX_USABLE_SCREEN_FRACTION) / 2;

        public static void displayMessageTopRight(string message, SpriteFont font, SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            List<string> splitMessage = splitForWindowSize(message, font, screenWidth);
            for (int i = 0; i < splitMessage.Count; i++)
            {
                Vector2 messageSize = font.MeasureString(splitMessage[i]);

                spriteBatch.DrawString(font, splitMessage[i], new Vector2((int)(screenWidth * (1 - BORDER_SCREEN_FRACTION)), (int)(screenHeight * BORDER_SCREEN_FRACTION)), Color.White, 0, new Vector2(messageSize.X, 0), 1, SpriteEffects.None, 0);
            }
        }

        public static void displayMessageCentered(string message, SpriteFont font, SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            List<string> splitMessage = splitForWindowSize(message, font, screenWidth);
            for (int i = 0; i < splitMessage.Count; i++)
            {
                Vector2 messageSize = font.MeasureString(splitMessage[i]);
                spriteBatch.DrawString(font, splitMessage[i], new Vector2(screenWidth, screenHeight) / 2 + messageSize * Vector2.UnitY * i - (messageSize * Vector2.UnitY * splitMessage.Count / 2), Color.White, 0, messageSize / 2, 1, SpriteEffects.None, 0);
            }
        }

        public static List<string> splitForWindowSize(string message, SpriteFont font, int screenWidth)
        {
            if (!cache.ContainsKey(message))
            {
                if (cache.Count > MAX_STRING_CACHE_SIZE)
                {
                    cache.Clear();
                }

                List<string> splitMessage = new List<string>();
                string[] originalLines = message.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                int usableScreenWidth = (int)(screenWidth * MAX_USABLE_SCREEN_FRACTION);

                foreach (string originalLine in originalLines)
                {
                    Vector2 messageSize = font.MeasureString(originalLine);
                    if (messageSize.X > usableScreenWidth)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < originalLine.Length; i++)
                        {
                            stringBuilder.Append(originalLine[i]);
                            messageSize = font.MeasureString(stringBuilder);
                            if (messageSize.X > usableScreenWidth)
                            {
                                //check if there is any whitespace we can split
                                Regex regex = new Regex(@"\s");
                                if (regex.IsMatch(stringBuilder.ToString()))
                                {
                                    int j = stringBuilder.Length - 1;
                                    do
                                    {
                                        stringBuilder.Remove(j--, 1);
                                        i--;
                                    }
                                    while (!Char.IsWhiteSpace(stringBuilder[j]));

                                    splitMessage.Add(stringBuilder.ToString());

                                    stringBuilder = new StringBuilder();
                                }
                                else
                                {
                                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                                    i--;
                                    splitMessage.Add(stringBuilder.ToString());
                                    stringBuilder = new StringBuilder();
                                }
                            }
                        }

                        splitMessage.Add(stringBuilder.ToString());
                    }
                    else
                    {
                        splitMessage.Add(originalLine);
                    }
                }

                cache.Add(message, splitMessage);
            }

            return cache[message];
        }
    }
}