using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayObjects;
using Accretion.GameplayElements.Objects;
using System.Collections.ObjectModel;

namespace Accretion.Levels.VictoryConditions
{
    class EatTheSun : VictoryCondition
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
