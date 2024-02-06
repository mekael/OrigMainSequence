using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Accretion.GameplayElements.Objects;
using Accretion.Levels.VictoryConditions;
using Accretion.GameplayElements.PhysicalLaws.Collision;
using Accretion.Levels.LevelGenerationHelpers;

namespace Accretion.Levels
{
    class TheFunnel : Level
    {
#if WINDOWS
        protected const int SOLAR_SYSTEM_HEIGHT = 70000;
        protected const int SUN_MASS = 10000;
        protected const int VICTORY_SIZE = SUN_MASS * 19;
        protected const int MASSES = 4000;
        protected const int STARTING_PLAYER_MASS = 60;
        protected const int STARTING_MASS_MAX = 150;
        protected const int INITIAL_ZOOM = 600;
#elif XBOX
        protected const int SOLAR_SYSTEM_HEIGHT = 35000;
        protected const int SUN_MASS = 5000;
        protected const int VICTORY_SIZE = SUN_MASS * 9;
        protected const int MASSES = 1100;
        protected const int STARTING_PLAYER_MASS = 60;
        protected const int STARTING_MASS_MAX = 140;
        protected const int INITIAL_ZOOM = 300;
#elif WINDOWS_PHONE
        protected const int SOLAR_SYSTEM_HEIGHT = 35000;
        protected const int SUN_MASS = 5000;
        protected const int VICTORY_SIZE = SUN_MASS * 9;
        protected const int MASSES = 800;
        protected const int STARTING_PLAYER_MASS = 60;
        protected const int STARTING_MASS_MAX = 140;
        protected const int INITIAL_ZOOM = 300;
#endif

        protected const int SOLAR_SYSTEM_LENGTH = SOLAR_SYSTEM_HEIGHT * 15 / 10;
        private float xOffset = 0;


        public TheFunnel()
        {
#if WINDOWS_PHONE
            this.mapSize = new Vector2(SOLAR_SYSTEM_LENGTH * 15, SOLAR_SYSTEM_HEIGHT * 8);
            this.collisionDetection = new SingleThreadedSlowMoCollisionDetection();
#elif XBOX
            this.collisionDetection = new SingleThreadedSlowMoCollisionDetection();
            this.mapSize = new Vector2(SOLAR_SYSTEM_LENGTH * 20, SOLAR_SYSTEM_HEIGHT * 15);
#else
            this.mapSize = new Vector2(SOLAR_SYSTEM_LENGTH * 20, SOLAR_SYSTEM_HEIGHT * 15);
#endif
            this.victoryCondition = new CriticalMassVictory(VICTORY_SIZE);
            this.gravitationalLaw = new MutualClassicGravity();

            this.xOffset = this.mapSize.X / 2.5f;
            this.initialZoom = INITIAL_ZOOM;
        }

        public override PlayerObject player()
        {
            Vector2 position = new Vector2(-SOLAR_SYSTEM_LENGTH * 8 / 10 - xOffset, 0);
            return new PlayerObject(position, Vector2.Zero, STARTING_PLAYER_MASS, (double).001, 300);
        }

        public override List<SpaceObject> spaceObjects()
        {
            SpaceObject player = this.player();

            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();
            for (int i = 0; i < MASSES; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_LENGTH, SOLAR_SYSTEM_LENGTH) - xOffset, rand.Next(-SOLAR_SYSTEM_HEIGHT, SOLAR_SYSTEM_HEIGHT));

                SpaceObject spaceObject = new RoundObject(position2d, Vector2.Zero, (int)Math.Ceiling(MassDistributions.hockeyStick(10) * STARTING_MASS_MAX), (double).001);

                if (spaceObject.hasCollidedWith(player) || (spaceObject.getFieldLocation() - player.getFieldLocation()).Length() < player.getRadius() * 3)
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
            RoundRadiatingObject sun = new RoundRadiatingObject(new Vector2(SOLAR_SYSTEM_LENGTH - this.xOffset, 0), new Vector2(0, 0), SUN_MASS, (double).001);
            return new List<SpaceObject>(new SpaceObject[] { sun });
        }

        public override string openingText()
        {
            return "There's something interesting happening off to the right; you should make your way over there.\n\nBecome huge to win.";
        }
    }
}
