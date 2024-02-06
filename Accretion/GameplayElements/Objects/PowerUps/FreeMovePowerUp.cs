using Accretion.GameplayObjects;
using Microsoft.Xna.Framework.Audio;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    internal class FreeMovePowerUp : PowerUp
    {
        public FreeMovePowerUp(int uses) : base(uses)
        {
        }

        protected static char displayChar;

        static FreeMovePowerUp()
        {
            char.TryParse("V", out displayChar);
        }

        public override char getFieldDisplayCharacter()
        {
            return displayChar;
        }

        public override string getPowerName()
        {
            return "Speed Boost";
        }

        public override SoundEffect getSoundEffect()
        {
            return null;
        }

        public override bool onUpdate(PlayerObject player, Field field)
        {
            return false;
        }

        public override void use(PlayerObject player, ref GameplayObjects.Field field)
        {
            player.addVelocity(player.getVelocity() / 2);
            base.use(player, ref field);
        }
    }
}