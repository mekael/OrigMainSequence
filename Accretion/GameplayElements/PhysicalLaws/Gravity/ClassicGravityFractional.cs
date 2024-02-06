using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    internal class ClassicGravityFractional : ClassicGravity
    {
        private float reductionFactor = 200f;

        public ClassicGravityFractional(float reductionFactor)
        {
            this.reductionFactor = reductionFactor;
        }

        public override void applyAcceleration(Objects.SpaceObject currentObject, Objects.SpaceObject gravitatingObject)
        {
            if (gravitatingObject != currentObject)
            {
                Vector2 acceleration = calculateAcceleration(currentObject, gravitatingObject);

                currentObject.addVelocity(acceleration / reductionFactor);
            }
        }

        public override Vector2 orbitalVelocity(Vector2 currentObjectFieldLocation, Objects.SpaceObject gravitatingMass)
        {
            return base.orbitalVelocity(currentObjectFieldLocation, gravitatingMass) / (float)Math.Sqrt(reductionFactor);
        }
    }
}