using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    class Rings : Heliocentric
    {
#if WINDOWS
        new protected const int STARTING_OBJECT_MAX_MASS = 92;
        new protected const int STARTING_PLAYER_MASS = 60;
        new protected const int SUN_MASS = 40000;
        protected const int INITIAL_ZOOM = 600;
#elif XBOX
        new protected const int STARTING_OBJECT_MAX_MASS = 132;
        new protected const int STARTING_PLAYER_MASS = 88;
        new protected const int SUN_MASS = 44000;
        protected const int INITIAL_ZOOM = 500;
#elif WINDOWS_PHONE
        protected const int MASSES = 850;
        new protected const int STARTING_OBJECT_MAX_MASS = 148;
        new protected const int STARTING_PLAYER_MASS = 100;
        new protected const int SUN_MASS = 40000;
        protected const int INITIAL_ZOOM = 500;
#endif

        public Rings()
        {
            this.initialZoom = INITIAL_ZOOM;
        }

        public override string openingText()
        {
            return "This planetary system has quite a few rings and moons. Make sure you bulk up enough before you venture into the outer rings made of more massive rocks.";
        }

        public override string defeatText()
        {
            return null;
        }

        public override List<SpaceObject> spaceObjects()
        {
            List<SpaceObject> rings = new List<SpaceObject>();
            SpaceObject sun = this.gravitationalObjects().First();
            Random rand = new Random();

            //pick a magic int that will give us the poper number of masses in the end
            int magicNumber = (int)Math.Pow(MASSES / (Math.Sqrt(7) + Math.Sqrt(8) + Math.Sqrt(9) + (Math.Sqrt(10))), 2);

            //4 rings
            for (int ring = 0; ring < 4; ring++)
            {
                for (int rock = 0; rock < (int) Math.Sqrt(magicNumber * (ring + 7)); rock++)
                {
                    Vector2 location = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()) - new Vector2(0.5f, 0.5f);
                    location.Normalize();
                    location = location * (int)(rand.Next(0, (int)(SOLAR_SYSTEM_SIZE / 5 * (0.75 + 0.25 * ring))) + (ring + 1) * SOLAR_SYSTEM_SIZE * (0.75 + 0.25 * ring / 4));
                    Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(location, sun);
                    int mass = rand.Next(STARTING_PLAYER_MASS * (int) Math.Pow(ring, 2) + STARTING_PLAYER_MASS / 5, STARTING_PLAYER_MASS * (int) Math.Pow(ring, 2) * 2 + STARTING_PLAYER_MASS / 3);
                    rings.Add(new RoundObject(location, orbitalVelocity, mass, DEFAULT_DENSITY));
                }
            }

            return rings;
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundRadiatingObject sun = new RoundRadiatingObject(new Vector2(0, 0), new Vector2(0, 0), SUN_MASS, DEFAULT_DENSITY);
            return new List<SpaceObject>(new SpaceObject[] { sun });
        }

        public override PlayerObject player()
        {
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            return new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
        }
    }
}
