using NUnit.Framework;
using OneTry.Core.Entities;
using OneTry.Core.Statuses;
using OneTry.Core.Statuses.Effects;
using UnityEngine;

namespace OneTry.Tests.EditMode
{
    public sealed class StatusEffectTests
    {
        private static Creature MakeCreature(Faction f = Faction.Neutral, float hp = 100f)
        {
            var go = new GameObject("Creature");
            var c = go.AddComponent<Creature>();
            c.Faction = f;
            if (c.Health != null) c.Health.Resource.SetMaximum(hp, refill: true);
            return c;
        }

        [Test]
        public void StatusEffect_AppliesToAnyCreature()
        {
            // R2: any status works on any creature.
            var player = MakeCreature(Faction.Player);
            var enemy  = MakeCreature(Faction.Hostile);

            var bleed1 = new BleedEffect(damagePerSecond: 5f, duration: 2f);
            var bleed2 = new BleedEffect(damagePerSecond: 5f, duration: 2f);

            player.Statuses.Apply(bleed1);
            enemy.Statuses.Apply(bleed2);

            Assert.AreEqual(1, player.Statuses.Active.Count);
            Assert.AreEqual(1, enemy.Statuses.Active.Count);

            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(enemy.gameObject);
        }

        [Test]
        public void BleedEffect_DamagesOverTime_AndExpires()
        {
            var c = MakeCreature(hp: 100f);
            c.Statuses.Apply(new BleedEffect(damagePerSecond: 10f, duration: 2f));

            // Tick 4 times of 0.5s = 2 seconds total → 20 damage, then expire.
            for (int i = 0; i < 4; i++) c.Statuses.Tick(0.5f);

            Assert.AreEqual(80f, c.Health.Resource.Current);
            Assert.AreEqual(0, c.Statuses.Active.Count);
            Object.DestroyImmediate(c.gameObject);
        }

        [Test]
        public void FactionSwitchEffect_FlipsFactionAndRestores()
        {
            // R7: an item effect can make an enemy friendly temporarily.
            var enemy = MakeCreature(Faction.Hostile);
            Assert.AreEqual(Faction.Hostile, enemy.Faction);

            enemy.Statuses.Apply(new FactionSwitchEffect(Faction.Friendly, duration: 1f));
            Assert.AreEqual(Faction.Friendly, enemy.Faction);

            // Tick past the duration.
            enemy.Statuses.Tick(1.5f);
            Assert.AreEqual(Faction.Hostile, enemy.Faction);
            Assert.AreEqual(0, enemy.Statuses.Active.Count);

            Object.DestroyImmediate(enemy.gameObject);
        }

        [Test]
        public void Modifiers_AggregateAcrossEffects()
        {
            var c = MakeCreature();
            // Two stacking effects each give x1.5 on damage taken.
            var e1 = new TestModifierEffect(ModifierKind.DamageTakenMultiplier, 1.5f);
            var e2 = new TestModifierEffect(ModifierKind.DamageTakenMultiplier, 1.5f);
            c.Statuses.Apply(e1);
            c.Statuses.Apply(e2);

            float mult = c.Statuses.GetMultiplier(ModifierKind.DamageTakenMultiplier);
            Assert.AreEqual(1.5f * 1.5f, mult, 1e-4);

            Object.DestroyImmediate(c.gameObject);
        }

        private sealed class TestModifierEffect : StatusEffect
        {
            public TestModifierEffect(ModifierKind kind, float value)
                : base(id: $"Mod:{kind}", duration: 0f)
            {
                AddModifier(new Modifier(kind, value));
            }
        }
    }
}
