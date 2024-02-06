using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.PhysicalLaws.Collision;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class WhirlPool : Heliocentric
    {
        //overrides won't be useful here since we're using all the base class functions

#if WINDOWS
        private const float VELOCITY_REDUCTION_FACTOR = 0.62f;
        private const int sunMassDivisor = 2;
        private const int gravityDivisor = 160;
        private readonly Vector2 MAP_SIZE = new Vector2(SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE) * 8;
#elif XBOX
        private const float VELOCITY_REDUCTION_FACTOR = 0.62f;
        private const int sunMassDivisor = 3;
        private const int gravityDivisor = 160;
        private readonly Vector2 MAP_SIZE = new Vector2(SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE) * 8;
#elif WINDOWS_PHONE
        private const float VELOCITY_REDUCTION_FACTOR = 0.62f;
        private const int sunMassDivisor = 3;
        private const int gravityDivisor = 130;
        private readonly Vector2 MAP_SIZE = new Vector2(SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE) * 6;
#endif

        public WhirlPool()
        {
            this.victoryCondition = new EatTheSun();
            this.gravitationalLaw = new ClassicGravityFractional(gravityDivisor);
            this.mapSize = new Vector2(SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE) * 8;
            this.wrapEdges = false;
        }

        public override string openingText()
        {
            return "This moon's gravity feels pretty weak, but the rocks around you are slowly falling out of orbit anyway. You'll have to absorb the moon to be safe.\n\nAbsorb the central mass to win";
        }

        public override string defeatText()
        {
            return "The moon's density is deceiving. Use the color, not the size, to decide if you can absorb it. Work quickly before it gains too much mass though.";
        }

        public override List<SpaceObject> spaceObjects()
        {
            List<SpaceObject> spaceObjects = base.spaceObjects();
            foreach (SpaceObject currentObject in spaceObjects)
            {
                Vector2 velocity = currentObject.getVelocity();
                velocity *= VELOCITY_REDUCTION_FACTOR;
                currentObject.setVelocity(velocity);
            }

            //this level runs a bit slow on xbox... got to cull some objects :(
            //TODO: remove this is optimizations allow
            return spaceObjects;
        }

        public override PlayerObject player()
        {
            SpaceObject player = base.player();
            player.setVelocity(player.getVelocity() * VELOCITY_REDUCTION_FACTOR);
            return base.player();
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            SpaceObject sun = base.gravitationalObjects().First();
            sun.setMass(sun.getMass() / sunMassDivisor);
            sun.setDensity(sun.getDensity() / sunMassDivisor);
            sun.unmoveable = true;
            return new List<SpaceObject> { sun };
        }
    }
}