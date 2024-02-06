using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accretion.GameplayElements.Objects;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    interface ICollisionDetection : IDisposable
    {
        bool collisionDetection(List<SpaceObject> collisionObjects);
    }
}
