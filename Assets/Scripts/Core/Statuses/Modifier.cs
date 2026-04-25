using System;

namespace OneTry.Core.Statuses
{
    public enum ModifierKind
    {
        DamageDealtMultiplier,
        DamageTakenMultiplier,
        HealingMultiplier,
        SpeedMultiplier,
        DamageDealtFlat,
        DamageTakenFlat,
    }

    /// <summary>
    /// Multiplicative or additive modifier that <see cref="StatusEffect"/>s
    /// can attach to their host <see cref="Creature"/>. Aggregated by the
    /// status controller and queried by combat systems.
    /// </summary>
    [Serializable]
    public sealed class Modifier
    {
        public ModifierKind Kind;
        public float Value;

        public Modifier(ModifierKind kind, float value)
        {
            Kind = kind;
            Value = value;
        }
    }
}
