using Accretion.GameplayElements.Objects;
using Accretion.Levels.LevelGenerationHelpers;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class TwinPeaks : Heliocentric
    {
        new protected const int MASSES = 6000;
        new protected const int SOLAR_SYSTEM_SIZE = 121000;
        private const int SUN_ORBIT_SPEEDS = 25;
        new protected const int STARTING_PLAYER_MASS = 60;
        new protected const int SUN_MASS = 7000;
        private const float RING_MULTIPLIER = 4f;
        private const double FRACTION_OF_MASS_TO_PUT_IN_RING = .8d;


        public TwinPeaks()
            : base()
        {
            this.mapSize = Vector2.One * SOLAR_SYSTEM_SIZE * 20;
            this.initialZoom = 1500;
        }

        public override string openingText()
        {
            return null;
        }

        public override string defeatText()
        {
            return "The mass you need is out in that ring, which is not very stable by the way.";
        }

        public override PlayerObject player()
        {
            Vector2 position = new Vector2(0, 0);
            Vector2 orbitalVelocity = Vector2.Zero;
            return new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
        }

        public override List<SpaceObject> spaceObjects()
        {
            //make the left solar system
            SpaceObject firstSun = gravitationalObjects()[0];
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();
            for (int i = 0; i < MASSES * (1 - FRACTION_OF_MASS_TO_PUT_IN_RING) / 2; i++)
            {
                Vector2 relativePosition = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE)) / 3;
                Vector2 position = relativePosition + firstSun.getFieldLocation();
                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, firstSun) + new Vector2(0, SUN_ORBIT_SPEEDS);
                int mass = (int)(MassDistributions.hockeyStick(3) * STARTING_PLAYER_MASS * 3 * relativePosition.Length() / SOLAR_SYSTEM_SIZE);
                SpaceObject spaceObject = new RoundObject(position, orbitalVelocity, mass, DEFAULT_DENSITY);
                if ((position - firstSun.getFieldLocation()).Length() + spaceObject.getRadius() < firstSun.getRadius() * 4)
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            //Make the right solar system
            SpaceObject secondSun = gravitationalObjects()[1];
            for (int i = 0; i < MASSES * (1 - FRACTION_OF_MASS_TO_PUT_IN_RING) / 2; i++)
            {
                Vector2 relativePosition = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE)) / 3;
                Vector2 position = relativePosition + secondSun.getFieldLocation();
                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, secondSun) + new Vector2(0, -SUN_ORBIT_SPEEDS);
                int mass = (int)(MassDistributions.hockeyStick(3) * STARTING_PLAYER_MASS * 3 * relativePosition.Length() / SOLAR_SYSTEM_SIZE);
                SpaceObject spaceObject = new RoundObject(position, orbitalVelocity, mass, DEFAULT_DENSITY);

                if ((position - secondSun.getFieldLocation()).Length() + spaceObject.getRadius() < secondSun.getRadius() * 4)
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            //Make the oort cloud
            SpaceObject virtualSun = new RoundObject(Vector2.Zero, Vector2.Zero, SUN_MASS * 2, DEFAULT_DENSITY);
            for (int i = 0; i < MASSES * FRACTION_OF_MASS_TO_PUT_IN_RING; i++)
            {
                Vector2 position2d = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()) - new Vector2(0.5f, 0.5f);
                position2d.Normalize();
                position2d = position2d * rand.Next(SOLAR_SYSTEM_SIZE * 9 / 10, SOLAR_SYSTEM_SIZE) * RING_MULTIPLIER;

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, virtualSun);
                int mass = (int)(MassDistributions.hockeyStick(10) * STARTING_PLAYER_MASS);
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, mass, DEFAULT_DENSITY);
                spaceObjects.Add(spaceObject);
            }

            return spaceObjects;
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundRadiatingObject sun1 = new RoundRadiatingObject(new Vector2(-SOLAR_SYSTEM_SIZE, 0), new Vector2(0, SUN_ORBIT_SPEEDS), SUN_MASS, DEFAULT_DENSITY);
            RoundRadiatingObject sun2 = new RoundRadiatingObject(new Vector2(SOLAR_SYSTEM_SIZE, 0), new Vector2(0, -SUN_ORBIT_SPEEDS), SUN_MASS, DEFAULT_DENSITY);
            return new List<SpaceObject>(new SpaceObject[] { sun1, sun2 });
        }
    }
}