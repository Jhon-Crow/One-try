using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    /// <summary>
    /// Snapshot of the world that a planner reasons over. Keys are strings
    /// so external tools (sensors, scripts, designer-authored data) can
    /// extend it without recompiling the core.
    /// </summary>
    public interface IWorldState
    {
        bool TryGet(string key, out object value);
        IWorldState Clone();
        void Set(string key, object value);
        bool Matches(IReadOnlyDictionary<string, object> conditions);
    }
}
