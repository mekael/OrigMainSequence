using Accretion.GameplayElements.Objects;
using Accretion.Levels.LevelGenerationHelpers;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class BinaryStar : Heliocentric
    {
        //construct a virtual mass to represent the two suns when calcuating orbits
        private SpaceObject virtualSun = new RoundObject(Vector2.Zero, Vector2.Zero, SUN_MASS * 2, DEFAULT_DENSITY);

        protected const int SUN_VELOCITY = 130;
#if WINDOWS_PHONE
        new protected const int MASSES = 800;
#endif

        public BinaryStar() : base()
        {
            this.music = null;
#if WINDOWS_PHONE
            this.mapSize = new Vector2(SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE) * 8;
#endif
        }

        public override string openingText()
        {
            return null;
        }

        public override string defeatText()
        {
            return null;
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundRadiatingObject sun = new RoundRadiatingObject(new Vector2(SOLAR_SYSTEM_SIZE / 11, 0), new Vector2(0, -SUN_VELOCITY), SUN_MASS, DEFAULT_DENSITY * 2);
            RoundRadiatingObject sun2 = new RoundRadiatingObject(new Vector2(-SOLAR_SYSTEM_SIZE / 11, 0), new Vector2(0, SUN_VELOCITY), SUN_MASS, DEFAULT_DENSITY * 2);

            return new List<SpaceObject>(new SpaceObject[] { sun, sun2 });
        }

        public override List<SpaceObject> spaceObjects()
        {
            //check how big the suns are and don't put any objects inside their orbit initially. They add a bit of movement to
            //the sun which makes the whole solar system move
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            SpaceObject player = this.player();
            Random rand = new Random();
            float sizeFactor = 1.2f;
            for (int i = 0; i < MASSES; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE)) * sizeFactor;

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, virtualSun);
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, (int)(MassDistributions.hockeyStick(30) * STARTING_OBJECT_MAX_MASS / 5), DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < SOLAR_SYSTEM_SIZE / 1.5 || spaceObject.hasCollidedWith(player))
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
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE / 1.3f, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, virtualSun);
            return new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
        }
    }
}