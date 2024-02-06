using Accretion.GameplayElements.Objects;
using Accretion.GameplayElements.PhysicalLaws.Collision;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal abstract class Level
    {
        protected const double DEFAULT_DENSITY = (double).001;
        protected const int DEFAULT_EJECTION_SPEED = 300;

        public IGravitationalLaw gravitationalLaw = new ClassicGravity();
        public bool wrapEdges = false;
        public bool cameraLockedOnPlayer = true;
        public float initialZoom = 100;
        public float startingZoom = 20;
        public Vector2 mapSize = new Vector2(7000000, 7000000);
        public Color backgroundColor = Color.Black;

        public abstract PlayerObject player();

        public abstract List<SpaceObject> spaceObjects();

        public abstract List<SpaceObject> gravitationalObjects();

        public abstract String openingText();

        public virtual String defeatText()
        {
            return null;
        }

        public virtual String successText()
        {
            return null;
        }

        public VictoryCondition victoryCondition = new LargestMassVictory();

        public String music;

        public ICollisionDetection collisionDetection;
    }
}