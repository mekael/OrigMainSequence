namespace Accretion.GameplayElements.Objects.PowerUps
{
    internal class RepelPowerUp : GravitatePowerUp
    {
        public RepelPowerUp(int uses) : base(uses)
        {
        }

        protected static char displayChar;

        static RepelPowerUp()
        {
            char.TryParse("A", out displayChar);
        }

        protected internal override float gravitationalFactor()
        {
            return -0.6f;
        }

        public override char getFieldDisplayCharacter()
        {
            return displayChar;
        }

        public override string getPowerName()
        {
            return "Antigraviton Capacitor";
        }
    }
}