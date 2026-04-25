using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat
{
    public interface IHitTarget
    {
        void OnHit(HitContext context);
    }

    /// <summary>
    /// Single object that travels through the on-hit pipeline. All actions
    /// see and mutate the same fields (so a damage action can amplify an
    /// upcoming heal action, etc.) which is what enables the "everything
    /// affects everything" promise of the issue.
    /// </summary>
    public sealed class HitContext
    {
        public IEntity Source;
        public Creature Victim;
        public Vector3 Position;
        public float DamageAmount;
        public float HealAmount;
        public List<IOnHitAction> OnHitActions = new();

        public HitContext(IEntity source, Vector3 position)
        {
            Source = source;
            Position = position;
        }
    }
}
