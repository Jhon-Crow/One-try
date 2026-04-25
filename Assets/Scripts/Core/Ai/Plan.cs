using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    public sealed class Plan
    {
        public IGoal Goal { get; }
        public IReadOnlyList<IAction> Steps { get; }
        public float TotalCost { get; }

        public Plan(IGoal goal, IReadOnlyList<IAction> steps, float cost)
        {
            Goal = goal;
            Steps = steps;
            TotalCost = cost;
        }

        public bool IsEmpty => Steps == null || Steps.Count == 0;

        public override string ToString()
        {
            if (IsEmpty) return "<empty plan>";
            var names = new List<string>(Steps.Count);
            for (int i = 0; i < Steps.Count; i++) names.Add(Steps[i].Name);
            return string.Join(" → ", names);
        }
    }
}
