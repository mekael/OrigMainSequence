using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    internal class ClassicGravity : IGravitationalLaw
    {
        protected const int GRAVITATIONAL_CONSTANT = 40000;

        public virtual void applyAcceleration(SpaceObject currentObject, SpaceObject gravitatingObject)
        {
            if (gravitatingObject != currentObject)
            {
                Vector2 acceleration = calculateAcceleration(currentObject, gravitatingObject);

                currentObject.addVelocity(acceleration);
            }
        }

        protected Vector2 calculateAcceleration(SpaceObject currentObject, SpaceObject gravitatingObject)
        {
            Vector2 separationVector = gravitatingObject.getFieldLocation() - currentObject.getFieldLocation();
            float distanceSquared = separationVector.LengthSquared();

            //for the last calculation I want the unit vector pointing in the direction of the gravitating object. So I will normalize separationVector
            separationVector.Normalize();

            return gravitatingObject.getMass() * separationVector * GRAVITATIONAL_CONSTANT / distanceSquared;
        }

        public virtual Vector2 orbitalVelocity(Vector2 currentObjectFieldLocation, SpaceObject gravitatingMass)
        {
            //calculate a velocity at a right angle to the position
            Vector2 separation = currentObjectFieldLocation - gravitatingMass.getFieldLocation();
            Vector3 position3d = new Vector3(separation.X, separation.Y, 0);
            Vector3 zAxis = Vector3.Backward;
            Vector3 rightAngle3d = Vector3.Cross(position3d, zAxis);
            Vector2 rightAngle2d = new Vector2(rightAngle3d.X, rightAngle3d.Y);
            rightAngle2d.Normalize();

            Vector2 orbitalVelocity = (float)Math.Sqrt(GRAVITATIONAL_CONSTANT * gravitatingMass.getMass() / separation.Length()) * rightAngle2d;
            return orbitalVelocity;
        }
    }
}