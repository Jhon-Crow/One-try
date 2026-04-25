using System.Collections.Generic;
using OneTry.Core.Entities;

namespace OneTry.Core.Ai
{
    /// <summary>
    /// Anything that drives a creature: a player input handler, a scripted
    /// boss phase machine, or a GOAP planner. Lives on the same GameObject
    /// as the <see cref="Creature"/>.
    /// </summary>
    public interface IAgentBrain
    {
        Creature Owner { get; }
        IReadOnlyList<IAction> Actions { get; }
        IReadOnlyList<IGoal> Goals { get; }
        IWorldState WorldState { get; }
        Plan CurrentPlan { get; }

        void Tick(float deltaTime);
    }
}
