using Accretion.GameplayElements.Objects;
using Accretion.GameplayObjects;
using System.Collections.ObjectModel;

namespace Accretion.Levels.VictoryConditions
{
    internal class EatTheSun : VictoryCondition
    {
        public override GameStatus gameStatus(Field field)
        {
            ReadOnlyCollection<SpaceObject> suns = field.getGravitationalObjects();
            if (field.getPlayer() != null && (suns == null || suns.Count < 1))
            {
                this.onVictory();
                return GameStatus.Victory;
            }
            else
            {
                return base.gameStatus(field);
            }
        }
    }
}