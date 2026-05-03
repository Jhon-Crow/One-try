using System.Collections.Generic;
using OneTry.Core.Entities;

namespace OneTry.Core.Statuses
{
    public interface IStatusReceiver
    {
        StatusEffectController Statuses { get; }
        Faction Faction { get; set; }
    }

    /// <summary>
    /// A status effect is a small object attached to a creature for some
    /// duration. It can mutate the creature's faction, modifiers, or apply
    /// per-tick damage/heal. Contract is intentionally small so any C# class
    /// or ScriptableObject can implement it.
    /// </summary>
    public interface IStatusEffect
    {
        string Id { get; }
        bool IsExpired { get; }
        IReadOnlyList<Modifier> Modifiers { get; }

        void OnApplied(Creature host);
        void OnTick(Creature host, float deltaTime);
        void OnRemoved(Creature host);
    }
}
