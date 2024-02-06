using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.Objects.PowerUps;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class MeteorDefense : Heliocentric
    {
        public MeteorDefense() : base()
        {
            this.victoryCondition = new CriticalMassVictory(STARTING_PLAYER_MASS * 40);
        }

        public override PlayerObject player()
        {
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE / 3 * 2, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            PlayerObject player = new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
            player.hasGravity = true;
            return player;
        }

        public override string openingText()
        {
            return "Hey look, there's an old abandoned meteor defense module near by. Grab it and try activating it when you're close to one of those big rocks.\n\nGrow 40x larger to win.";
        }

        public override string defeatText()
        {
            return "A meteor defense laser can destroy near by threats. It won't fire on rocks that are smaller than you, much larger than you, or too far away.\n\nYou can always grab a powerup, even if it's larger than you are.";
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            List<SpaceObject> suns = base.gravitationalObjects();
            foreach (SpaceObject sun in suns)
            {
                sun.setMass(sun.getMass() / 2);
            }

            return suns;
        }

        public override List<SpaceObject> spaceObjects()
        {
            PlayerObject player = this.player(); //for reference
            List<SpaceObject> spaceObjects = new List<SpaceObject>(13);
            Vector2 powerupLocation = this.player().getFieldLocation() * 1.05f;
            spaceObjects.Add(new MeteorDefensePowerUp(10, 100d, powerupLocation, this.gravitationalLaw.orbitalVelocity(powerupLocation, this.gravitationalObjects().First())));

            spaceObjects.Add(new RoundObject(Vector2.UnitX * SOLAR_SYSTEM_SIZE, new Vector2(0, -30), player.getMass() * 20, player.getDensity()));
            spaceObjects.Add(new RoundObject(-Vector2.UnitX * SOLAR_SYSTEM_SIZE, new Vector2(0, 30), player.getMass() * 20, player.getDensity()));
            spaceObjects.Add(new RoundObject(Vector2.UnitY * SOLAR_SYSTEM_SIZE, new Vector2(30, 0), player.getMass() * 20, player.getDensity()));
            spaceObjects.Add(new RoundObject(-Vector2.UnitY * SOLAR_SYSTEM_SIZE, new Vector2(-30, 0), player.getMass() * 20, player.getDensity()));

            spaceObjects.Add(new RoundObject(Vector2.UnitX * .625f * SOLAR_SYSTEM_SIZE, new Vector2(0, -30), player.getMass() * 15, player.getDensity()));
            spaceObjects.Add(new RoundObject(-Vector2.UnitX * .625f * SOLAR_SYSTEM_SIZE, new Vector2(0, 30), player.getMass() * 15, player.getDensity()));
            spaceObjects.Add(new RoundObject(Vector2.UnitY * .625f * SOLAR_SYSTEM_SIZE, new Vector2(30, 0), player.getMass() * 15, player.getDensity()));
            spaceObjects.Add(new RoundObject(-Vector2.UnitY * .625f * SOLAR_SYSTEM_SIZE, new Vector2(-30, 0), player.getMass() * 15, player.getDensity()));

            spaceObjects.Add(new RoundObject(Vector2.UnitX * .25f * SOLAR_SYSTEM_SIZE, new Vector2(0, -70), player.getMass() * 10, player.getDensity()));
            spaceObjects.Add(new RoundObject(-Vector2.UnitX * .25f * SOLAR_SYSTEM_SIZE, new Vector2(0, 70), player.getMass() * 10, player.getDensity()));
            spaceObjects.Add(new RoundObject(Vector2.UnitY * .25f * SOLAR_SYSTEM_SIZE, new Vector2(70, 0), player.getMass() * 10, player.getDensity()));
            spaceObjects.Add(new RoundObject(-Vector2.UnitY * .25f * SOLAR_SYSTEM_SIZE, new Vector2(-70, 0), player.getMass() * 10, player.getDensity()));

            SpaceObject sun = this.gravitationalObjects().First();
            Vector2 location = Vector2.UnitX * SOLAR_SYSTEM_SIZE * .9f;
            spaceObjects.Add(new RoundObject(location, gravitationalLaw.orbitalVelocity(location, sun), player.getMass() * 10, player.getDensity()));
            location = -Vector2.UnitX * SOLAR_SYSTEM_SIZE * .9f;
            spaceObjects.Add(new RoundObject(location, gravitationalLaw.orbitalVelocity(location, sun), player.getMass() * 10, player.getDensity()));
            location = Vector2.UnitY * SOLAR_SYSTEM_SIZE * .9f;
            spaceObjects.Add(new RoundObject(location, gravitationalLaw.orbitalVelocity(location, sun), player.getMass() * 10, player.getDensity()));
            location = -Vector2.UnitY * SOLAR_SYSTEM_SIZE * .9f;
            spaceObjects.Add(new RoundObject(location, gravitationalLaw.orbitalVelocity(location, sun), player.getMass() * 10, player.getDensity()));

            return spaceObjects;
        }
    }
}