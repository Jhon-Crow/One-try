using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    /// <summary>
    /// Minimal A*-style GOAP planner. Searches the (world-state, action)
    /// graph from <paramref name="initial"/> until it finds a state
    /// satisfying the goal's desired state, bounded by
    /// <paramref name="maxDepth"/> to keep the search finite.
    ///
    /// Heuristic: number of unsatisfied goal keys. Cost: action.Cost.
    /// Returns an empty plan if no plan is found within the budget.
    /// </summary>
    public sealed class Planner : IPlanner
    {
        private sealed class Node
        {
            public IWorldState State;
            public IAction From;
            public Node Parent;
            public float G;
            public float H;
            public int Depth;
            public float F => G + H;
        }

        public Plan Plan(IWorldState initial, IGoal goal,
            IReadOnlyList<IAction> available, int maxDepth = 12)
        {
            if (initial == null || goal == null || available == null)
                return new Plan(goal, new List<IAction>(), 0f);

            // Already satisfied?
            if (initial.Matches(goal.DesiredState))
                return new Plan(goal, new List<IAction>(), 0f);

            var open = new List<Node>
            {
                new Node
                {
                    State = initial,
                    G = 0f,
                    H = Heuristic(initial, goal),
                    Depth = 0,
                }
            };

            int safety = 0;
            const int maxIterations = 4096;

            while (open.Count > 0 && safety++ < maxIterations)
            {
                int bestIdx = 0;
                for (int i = 1; i < open.Count; i++)
                    if (open[i].F < open[bestIdx].F) bestIdx = i;
                var current = open[bestIdx];
                open.RemoveAt(bestIdx);

                if (current.State.Matches(goal.DesiredState))
                    return Reconstruct(current, goal);

                if (current.Depth >= maxDepth) continue;

                for (int i = 0; i < available.Count; i++)
                {
                    var a = available[i];
                    if (!a.IsExecutable(current.State)) continue;

                    var next = a.ApplyEffects(current.State);
                    var node = new Node
                    {
                        State = next,
                        From = a,
                        Parent = current,
                        G = current.G + a.Cost,
                        H = Heuristic(next, goal),
                        Depth = current.Depth + 1,
                    };
                    open.Add(node);
                }
            }

            return new Plan(goal, new List<IAction>(), 0f);
        }

        private static float Heuristic(IWorldState s, IGoal g)
        {
            int missing = 0;
            foreach (var kv in g.DesiredState)
            {
                if (!s.TryGet(kv.Key, out var have) || !Equals(have, kv.Value))
                    missing++;
            }
            return missing;
        }

        private static Plan Reconstruct(Node end, IGoal goal)
        {
            var steps = new List<IAction>();
            var cursor = end;
            while (cursor != null && cursor.From != null)
            {
                steps.Add(cursor.From);
                cursor = cursor.Parent;
            }
            steps.Reverse();
            return new Plan(goal, steps, end.G);
        }
    }
}
