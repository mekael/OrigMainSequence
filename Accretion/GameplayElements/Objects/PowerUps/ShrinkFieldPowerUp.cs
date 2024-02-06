using Accretion.GameplayObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    internal class ShrinkFieldPowerUp : PowerUp
    {
        protected static SoundEffect soundEffect;
        private AbilitySentinel abilitySentinel;
        private readonly TimeSpan duration = TimeSpan.FromSeconds(8);

        protected static char displayChar;

        static ShrinkFieldPowerUp()
        {
            char.TryParse("R", out displayChar);
            soundEffect = AccretionGame.staticContent.Load<SoundEffect>("6142__noisecollector__beam02");
        }

        public override char getFieldDisplayCharacter()
        {
            return displayChar;
        }

        public override void use(PlayerObject player, ref Field field)
        {
            if (this.uses > 0 && !this.abilitySentinel.isInUse())
            {
                base.use(player, ref field);
                abilitySentinel.fireAbility();
            }
        }

        public override bool onUpdate(PlayerObject player, Field field)
        {
            if (this.abilitySentinel.isInUse() && player != null)
            {
                double rampUpFactor = Math.Min(1 - (DateTime.UtcNow - abilitySentinel.getLastUsedTimeUTC()).TotalMilliseconds / 2000, 1);
                double rampDownFactor = Math.Min((DateTime.UtcNow - this.duration - abilitySentinel.getLastUsedTimeUTC() + TimeSpan.FromSeconds(2)).TotalMilliseconds / 2000, 1);
                double timeRampFactor = Math.Max(rampUpFactor, rampDownFactor);

                //todo: parallelize
                foreach (SpaceObject spaceObject in field.getSpaceObjects())
                {
                    if (spaceObject != null && !spaceObject.pendingRemoval && !spaceObject.hasGravity && spaceObject != player)
                    {
                        double distanceRampFactor = Math.Min(player.distanceFrom(spaceObject) / 30000 + 0.1, 1);
                        spaceObject.setSecretMassModifier(Math.Max(timeRampFactor, distanceRampFactor));
                    }
                }
            }
            else if (this.abilitySentinel.numberOfExpirationsSinceLastCheck() > 0 && player != null)
            {
                foreach (SpaceObject spaceObject in field.getSpaceObjects())
                {
                    if (spaceObject != null && !spaceObject.pendingRemoval && !spaceObject.hasGravity && spaceObject != player)
                    {
                        spaceObject.setSecretMassModifier(1);
                    }
                }
            }

            return this.abilitySentinel.isInUse();
        }

        public ShrinkFieldPowerUp(int uses)
            : base(uses)
        {
            this.abilitySentinel = new AbilitySentinel(this.duration);
        }

        public ShrinkFieldPowerUp(int uses, Vector2 location, Vector2 velocity)
            : base(uses, location, velocity)
        {
            this.abilitySentinel = new AbilitySentinel(this.duration);
        }

        public override string getPowerName()
        {
            return "Mass Reduction Field";
        }

        public override SoundEffect getSoundEffect()
        {
            return soundEffect;
        }
    }
}