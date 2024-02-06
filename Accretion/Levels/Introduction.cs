﻿using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.Objects.PowerUps;
using Accretion.GraphicHelpers;
using Accretion.Levels.LevelGenerationHelpers;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class Introduction : Level
    {
        protected const int MASSES = 2500;
        private const int SOLAR_SYSTEM_SIZE = 60000;
        private const int VICTORY_MASS = STARTING_OBJECT_MAX_MASS * 40;
        private const int INITIAL_ZOOM = 200;

        protected const int STARTING_OBJECT_MAX_MASS = 60;

        public Introduction()
        {
            this.victoryCondition = new CriticalMassVictory(STARTING_OBJECT_MAX_MASS * 40);
            this.initialZoom = INITIAL_ZOOM;
            this.mapSize = SOLAR_SYSTEM_SIZE * 3 * Vector2.One;
            this.wrapEdges = true;
        }

        public override string successText()
        {
            return "Excelent, you're big enough that nothing here can threaten you.";
        }

        public override string openingText()
        {
            return "Hey, what's a little rock like you doing bippin' and boppin' to the interstellar beat? You better absorb some smaller rocks and avoid the bigger ones if you want to survive. Move wisely; you lose mass each time."
            + Environment.NewLine + Environment.NewLine
            + PlatformSpecificStrings.CONTROLS
            + Environment.NewLine + Environment.NewLine
            + "Grow really big to win.";
        }

        public override string defeatText()
        {
            return "Careful! Don't collide with objects that are more massive than you.";
        }

        public override PlayerObject player()
        {
            PlayerObject player = new PlayerObject(Vector2.Zero, Vector2.Zero, STARTING_OBJECT_MAX_MASS, DEFAULT_DENSITY, 200);
            player.setPowerUp(new HintPowerUp(PlatformSpecificStrings.ZOOM_HINT));
            return player;
        }

        public override List<SpaceObject> spaceObjects()
        {
            SpaceObject player = this.player();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();
            for (int i = 0; i < MASSES; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE));

                Vector2 velocity = new Vector2(rand.Next(0, 8), rand.Next(0, 8));
                SpaceObject spaceObject = new RoundObject(position2d, velocity, (int)Math.Ceiling(MassDistributions.hockeyStick(STARTING_OBJECT_MAX_MASS) * 10), DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < player.getRadius() * 16 || position2d.Length() > SOLAR_SYSTEM_SIZE)
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
            return new List<SpaceObject>();
        }
    }
}