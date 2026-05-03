using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    /// <summary>
    /// What an agent is trying to achieve. Goals are pure data: a set of
    /// (key, expected-value) pairs that the planner tries to satisfy.
    /// </summary>
    public interface IGoal
    {
        string Name { get; }
        float Priority { get; }
        IReadOnlyDictionary<string, object> DesiredState { get; }
        bool IsValid(IWorldState world);
    }

    public sealed class Goal : IGoal
    {
        public string Name { get; }
        public float Priority { get; }
        public IReadOnlyDictionary<string, object> DesiredState { get; }

        public Goal(string name, IReadOnlyDictionary<string, object> desired, float priority = 1f)
        {
            Name = name;
            Priority = priority;
            DesiredState = desired;
        }

        public bool IsValid(IWorldState world) => true;
    }
}
