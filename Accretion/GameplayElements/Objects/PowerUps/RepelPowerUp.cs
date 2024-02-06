
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    class RepelPowerUp : GravitatePowerUp
    {
        public RepelPowerUp(int uses) : base(uses) {}

        protected static char displayChar;

        static RepelPowerUp()
        {
            char.TryParse("A", out displayChar);
        }

        protected internal override float gravitationalFactor()
        {
            return -0.6f;
        }

        public override char getFieldDisplayCharacter()
        {
            return displayChar;
        }

        public override string getPowerName()
        {
            return "Antigraviton Capacitor";
        }
    }
}
