using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Accretion.GameplayElements.Objects;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    interface IGravitationalLaw
    {
        void applyAcceleration(SpaceObject currentObject, SpaceObject gravitatingMass);

        Vector2 orbitalVelocity(Vector2 currentObjectFieldLocation, SpaceObject gravitatingMass);
    }
}
