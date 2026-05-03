using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat.Projectiles
{
    /// <summary>
    /// Wraps an existing <see cref="Creature"/> so it can be fired as ammo
    /// by a <see cref="Weapon"/>. The wrapped creature retains its weapons,
    /// statuses, and brain — so it can keep doing things after landing
    /// (e.g. attacking with a stolen weapon as in the issue's example).
    /// </summary>
    public sealed class CreatureAsProjectile : MonoBehaviour, IProjectile
    {
        private Vector3 _velocity;
        private readonly List<IOnHitAction> _onHit = new();
        public Creature Creature;

        public IEntity Source { get; set; }
        public IList<IOnHitAction> OnHitActions => _onHit;

        public void Launch(Vector3 origin, Vector3 direction, float speed)
        {
            if (Creature != null) Creature.transform.position = origin;
            _velocity = direction.sqrMagnitude > 0f
                ? direction.normalized * speed
                : Vector3.zero;
        }

        private void Update()
        {
            if (Creature == null) return;
            Creature.transform.position += _velocity * Time.deltaTime;
        }

        public void ResolveHit(IHitTarget target)
        {
            if (target == null) return;
            var ctx = new HitContext(Source, transform.position);
            for (int i = 0; i < _onHit.Count; i++) ctx.OnHitActions.Add(_onHit[i]);
            target.OnHit(ctx);
        }
    }

    /// <summary>
    /// Factory that wraps a fixed <see cref="Creature"/> as ammo. Useful for
    /// the literal scenario in the issue ("an enemy fires another enemy").
    /// </summary>
    public sealed class CreatureProjectileFactory : IProjectileFactory
    {
        private readonly System.Func<Creature> _supplier;
        private readonly List<IOnHitAction> _onHit = new();

        public CreatureProjectileFactory(System.Func<Creature> supplier)
        {
            _supplier = supplier;
        }

        public IList<IOnHitAction> OnHitActions => _onHit;

        public IProjectile Create(IEntity source, Vector3 origin, Vector3 direction)
        {
            var creature = _supplier?.Invoke();
            if (creature == null) return null;
            var p = creature.gameObject.AddComponent<CreatureAsProjectile>();
            p.Creature = creature;
            p.Source = source;
            return p;
        }
    }
}
