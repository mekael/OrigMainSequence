using Accretion.GameplayElements.PhysicalLaws.Collision;
using Accretion.GameplayObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    internal class GravitatePowerUp : PowerUp
    {
        private IGravitationalLaw gravitationalLaw;
        protected SoundEffect soundEffect;
        protected AbilitySentinel abilitySentinel = new AbilitySentinel(TimeSpan.FromSeconds(2));

        protected static char displayChar;

        static GravitatePowerUp()
        {
            char.TryParse("G", out displayChar);
        }

        public override char getFieldDisplayCharacter()
        {
            return displayChar;
        }

        public GravitatePowerUp(int uses) : base(uses)
        {
            this.gravitationalLaw = new ClassicGravityFractional(this.gravitationalFactor());
       //     this.soundEffect = AccretionGame.staticContent.Load<SoundEffect>("6142__noisecollector__beam02");
        }

        public GravitatePowerUp(int uses, Vector2 location, Vector2 velocity)
            : base(uses, location, velocity)
        {
            this.gravitationalLaw = new ClassicGravityFractional(this.gravitationalFactor());
         //   this.soundEffect = AccretionGame.staticContent.Load<SoundEffect>("6142__noisecollector__beam02");
        }

        protected internal virtual float gravitationalFactor()
        {
            return 0.4f * this.abilitySentinel.numberOfActiveAbilityInstances();
        }

        public override void use(PlayerObject player, ref Field field)
        {
            if (this.uses > 0)
            {
                if (player != null && field != null)
                {
                    this.abilitySentinel.fireAbility();
                    base.use(player, ref field);
                }
            }
        }

        public override SoundEffect getSoundEffect()
        {
            return this.soundEffect;
        }

        public override void drawAttached(SpriteBatch spriteBatch, Vector2 cameraFieldLocation, float zoomLevel, Vector2? lightSource, PlayerObject player, int windowWidth, int windowHeight)
        {
            base.drawAttached(spriteBatch, cameraFieldLocation, zoomLevel, lightSource, player, windowWidth, windowHeight);
        }

        public override bool onUpdate(PlayerObject player, Field field)
        {
            //todo: parallelize
            if (this.abilitySentinel.isInUse())
            {
                if (field != null && player != null)
                {
                    this.gravitationalLaw = new ClassicGravityFractional(this.gravitationalFactor());

                    foreach (SpaceObject spaceObject in field.getSpaceObjects())
                    {
                        //todo: increase this power if there are stacked powers in use
                        this.gravitationalLaw.applyAcceleration(spaceObject, player);
                    }
                }
            }

            return this.abilitySentinel.isInUse();
        }

        public override string getPowerName()
        {
            return "Graviton Capacitor";
        }
    }
}