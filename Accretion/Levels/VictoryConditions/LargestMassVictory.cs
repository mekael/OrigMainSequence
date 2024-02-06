using Accretion.GameplayElements.Objects;

namespace Accretion.Levels.VictoryConditions
{
    internal class LargestMassVictory : VictoryCondition
    {
        public override GameStatus gameStatus(GameplayObjects.Field field)
        {
            if (field.getPlayer() == null || field.getPlayer().getMass() <= 0)
            {
                this.onDefeat();
                return GameStatus.Defeat;
            }

            foreach (SpaceObject spaceObject in field.getSpaceObjects())
            {
                if (spaceObject.getMass() > field.getPlayer().getMass())
                {
                    return GameStatus.InProgress;
                }
            }

            this.onVictory();
            return GameStatus.Victory;
        }
    }
}