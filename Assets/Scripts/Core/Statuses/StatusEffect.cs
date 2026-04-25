using System.Collections.Generic;
using OneTry.Core.Entities;

namespace OneTry.Core.Statuses
{
    /// <summary>
    /// Convenience base for plain-C# status effects. Stores duration,
    /// modifiers, and a stable id. Subclasses override <see cref="OnTick"/>
    /// to do the actual work.
    /// </summary>
    public abstract class StatusEffect : IStatusEffect
    {
        private readonly List<Modifier> _modifiers = new();
        private float _remaining;

        public string Id { get; }
        public float Duration { get; }
        public bool IsExpired => Duration > 0f && _remaining <= 0f;
        public IReadOnlyList<Modifier> Modifiers => _modifiers;

        protected StatusEffect(string id, float duration)
        {
            Id = id ?? GetType().Name;
            Duration = duration;
            _remaining = duration;
        }

        protected void AddModifier(Modifier m) => _modifiers.Add(m);

        public virtual void OnApplied(Creature host) { }

        public virtual void OnTick(Creature host, float deltaTime)
        {
            if (Duration > 0f) _remaining -= deltaTime;
        }

        public virtual void OnRemoved(Creature host) { }
    }
}
