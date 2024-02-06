using Accretion.GameplayElements.Objects;

namespace Accretion.GameplayElements.PhysicalLaws.Collision
{
    internal interface ICollisionDetection : IDisposable
    {
        bool collisionDetection(List<SpaceObject> collisionObjects);
    }
}