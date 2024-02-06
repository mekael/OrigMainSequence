using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.Objects.PowerUps;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class DebugShrinkField : Heliocentric
    {
        public override PlayerObject player()
        {
            PlayerObject player = base.player();
            return player;
        }

        public override string openingText()
        {
            return "That Mass Reduction Field over there might help you even the odds";
        }

        public override string defeatText()
        {
            return "The Reduction Field will let you absorb larger objects, but it only lasts a few seconds and its strength decreases with distance.";
        }

        public override List<SpaceObject> spaceObjects()
        {
            List<SpaceObject> spaceObjects = base.spaceObjects(4d, 2d);

            //Vector2 powerupLocation = new Vector2(this.gravitationalObjects().FirstOrDefault().getRadius() * 2, 0);
            Vector2 powerupLocation = this.player().getFieldLocation() * 1.1f;

            spaceObjects.Add(new ShrinkFieldPowerUp(5, powerupLocation, this.gravitationalLaw.orbitalVelocity(powerupLocation, this.gravitationalObjects().First())));
            return spaceObjects;
        }

        protected override float angularShapeMultiplier(Vector2 position)
        {
            return (float)Math.Abs(Math.Cos(3 * Math.Atan2(position.X, -position.Y)));
        }
    }
}