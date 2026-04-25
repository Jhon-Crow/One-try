using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    /// <summary>
    /// One step in a plan. Has preconditions (must be true to execute),
    /// effects (applied to the world state once executed), and a cost
    /// (used by the A* planner to pick cheaper plans).
    /// </summary>
    public interface IAction
    {
        string Name { get; }
        float Cost { get; }
        IReadOnlyDictionary<string, object> Preconditions { get; }
        IReadOnlyDictionary<string, object> Effects { get; }
        bool IsExecutable(IWorldState world);
        IWorldState ApplyEffects(IWorldState world);
    }

    /// <summary>
    /// Plain-data action with declarative preconditions and effects.
    /// Concrete actions either subclass this or implement IAction directly
    /// and override <c>IsExecutable</c> for runtime checks.
    /// </summary>
    public class Action : IAction
    {
        private readonly Dictionary<string, object> _pre;
        private readonly Dictionary<string, object> _eff;

        public string Name { get; }
        public float Cost { get; }
        public IReadOnlyDictionary<string, object> Preconditions => _pre;
        public IReadOnlyDictionary<string, object> Effects => _eff;

        public Action(string name, float cost,
            IReadOnlyDictionary<string, object> preconditions,
            IReadOnlyDictionary<string, object> effects)
        {
            Name = name;
            Cost = cost;
            _pre = preconditions != null
                ? new Dictionary<string, object>(preconditions)
                : new Dictionary<string, object>();
            _eff = effects != null
                ? new Dictionary<string, object>(effects)
                : new Dictionary<string, object>();
        }

        public virtual bool IsExecutable(IWorldState world) => world.Matches(_pre);

        public virtual IWorldState ApplyEffects(IWorldState world)
        {
            var clone = world.Clone();
            foreach (var kv in _eff) clone.Set(kv.Key, kv.Value);
            return clone;
        }
    }
}
