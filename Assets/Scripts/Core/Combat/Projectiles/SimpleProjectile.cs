using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat.Projectiles
{
    /// <summary>
    /// Plain GameObject projectile. Travels in a straight line and resolves
    /// a hit when it reaches a target — or, in tests, when
    /// <see cref="ResolveHit(IHitTarget)"/> is called directly.
    /// </summary>
    public sealed class SimpleProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private float _maxLifetime = 5f;

        private Vector3 _velocity;
        private float _aliveFor;
        private readonly List<IOnHitAction> _onHit = new();

        public IEntity Source { get; set; }
        public IList<IOnHitAction> OnHitActions => _onHit;

        public void Launch(Vector3 origin, Vector3 direction, float speed)
        {
            transform.position = origin;
            _velocity = direction.sqrMagnitude > 0f
                ? direction.normalized * speed
                : Vector3.zero;
            _aliveFor = 0f;
        }

        private void Update()
        {
            if (_velocity.sqrMagnitude > 0f)
            {
                transform.position += _velocity * Time.deltaTime;
            }
            _aliveFor += Time.deltaTime;
            if (_aliveFor >= _maxLifetime) Destroy(gameObject);
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
    /// Factory for <see cref="SimpleProjectile"/>. The on-hit action list
    /// is mutable — wrap it with item effects to mutate behavior at runtime.
    /// </summary>
    public sealed class SimpleProjectileFactory : IProjectileFactory
    {
        private readonly List<IOnHitAction> _onHit = new();

        public IList<IOnHitAction> OnHitActions => _onHit;

        public IProjectile Create(IEntity source, Vector3 origin, Vector3 direction)
        {
            var go = new GameObject("Projectile");
            var p = go.AddComponent<SimpleProjectile>();
            p.Source = source;
            return p;
        }
    }
}
