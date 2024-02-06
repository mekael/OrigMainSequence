using Accretion.GameplayElements.Objects;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class SpiralArms : Heliocentric
    {
        protected new const int SOLAR_SYSTEM_SIZE = 120000;
        protected const int INITIAL_ZOOM = 400;
 

        public SpiralArms() : base()
        {
            this.music = null;
            this.victoryCondition = new EatTheSun();
            this.initialZoom = INITIAL_ZOOM;
        }

        public override string openingText()
        {
            return "Many problems, one solution: SWALLOW THE SUN! No really, that's the only way to beat the level.";
        }

        public override string defeatText()
        {
            return null;
        }

        public override List<SpaceObject> spaceObjects()
        {
            SpaceObject player = this.player();

            //check how big the sun is and don't put any objects inside it initially. They add a bit of movement to
            //the sun which makes the whole solar system move
            SpaceObject sun = gravitationalObjects().First();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();

            //Do the horizontal arms
            for (int i = 0; i < MASSES / 2; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE / 8, SOLAR_SYSTEM_SIZE / 8));
                int mass = (int)(rand.Next(1, STARTING_OBJECT_MAX_MASS) * Math.Pow(position2d.Length() * Math.Sqrt(2) / SOLAR_SYSTEM_SIZE, 2));
                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, mass, DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < sun.getRadius() * 4 || spaceObject.hasCollidedWith(player))
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            //Do the vertical arms
            for (int i = 0; i < MASSES / 2; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE / 8, SOLAR_SYSTEM_SIZE / 8), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE));
                int mass = (int)(rand.Next(1, STARTING_OBJECT_MAX_MASS) * Math.Pow(position2d.Length() * Math.Sqrt(2) / SOLAR_SYSTEM_SIZE, 2));
                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, mass, (double).001);
                if (position2d.Length() + spaceObject.getRadius() < sun.getRadius() * 4)
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            return spaceObjects;
        }

        public override PlayerObject player()
        {
            PlayerObject player = base.player();
            player.setFieldLocation(player.getFieldLocation() * new Vector2(2, 1));
            player.setVelocity(gravitationalLaw.orbitalVelocity(player.getFieldLocation(), this.gravitationalObjects().First()));
            return player;
        }
    }
}