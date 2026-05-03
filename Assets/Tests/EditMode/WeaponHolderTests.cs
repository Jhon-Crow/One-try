using System.Collections.Generic;
using NUnit.Framework;
using OneTry.Core.Combat;
using OneTry.Core.Combat.HitActions;
using OneTry.Core.Combat.Projectiles;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Tests.EditMode
{
    public sealed class WeaponHolderTests
    {
        private static Creature Make(string n)
        {
            var go = new GameObject(n);
            return go.AddComponent<Creature>();
        }

        [Test]
        public void AnyCreature_CanWieldAnyWeapon()
        {
            // R3: weapons aren't bound to creature subclass.
            var player = Make("Player");
            var enemy  = Make("Enemy");
            var rifle  = new Weapon("Rifle", new SimpleProjectileFactory());

            Assert.IsTrue(player.Weapons.Attach(0, rifle));
            Assert.AreSame(rifle, player.Weapons.GetSlot(0));
            Assert.AreSame(player, rifle.Wielder);

            // Detach + give to enemy → wielder follows.
            player.Weapons.TransferTo(fromSlot: 0, other: enemy.Weapons, toSlot: 0);
            Assert.IsNull(player.Weapons.GetSlot(0));
            Assert.AreSame(rifle, enemy.Weapons.GetSlot(0));
            Assert.AreSame(enemy, rifle.Wielder);

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(enemy.gameObject);
        }

        [Test]
        public void Weapon_FactoryAndOnHitActions_AreSwappableAtRuntime()
        {
            // R4 / R7: bullets become healing.
            var player = Make("Player");
            var enemy  = Make("Enemy");
            var factory = new SimpleProjectileFactory();
            var rifle = new Weapon("Rifle", factory);
            player.Weapons.Attach(0, rifle);

            // By default — damage on hit.
            factory.OnHitActions.Add(new DamageAction(10f));

            // Player triggers an item effect — wipe damage actions, install heal.
            factory.OnHitActions.Clear();
            factory.OnHitActions.Add(new HealAction(15f));

            // Simulate hit by building a HitContext directly.
            var ctx = new HitContext(player, enemy.transform.position);
            for (int i = 0; i < factory.OnHitActions.Count; i++)
                ctx.OnHitActions.Add(factory.OnHitActions[i]);

            float beforeHp = enemy.Health.Resource.Current;
            enemy.OnHit(ctx);
            // Healing on a full-HP creature does nothing visible — hit a damaged
            // one to actually observe the heal.
            enemy.Health.ReceiveDamage(50f, source: null);
            float midHp = enemy.Health.Resource.Current;

            ctx = new HitContext(player, enemy.transform.position);
            for (int i = 0; i < factory.OnHitActions.Count; i++)
                ctx.OnHitActions.Add(factory.OnHitActions[i]);
            enemy.OnHit(ctx);

            Assert.Greater(enemy.Health.Resource.Current, midHp);
            Assert.AreEqual(beforeHp, enemy.Health.Resource.Maximum);

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(enemy.gameObject);
        }
    }
}
