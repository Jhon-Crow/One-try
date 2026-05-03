using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat
{
    /// <summary>
    /// Anything a weapon can fire. A SimpleProjectile, a creature, another
    /// weapon, an item — any of those can implement this interface.
    /// </summary>
    public interface IProjectile
    {
        IEntity Source { get; set; }
        IList<IOnHitAction> OnHitActions { get; }
        void Launch(Vector3 origin, Vector3 direction, float speed);
    }
}
