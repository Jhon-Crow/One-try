using NUnit.Framework;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Tests.EditMode
{
    public sealed class CreatureTests
    {
        private static Creature SpawnCreature(string name, Faction faction = Faction.Neutral, float hp = 100f)
        {
            var go = new GameObject(name);
            var c = go.AddComponent<Creature>();
            c.Faction = faction;
            // Force component wiring (Awake is auto-called when AddComponent runs in EditMode).
            // Reset health to a known value.
            if (c.Health != null) c.Health.Resource.SetMaximum(hp, refill: true);
            return c;
        }

        [Test]
        public void Creature_AggregatesStandardModules()
        {
            var c = SpawnCreature("Player");
            Assert.IsNotNull(c.Health);
            Assert.IsNotNull(c.Statuses);
            Assert.IsNotNull(c.Weapons);
            Assert.IsTrue(c.IsAlive);
            Object.DestroyImmediate(c.gameObject);
        }

        [Test]
        public void PlayerAndEnemy_ShareSameBaseClass()
        {
            // R1: a player and an enemy are both just Creatures with different
            // Faction values — no class hierarchy split is needed.
            var player = SpawnCreature("Player", Faction.Player);
            var enemy  = SpawnCreature("Enemy",  Faction.Hostile);

            Assert.IsInstanceOf<Creature>(player);
            Assert.IsInstanceOf<Creature>(enemy);
            Assert.AreEqual(player.GetType(), enemy.GetType());
            Assert.IsTrue(player.Faction.IsHostileTo(enemy.Faction));

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(enemy.gameObject);
        }

        [Test]
        public void Creature_TakesDamage_AndDies()
        {
            var c = SpawnCreature("Mob", hp: 10f);
            int dieCount = 0;
            c.Health.Died += _ => dieCount++;

            c.Health.ReceiveDamage(7f, source: null);
            Assert.AreEqual(3f, c.Health.Resource.Current);
            Assert.IsTrue(c.IsAlive);

            c.Health.ReceiveDamage(50f, source: null);
            Assert.AreEqual(0f, c.Health.Resource.Current);
            Assert.IsFalse(c.IsAlive);
            Assert.AreEqual(1, dieCount);

            Object.DestroyImmediate(c.gameObject);
        }
    }
}
