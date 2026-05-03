using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat
{
    /// <summary>
    /// Decouples a weapon from the *kind* of projectile it fires. The same
    /// weapon (e.g. "the player's rifle") can be loaded with a different
    /// factory at runtime — bullets, grenades, creatures, items.
    /// </summary>
    public interface IProjectileFactory
    {
        IList<IOnHitAction> OnHitActions { get; }
        IProjectile Create(IEntity source, Vector3 origin, Vector3 direction);
    }
}
