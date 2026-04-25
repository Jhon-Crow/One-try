using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    public interface IPlanner
    {
        Plan Plan(IWorldState initial, IGoal goal, IReadOnlyList<IAction> available, int maxDepth = 12);
    }
}
