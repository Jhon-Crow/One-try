using OneTry.Core.Entities;

namespace OneTry.Core.Statuses.Effects
{
    /// <summary>
    /// Temporarily forces the host's <see cref="Faction"/> to a chosen value
    /// (e.g. turn an enemy friendly). Original faction is restored when the
    /// effect expires.
    /// </summary>
    public sealed class FactionSwitchEffect : StatusEffect
    {
        private readonly Faction _newFaction;
        private Faction _saved;

        public FactionSwitchEffect(Faction newFaction, float duration)
            : base(id: $"Faction:{newFaction}", duration: duration)
        {
            _newFaction = newFaction;
        }

        public override void OnApplied(Creature host)
        {
            _saved = host.Faction;
            host.Faction = _newFaction;
        }

        public override void OnRemoved(Creature host)
        {
            host.Faction = _saved;
        }
    }
}
