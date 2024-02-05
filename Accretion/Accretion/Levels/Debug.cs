using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;
using Accretion.Levels.VictoryConditions;
using Accretion.GameplayElements.PhysicalLaws.Collision;

namespace Accretion.Levels
{
    class Debug : Level
    {
        public Debug()
        {
            this.victoryCondition = new NeverEnding();
        }

        public override GameplayElements.Objects.PlayerObject player()
        {
            return new PlayerObject(Vector2.Zero, Vector2.Zero, 600, (double).001, 30);
        }

        public override List<GameplayElements.Objects.SpaceObject> spaceObjects()
        {
            return null;
        }

        public override List<GameplayElements.Objects.SpaceObject> gravitationalObjects()
        {
            return null;
        }

        public override string openingText()
        {
            return null;
        }
    }
}
