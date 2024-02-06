using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.Objects.PowerUps;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class DebugSpeedBoost : Heliocentric
    {
        public override PlayerObject player()
        {
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE / 2, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            PlayerObject player = new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
            player.setPowerUp(new FreeMovePowerUp(100));
            //player.setPowerUp(new GravitatePowerUp(100));
            //player.setPowerUp(new RepelPowerUp(100));
            return player;
        }
    }
}