using Accretion.GameplayElements.Objects;
using Accretion.GraphicHelpers;
using Accretion.Levels.VictoryConditions;
using Microsoft.Xna.Framework;

namespace Accretion.Levels
{
    internal class MenuBackground : Level
    {
#if WINDOWS
        protected const int MASSES = 1000;
        protected readonly Vector2 estimatedScreenSize = new Vector2(1920, 1080);
        protected const int STARTING_ZOOM = 40;
#elif XBOX
        protected const int MASSES = 700;
        protected readonly Vector2 estimatedScreenSize = new Vector2(1920, 1080);
        protected const int STARTING_ZOOM = 40;
#elif WINDOWS_PHONE
        protected const int MASSES = 200;
        protected readonly Vector2 estimatedScreenSize = new Vector2(480, 800);
        protected const int STARTING_ZOOM = 40;
#endif

        protected const int SUN_MASS = 6000;
        protected const int STARTING_OBJECT_MAX_MASS = 70;

        protected int solarSystemSize;

        public MenuBackground()
        {
            this.victoryCondition = new NeverEnding();
            this.initialZoom = STARTING_ZOOM;
            this.startingZoom = STARTING_ZOOM;
            solarSystemSize = (int)FieldAndScreenConversions.GetFieldLocation(estimatedScreenSize, Vector2.Zero, STARTING_ZOOM).Length();
        }

        public override PlayerObject player()
        {
            return null;
        }

        public override List<SpaceObject> spaceObjects()
        {
            SpaceObject sun = gravitationalObjects().First();
            List<SpaceObject> spaceObjects = new List<SpaceObject>();
            Random rand = new Random();

            int minOrbitDistance = (int)(sun.getFieldLocation().Length() + sun.getRadius() * 3);

            for (int i = 0; i < MASSES; i++)
            {
                Vector2 direction = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()) - new Vector2(0.5f, 0.5f);
                direction.Normalize();
                Vector2 position2d = direction * rand.Next(minOrbitDistance, solarSystemSize);
                Vector2 orbitalVelocity = gravitationalLaw.orbitalVelocity(position2d, sun);
                SpaceObject spaceObject = new RoundObject(position2d, orbitalVelocity, rand.Next(1, STARTING_OBJECT_MAX_MASS), DEFAULT_DENSITY);
                spaceObjects.Add(spaceObject);
            }

            return spaceObjects;
        }

        public override List<SpaceObject> gravitationalObjects()
        {
            RoundRadiatingObject sun = new RoundRadiatingObject(new Vector2(0, 0), new Vector2(0, 0), SUN_MASS, DEFAULT_DENSITY);
            sun.unmoveable = true;
            return new List<SpaceObject>(new SpaceObject[] { sun });
        }

        public override string openingText()
        {
            return null;
        }
    }
}