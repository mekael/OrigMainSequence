using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;

namespace Accretion.Levels.VictoryConditions
{
    class MajorityOfTheMassVictory : VictoryCondition
    {
        public override GameStatus gameStatus(GameplayObjects.Field field)
        {
            int totalMass = 0;
            int smallestMass = field.getSpaceObjects().First().getMass();

            foreach (SpaceObject spaceObject in field.getSpaceObjects())
            {
                totalMass += spaceObject.getMass();

                if (spaceObject.getMass() < smallestMass)
                {
                    smallestMass = spaceObject.getMass();
                }
            }

            if (field.getPlayer().getMass() <= smallestMass)
            {
                this.onDefeat();
                return GameStatus.Defeat;
            }
            else if (field.getPlayer().getMass() >= totalMass / 2)
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
