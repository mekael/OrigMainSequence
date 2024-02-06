using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.PhysicalLaws.Collision;
using Accretion.Levels.LevelGenerationHelpers;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class Heliocentric : Level
    {

        protected const int MASSES = 6000;
        protected const int SOLAR_SYSTEM_SIZE = 60000;
        protected const int STARTING_OBJECT_MAX_MASS = 120;
        protected const int STARTING_PLAYER_MASS = 25;
        protected const int SUN_MASS = 8000;


        public Heliocentric()
        {

            this.mapSize = new Vector2(SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE) * 15;

            this.gravitationalLaw = new ClassicGravity();
        }

        public override string openingText()
        {
            return "Hey look, a star! Time to practice some orbital mechanics."
                + Environment.NewLine + Environment.NewLine
                + "Become the largest object to win";
        }

        public override string defeatText()
        {
            return "Orbital mechanics can be counter intuitive. You can reach a higher orbit by speeding up, or a lower orbit by slowing down. Try not to fight gravity directly.";
        }

        public override PlayerObject player()
        {
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE / 2, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            return new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, DEFAULT_EJECTION_SPEED);
        }

        public override List<SpaceObject> spaceObjects()
        {
            return this.spaceObjects(1d, 1d);
        }

        protected List<SpaceObject> spaceObjects(double massMultiplier, double solarSystemSizeMultiplier)
        {
            SpaceObject player = this.player();
            //temprarily enlarge the player a little to ensure that nothing is placed too near it. A new copy will be returned
            //to anyone who calls this.player, so don't even worry about reverting it back later
            player.setMass(player.getMass() * 2);

            //check how big the sun is and don't put any objects inside it initially. They add a bit of movement to
            //the sun which makes the whole solar system move
            SpaceObject sun = gravitationalObjects().First();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();
            for (int i = 0; i < MASSES; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE)) * (float)solarSystemSizeMultiplier;
                position2d = position2d * this.angularShapeMultiplier(position2d);

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                int mass = (int)(MassDistributions.hockeyStick(40) * STARTING_OBJECT_MAX_MASS * position2d.Length() * massMultiplier / 5 / SOLAR_SYSTEM_SIZE);
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

            return spaceObjects;
        }

        protected virtual float angularShapeMultiplier(Vector2 position)
        {
            return 1f;
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundObject sun = new RoundRadiatingObject(new Vector2(0, 0), new Vector2(0, 0), SUN_MASS, DEFAULT_DENSITY);
            return new List<SpaceObject>(new SpaceObject[] { sun });
        }
    }
}