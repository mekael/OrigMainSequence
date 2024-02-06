using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    class CounterRevolutionary : Heliocentric
    {
        new protected const int SOLAR_SYSTEM_SIZE = 50000;
        public CounterRevolutionary() : base()
        {
            this.music = null;
        }

        public override string openingText()
        {
            return null;
        }

        public override string defeatText()
        {
            return "Those two rings are going to collide if the star gains mass and gravitational strength.";
        }

        public override List<SpaceObject> spaceObjects()
        {
            //keep track of the player to ensure we don't put any objects on top of them
            SpaceObject player = this.player();

            //check how big the sun is and don't put any objects inside it initially. They add a bit of movement to
            //the sun which makes the whole solar system move
            SpaceObject sun = gravitationalObjects().First();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();
            for (int i = 0; i < MASSES * 2 / 5; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE / 2, SOLAR_SYSTEM_SIZE / 2), rand.Next(-SOLAR_SYSTEM_SIZE / 2, SOLAR_SYSTEM_SIZE / 2));

                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, rand.Next(1, STARTING_PLAYER_MASS), DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < sun.getRadius() * 4 || position2d.Length() + spaceObject.getRadius() > SOLAR_SYSTEM_SIZE / 2 || spaceObject.hasCollidedWith(player))
                {
                    i--;
                }
                else
                {
                    spaceObjects.Add(spaceObject);
                }
            }

            for (int i = 0; i < MASSES * 3 / 5; i++)
            {
                Vector2 position2d = new Vector2(rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE), rand.Next(-SOLAR_SYSTEM_SIZE, SOLAR_SYSTEM_SIZE));

                Vector2 orbitalVelocity = -gravitationalLaw.orbitalVelocity(position2d, gravitationalObjects().First());
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, rand.Next(1, STARTING_OBJECT_MAX_MASS), DEFAULT_DENSITY);
                if (position2d.Length() + spaceObject.getRadius() < SOLAR_SYSTEM_SIZE / 2 * 1.2 || spaceObject.hasCollidedWith(player))
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
            List<SpaceObject> suns = base.gravitationalObjects();
            foreach (SpaceObject sun in suns)
            {
                sun.unmoveable = true;
            }

            return suns;
        }

        public override PlayerObject player()
        {
            PlayerObject player = base.player();
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE / 2, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            player.setFieldLocation(position);
            player.setVelocity(orbitalVelocity);
            return player;
        }
    }
}
