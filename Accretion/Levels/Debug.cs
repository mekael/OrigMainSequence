using Accretion.GameplayElements.Objects;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class Debug : Level
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