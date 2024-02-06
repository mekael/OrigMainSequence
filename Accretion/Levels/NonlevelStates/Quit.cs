namespace Accretion.Levels.NonlevelStates
{
    internal class Quit : Level
    {
        public override GameplayElements.Objects.PlayerObject player()
        {
            throw new NotImplementedException();
        }

        public override List<GameplayElements.Objects.SpaceObject> spaceObjects()
        {
            throw new NotImplementedException();
        }

        public override List<GameplayElements.Objects.SpaceObject> gravitationalObjects()
        {
            throw new NotImplementedException();
        }

        public override string openingText()
        {
            throw new NotImplementedException();
        }
    }
}