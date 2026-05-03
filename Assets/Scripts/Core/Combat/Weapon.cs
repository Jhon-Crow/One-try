using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat
{
    /// <summary>
    /// Reference <see cref="IWeapon"/> implementation. Plain C# class — not a
    /// MonoBehaviour — so the same instance can move between creatures
    /// without rebinding GameObjects (which is exactly what R6 wants).
    /// </summary>
    public class Weapon : IWeapon
    {
        private float _readyAt;

        public string Name { get; }
        public Creature Wielder { get; set; }
        public IProjectileFactory ProjectileFactory { get; set; }
        public float Cooldown { get; }

        public Weapon(string name, IProjectileFactory factory, float cooldown = 0.25f)
        {
            Name = name;
            ProjectileFactory = factory;
            Cooldown = cooldown;
        }

        public bool CanFire => Time.time >= _readyAt && ProjectileFactory != null;

        public virtual bool TryFire(Vector3 origin, Vector3 direction)
        {
            if (!CanFire) return false;
            var projectile = ProjectileFactory.Create(Wielder, origin, direction);
            if (projectile == null) return false;
            // Copy on-hit actions from the factory into the projectile so each
            // shot carries its own (mutable) list — item effects can change
            // future shots without rewriting the in-flight one.
            projectile.OnHitActions.Clear();
            for (int i = 0; i < ProjectileFactory.OnHitActions.Count; i++)
            {
                projectile.OnHitActions.Add(ProjectileFactory.OnHitActions[i]);
            }
            projectile.Launch(origin, direction, speed: 20f);
            _readyAt = Time.time + Cooldown;
            return true;
        }
    }
}
