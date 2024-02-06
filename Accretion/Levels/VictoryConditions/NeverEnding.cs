namespace Accretion.Levels.VictoryConditions
{
    internal class NeverEnding : VictoryCondition
    {
        public override GameStatus gameStatus(GameplayObjects.Field field)
        {
            return GameStatus.InProgress;
        }
    }
}