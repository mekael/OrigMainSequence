using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;
using Accretion.GameplayElements.Objects.PowerUps;

namespace Accretion.Levels
{
    class Nebulous : Level
    {
#if WINDOWS
        protected const int MASSES = 6000;
        protected const int SOLAR_SYSTEM_SIZE = 60000;
        protected const int STARTING_OBJECT_MAX_MASS = 1;
        protected const int STARTING_PLAYER_MASS = 25;
#elif XBOX
        protected const int MASSES = 1200;
        protected const int SOLAR_SYSTEM_SIZE = 40000;
        protected const int STARTING_OBJECT_MAX_MASS = 2;
        protected const int STARTING_PLAYER_MASS = 40;
#elif WINDOWS_PHONE
        protected const int MASSES = 880;
        protected const int SOLAR_SYSTEM_SIZE = 40000;
        protected const int STARTING_OBJECT_MAX_MASS = 2;
        protected const int STARTING_PLAYER_MASS = 40;
#endif
        public Nebulous()
        {
            this.victoryCondition = new VictoryConditions.CriticalMassVictory(this.player().getMass() + (int)(MASSES * STARTING_OBJECT_MAX_MASS * 0.53));
            this.mapSize = SOLAR_SYSTEM_SIZE * 4 * Vector2.One;
            this.wrapEdges = false;
        }

        public override PlayerObject player()
        {
            return new PlayerObject(Vector2.Zero, Vector2.Zero, STARTING_PLAYER_MASS, DEFAULT_DENSITY, DEFAULT_EJECTION_SPEED);
        }

        public override List<SpaceObject> spaceObjects()
        {
            PlayerObject player = this.player();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();

            for (int i = 0; i < MASSES; i++)
            {
                double angle = rand.NextDouble() * 2 * Math.PI;
                double radius = angle / Math.PI / 2 * SOLAR_SYSTEM_SIZE * (1 + 0.3 * rand.NextDouble());

                Vector2 position2d = (float)radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                //everything slowly moves apart
                Vector2 velocity = new Vector2(position2d.X, position2d.Y);
                velocity.Normalize();
                SpaceObject spaceObject = new RoundObject(position2d, velocity, STARTING_OBJECT_MAX_MASS, DEFAULT_DENSITY);
                if ((position2d - player.getFieldLocation()).Length() < 4 * spaceObject.getRadius() + player.getRadius())
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
            spaceObjects.Add(new GravitatePowerUp(8, powerupLocation, Vector2.Zero));

            return spaceObjects;
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            return new List<SpaceObject>();
        }

        public override string openingText()
        {
            return "There's not much to work with here, but that Graviton Capacitor might help you gather some mass.\n\n Collect most of the mass in the nebula to win";
        }
    }
}
