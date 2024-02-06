using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accretion.Levels.VictoryConditions
{
    class NeverEnding : VictoryCondition
    {
        public override GameStatus gameStatus(GameplayObjects.Field field)
        {
            return GameStatus.InProgress;
        }
    }
}
