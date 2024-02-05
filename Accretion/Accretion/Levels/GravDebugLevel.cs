using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;
using Accretion.GameplayElements.Objects.PowerUps;

namespace Accretion.Levels
{
    class GravDebugLevel : Heliocentric
    {
        public override PlayerObject player()
        {
            Vector2 position = new Vector2(SOLAR_SYSTEM_SIZE / 2, 0);
            Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position, gravitationalObjects().First());
            PlayerObject player = new PlayerObject(position, orbitalVelocity, STARTING_PLAYER_MASS, DEFAULT_DENSITY, 300);
            //player.setPowerUp(new MeteorDefensePowerUp(100));
            player.setPowerUp(new GravitatePowerUp(100));
            //player.setPowerUp(new RepelPowerUp(100));
            return player;
        }
    }
}
