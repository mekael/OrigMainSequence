using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    internal class LinearDecayGravity : IGravitationalLaw
    {
        private const int GRAVITATIONAL_CONSTANT = 4;

        public void applyAcceleration(SpaceObject currentObject, Objects.SpaceObject gravitatingObject)
        {
            if (gravitatingObject != currentObject)
            {
                Vector2 separationVector = gravitatingObject.getFieldLocation() - currentObject.getFieldLocation();
                float distance = Math.Abs(separationVector.Length());

                //for the last calculation I want the unit vector pointing in the direction of the gravitating object. So I will normalize separationVector
                separationVector.Normalize();

                Vector2 acceleration = gravitatingObject.getMass() * separationVector * GRAVITATIONAL_CONSTANT / distance;

                currentObject.addVelocity(acceleration);
            }
        }

        public Vector2 orbitalVelocity(Vector2 currentObjectFieldLocation, SpaceObject gravitatingMass)
        {
            //calculate a velocity at a right angle to the position
            Vector2 separation = currentObjectFieldLocation - gravitatingMass.getFieldLocation();
            Vector3 position3d = new Vector3(separation.X, separation.Y, 0);
            Vector3 zAxis = Vector3.Backward;
            Vector3 rightAngle3d = Vector3.Cross(position3d, zAxis);
            Vector2 rightAngle2d = new Vector2(rightAngle3d.X, rightAngle3d.Y);
            rightAngle2d.Normalize();

            Vector2 orbitalVelocity = (float)Math.Sqrt(GRAVITATIONAL_CONSTANT * gravitatingMass.getMass()) * rightAngle2d;
            return orbitalVelocity;
        }
    }
}