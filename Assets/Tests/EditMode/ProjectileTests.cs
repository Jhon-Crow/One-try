using NUnit.Framework;
using OneTry.Core.Combat;
using OneTry.Core.Combat.HitActions;
using OneTry.Core.Combat.Projectiles;
using OneTry.Core.Entities;
using OneTry.Core.Statuses.Effects;
using UnityEngine;

namespace OneTry.Tests.EditMode
{
    public sealed class ProjectileTests
    {
        private static Creature Make(string n, Faction f = Faction.Neutral, float hp = 100f)
        {
            var go = new GameObject(n);
            var c = go.AddComponent<Creature>();
            c.Faction = f;
            if (c.Health != null) c.Health.Resource.SetMaximum(hp, refill: true);
            return c;
        }

        [Test]
        public void EnemyFiresAnotherEnemy_AsProjectile_Hits()
        {
            // R5: a creature is a valid projectile.
            // Scenario from the issue: enemy fires another enemy at a target.
            var shooter = Make("Shooter", Faction.Hostile);
            var ammo    = Make("AmmoCreature", Faction.Hostile);
            var target  = Make("Target", Faction.Player, hp: 50f);

            var factory = new CreatureProjectileFactory(() => ammo);
            factory.OnHitActions.Add(new DamageAction(20f));

            var rifle = new Weapon("CreatureLauncher", factory, cooldown: 0f);
            shooter.Weapons.Attach(0, rifle);

            var projectile = factory.Create(shooter, shooter.transform.position, Vector3.forward);
            Assert.IsNotNull(projectile);
            Assert.IsInstanceOf<CreatureAsProjectile>(projectile);

            // Manually resolve the hit (no physics in EditMode).
            var asCreatureProjectile = (CreatureAsProjectile)projectile;
            asCreatureProjectile.OnHitActions.Add(new DamageAction(20f));
            asCreatureProjectile.ResolveHit(target);

            Assert.AreEqual(30f, target.Health.Resource.Current);

            Object.DestroyImmediate(shooter.gameObject);
            Object.DestroyImmediate(target.gameObject);
            // ammo is destroyed by Unity when its GameObject was reused as the
            // projectile body — no extra cleanup.
            if (ammo != null) Object.DestroyImmediate(ammo.gameObject);
        }

        [Test]
        public void HealingBullets_HealEnemy_InsteadOfHurting()
        {
            // R6+R7 combined: an enemy steals the player's weapon, but the
            // player's item effect made the bullets healing — the enemy
            // ends up healing its own teammate when it fires.
            var player    = Make("Player",  Faction.Player);
            var enemyA    = Make("EnemyA",  Faction.Hostile);
            var teammate  = Make("EnemyB",  Faction.Hostile, hp: 100f);

            var factory = new SimpleProjectileFactory();
            var rifle   = new Weapon("Rifle", factory, cooldown: 0f);
            player.Weapons.Attach(0, rifle);

            // Player triggers item effect — bullets become healing.
            factory.OnHitActions.Add(new HealAction(40f));

            // Enemy steals the rifle.
            player.Weapons.TransferTo(0, enemyA.Weapons, 0);
            Assert.AreSame(rifle, enemyA.Weapons.GetSlot(0));

            // Damage teammate first so we have room to heal.
            teammate.Health.ReceiveDamage(60f, source: null);
            Assert.AreEqual(40f, teammate.Health.Resource.Current);

            // Build a hit on the teammate (simulated trigger).
            var ctx = new HitContext(enemyA, teammate.transform.position);
            for (int i = 0; i < factory.OnHitActions.Count; i++)
                ctx.OnHitActions.Add(factory.OnHitActions[i]);
            teammate.OnHit(ctx);

            Assert.AreEqual(80f, teammate.Health.Resource.Current);

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(enemyA.gameObject);
            Object.DestroyImmediate(teammate.gameObject);
        }

        [Test]
        public void ApplyStatusAction_LandsStatusOnHit()
        {
            var attacker = Make("A");
            var victim   = Make("V");

            var ctx = new HitContext(attacker, victim.transform.position);
            ctx.OnHitActions.Add(new ApplyStatusAction(() =>
                new BleedEffect(damagePerSecond: 5f, duration: 1f)));
            victim.OnHit(ctx);

            Assert.AreEqual(1, victim.Statuses.Active.Count);
            Assert.AreEqual("Bleed", victim.Statuses.Active[0].Id);

            Object.DestroyImmediate(attacker.gameObject);
            Object.DestroyImmediate(victim.gameObject);
        }
    }
}
