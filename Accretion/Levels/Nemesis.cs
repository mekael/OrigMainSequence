using Accretion.GameplayElements.Objects;
using Accretion.Levels.LevelGenerationHelpers;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class Nemesis : Heliocentric
    {
        private const int NEMESIS_INTIAL_SPEED_DIVISOR = -5455; //divide solar system size by this to get initial speed of the nemesis
#if WINDOWS_PHONE
        new protected int MASSES = 700;
        new protected int SOLAR_SYSTEM_SIZE = 40000;
        protected const int STARTING_OBJECT_MAX_MASS = 275;
        protected const int STARTING_PLAYER_MASS = 45;
        private const float NEMESIS_DISTANCE_FACTOR = 5f;
#else
        private const float NEMESIS_DISTANCE_FACTOR = 4.5f;
#endif

        public Nemesis() : base()
        {
            this.music = null;
            this.mapSize = SOLAR_SYSTEM_SIZE * new Vector2(12, 8);
            this.wrapEdges = true;
#if WINDOWS_PHONE
            this.wrapEdges = true;
#endif
        }

        public override string openingText()
        {
            return "There's something ominous on the outskirts of this system. You might want to get really big before it arrives.";
        }

        public override string defeatText()
        {
            return "There's not much you can do about the coming collision, so you better bulk up as quickly as possible.";
        }

        public override PlayerObject player()
        {
            Vector2 position = new Vector2(-SOLAR_SYSTEM_SIZE / 2, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            return new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
        }

        public override List<SpaceObject> spaceObjects()
        {
            SpaceObject player = this.player();

            //make the mini solar system at the outskirts
            int nemesisInitialSpeed = SOLAR_SYSTEM_SIZE / NEMESIS_INTIAL_SPEED_DIVISOR;
            SpaceObject nemesis = this.gravitationalObjects()[1];
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();
            for (int i = 0; i < MASSES / 6; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE / 3, SOLAR_SYSTEM_SIZE / 3), rand.Next(-SOLAR_SYSTEM_SIZE / 3, SOLAR_SYSTEM_SIZE / 3)) + nemesis.getFieldLocation();

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, nemesis) + new Vector2(nemesisInitialSpeed, 0);
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, rand.Next(1, STARTING_OBJECT_MAX_MASS / 3), DEFAULT_DENSITY);
                if ((position2d - nemesis.getFieldLocation()).Length() + spaceObject.getRadius() < nemesis.getRadius() * 4 || spaceObject.hasCollidedWith(player))
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            //now create the main solar system
            SpaceObject sun = this.gravitationalObjects()[0];
            for (int i = 0; i < MASSES * 5 / 6; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE));

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, (int)(MassDistributions.hockeyStick(25) * STARTING_OBJECT_MAX_MASS / 8), DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < sun.getRadius() * 4 || spaceObject.hasCollidedWith(player))
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

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundRadiatingObject sun = new RoundRadiatingObject(new Vector2(0, 0), new Vector2(0, 0), SUN_MASS * 3 / 4, DEFAULT_DENSITY);
            RoundRadiatingObject nemesis = new RoundRadiatingObject(new Vector2(SOLAR_SYSTEM_SIZE * NEMESIS_DISTANCE_FACTOR, 0), new Vector2(SOLAR_SYSTEM_SIZE / NEMESIS_INTIAL_SPEED_DIVISOR, 0), SUN_MASS * 2 / 3, DEFAULT_DENSITY);
            return new List<SpaceObject>(new SpaceObject[] { sun, nemesis });
        }
    }
}