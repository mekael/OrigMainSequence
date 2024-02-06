using Accretion.GameplayObjects;
using Accretion.GraphicHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    internal class HintPowerUp : PowerUp
    {
        private String hint;

        public HintPowerUp(String hint) : base(1)
        {
            this.hint = hint;
        }

        public override void use(PlayerObject player, ref Field field)
        {
        }

        public override char getFieldDisplayCharacter()
        {
            return 'H';
        }

        public override string getPowerName()
        {
            return "Hint";
        }

        public override SoundEffect getSoundEffect()
        {
            return null;
        }

        public override bool onUpdate(PlayerObject player, GameplayObjects.Field field)
        {
            return true;
        }

        public override void drawAttached(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, PlayerObject player, int windowWidth, int windowHeight)
        {
            MessageWriter.displayMessageTopRight(PlatformSpecificStrings.ZOOM_HINT, font, spriteBatch, windowWidth, windowHeight);
        }
    }
}