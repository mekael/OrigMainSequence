using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    class MutualClassicGravity : ClassicGravity
    {
        public new void calculateAcceleration(SpaceObject currentObject, SpaceObject gravitatingObject)
        {
            Vector2 separationVector = gravitatingObject.getFieldLocation() - currentObject.getFieldLocation();
            float distanceSquared = separationVector.LengthSquared();

            //for the last calculation I want the unit vector pointing in the direction of the gravitating object. So I will normalize separationVector
            separationVector.Normalize();

            Vector2 acceleration = gravitatingObject.getMass() * separationVector * GRAVITATIONAL_CONSTANT / distanceSquared;
            Vector2 equalAndOppositeReation = currentObject.getMass() * separationVector * GRAVITATIONAL_CONSTANT / distanceSquared * -1;

            currentObject.addVelocity(acceleration);
            gravitatingObject.addVelocity(equalAndOppositeReation);
        }
    }
}
