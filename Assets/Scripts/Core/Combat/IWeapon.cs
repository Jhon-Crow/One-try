using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat
{
    public interface IWeaponWielder
    {
        WeaponHolder Weapons { get; }
    }

    /// <summary>
    /// Anything a creature can carry and trigger to produce projectiles.
    /// The wielder is whoever currently holds the weapon (mutable).
    /// </summary>
    public interface IWeapon
    {
        string Name { get; }
        Creature Wielder { get; set; }
        IProjectileFactory ProjectileFactory { get; set; }
        float Cooldown { get; }
        bool CanFire { get; }
        bool TryFire(Vector3 origin, Vector3 direction);
    }
}
