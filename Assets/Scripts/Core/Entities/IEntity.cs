using UnityEngine;

namespace OneTry.Core.Entities
{
    /// <summary>
    /// Anything that exists in the world and can be the source or target of
    /// gameplay interactions: creatures, projectiles, world objects, items.
    /// Implementations may or may not be MonoBehaviours.
    /// </summary>
    public interface IEntity
    {
        int EntityId { get; }
        Vector3 Position { get; }
        Faction Faction { get; }
        bool IsAlive { get; }
    }
}
