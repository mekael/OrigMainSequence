﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayObjects;

namespace Accretion.Levels.VictoryConditions
{
    class CriticalMassVictory : VictoryCondition
    {
        private int criticalMass;

        public CriticalMassVictory(int criticalMass)
        {
            this.criticalMass = criticalMass;
        }

        public override GameStatus gameStatus(Field field)
        {
            if (field.getPlayer().getMass() > criticalMass)
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
