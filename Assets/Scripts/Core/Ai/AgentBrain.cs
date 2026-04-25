using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Ai
{
    /// <summary>
    /// MonoBehaviour wrapper around a planner. Holds a list of goals and
    /// actions, picks the highest-priority valid goal, asks the planner
    /// for a plan, and executes the steps.
    ///
    /// Concrete behaviors (player controller, enemy script, boss FSM) plug
    /// in by populating <see cref="Goals"/>/<see cref="Actions"/> in
    /// <c>Awake</c> or via the Editor.
    /// </summary>
    public class AgentBrain : MonoBehaviour, IAgentBrain
    {
        [SerializeField] private float _replanInterval = 0.5f;
        [SerializeField] private int _maxPlanDepth = 12;

        private readonly List<IAction> _actions = new();
        private readonly List<IGoal> _goals = new();
        private readonly WorldState _world = new();

        private IPlanner _planner = new Planner();
        private Plan _plan;
        private int _stepIndex;
        private float _replanTimer;
        private Creature _owner;

        public Creature Owner => _owner;
        public IReadOnlyList<IAction> Actions => _actions;
        public IReadOnlyList<IGoal> Goals => _goals;
        public IWorldState WorldState => _world;
        public Plan CurrentPlan => _plan;

        public void Bind(Creature owner) => _owner = owner;

        public void RegisterAction(IAction action)
        {
            if (action != null && !_actions.Contains(action)) _actions.Add(action);
        }

        public void RegisterGoal(IGoal goal)
        {
            if (goal != null && !_goals.Contains(goal)) _goals.Add(goal);
        }

        public void SetPlanner(IPlanner planner) => _planner = planner ?? new Planner();

        public void SetFact(string key, object value) => _world.Set(key, value);

        public void Replan()
        {
            _plan = null;
            _stepIndex = 0;
            IGoal best = null;
            float bestPriority = float.MinValue;
            for (int i = 0; i < _goals.Count; i++)
            {
                var g = _goals[i];
                if (!g.IsValid(_world)) continue;
                if (g.Priority > bestPriority)
                {
                    bestPriority = g.Priority;
                    best = g;
                }
            }
            if (best == null) return;
            _plan = _planner.Plan(_world, best, _actions, _maxPlanDepth);
        }

        public virtual void Tick(float deltaTime)
        {
            _replanTimer -= deltaTime;
            if (_plan == null || _plan.IsEmpty || _replanTimer <= 0f)
            {
                _replanTimer = _replanInterval;
                Replan();
            }
            if (_plan == null || _plan.IsEmpty) return;
            if (_stepIndex >= _plan.Steps.Count) return;

            var step = _plan.Steps[_stepIndex];
            // Default execution: assume the action is "instant" — concrete
            // brains override Tick to translate steps into multi-frame
            // actions (move-to, fire-weapon, etc.).
            if (step.IsExecutable(_world))
            {
                foreach (var kv in step.Effects) _world.Set(kv.Key, kv.Value);
                _stepIndex++;
            }
        }

        protected virtual void Update()
        {
            if (_owner != null) Tick(Time.deltaTime);
        }
    }
}
