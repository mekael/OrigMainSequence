using Accretion.GameplayElements.Objects;
using Microsoft.Xna.Framework;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    internal interface IGravitationalLaw
    {
        void applyAcceleration(SpaceObject currentObject, SpaceObject gravitatingMass);

        Vector2 orbitalVelocity(Vector2 currentObjectFieldLocation, SpaceObject gravitatingMass);
    }
}