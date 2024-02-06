using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.Objects.PowerUps;
using Accretion.Levels.LevelGenerationHelpers;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class Shell : Heliocentric
    {
        new protected const int MASSES = 6000;
        new protected const int SOLAR_SYSTEM_SIZE = 120000;
        new protected const int STARTING_OBJECT_MAX_MASS = 240;
        new protected const int STARTING_PLAYER_MASS = 25;
        new protected const int SUN_MASS = 48000;


        public override PlayerObject player()
        {
            Vector2 position = new Vector2(-SOLAR_SYSTEM_SIZE * 1.2f, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            return new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundObject sun = new RoundRadiatingObject(new Vector2(0, 0), new Vector2(0, 0), SUN_MASS, DEFAULT_DENSITY);
            return new List<SpaceObject>(new SpaceObject[] { sun });
        }

        public override string openingText()
        {
            return "This system is encased in a protective Dyson sphere, but that Mass Reduction Field generator might help you break in";
        }

        public override string defeatText()
        {
            return "The Reduction Field will let you absorb larger objects, but it only lasts a few seconds and its strength decreases with distance.";
        }

        public override List<SpaceObject> spaceObjects()
        {
            PlayerObject player = this.player();
            SpaceObject exampleBoundaryObject = new RoundObject(new Vector2(), new Vector2(), player.getMass() * 2, player.getDensity());
            float shellRadius = SOLAR_SYSTEM_SIZE * 1.05f;
            SpaceObject sun = gravitationalObjects().First();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();

            //add a protective outter shell
            int shellMasses = 0;
            for (double i = exampleBoundaryObject.getRadius() * 2 / shellRadius; i < 2 * Math.PI; i += exampleBoundaryObject.getRadius() * 2 * 2 / shellRadius)
            {
                Vector2 position = shellRadius * new Vector2((float)Math.Cos(i), (float)Math.Sin(i));
                Vector2 orbitalVelocity = orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
                spaceObjects.Add(new RoundObject(position, orbitalVelocity, player.getMass() * 2, player.getDensity()));
                shellMasses++;
            }

            for (int i = 0; i < MASSES - shellMasses; i++)
            {
                double angle = rand.NextDouble() * 2 * Math.PI;
                double radius = rand.NextDouble() * SOLAR_SYSTEM_SIZE;

                Vector2 position2d = (float)radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                int mass = (int)(MassDistributions.hockeyStick(40) * STARTING_OBJECT_MAX_MASS * position2d.Length() / 5 / SOLAR_SYSTEM_SIZE);
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, mass, DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < sun.getRadius() * 4 || (position2d - player.getFieldLocation()).Length() < 4 * spaceObject.getRadius() + player.getRadius())
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            //Add the powerup
            Vector2 powerupLocation = player.getFieldLocation() - Vector2.UnitY * player.getRadius() * 16;
            spaceObjects.Add(new ShrinkFieldPowerUp(3, powerupLocation, this.gravitationalLaw.orbitalVelocity(powerupLocation, this.gravitationalObjects().First())));

            return spaceObjects;
        }

        protected override float angularShapeMultiplier(Vector2 position)
        {
            return 1f;
        }
    }
}