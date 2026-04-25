using System.Collections.Generic;
using NUnit.Framework;
using OneTry.Core.Ai;

namespace OneTry.Tests.EditMode
{
    public sealed class PlannerTests
    {
        [Test]
        public void Planner_FindsTrivialPlan_OneAction()
        {
            var world = new WorldState();
            world.Set("hasWeapon", false);

            var pickUp = new Action(
                name: "PickUpWeapon",
                cost: 1f,
                preconditions: new Dictionary<string, object> { { "hasWeapon", false } },
                effects: new Dictionary<string, object>      { { "hasWeapon", true  } });

            var goal = new Goal("Armed",
                new Dictionary<string, object> { { "hasWeapon", true } });

            var plan = new Planner().Plan(world, goal, new List<IAction> { pickUp });

            Assert.IsFalse(plan.IsEmpty);
            Assert.AreEqual(1, plan.Steps.Count);
            Assert.AreEqual("PickUpWeapon", plan.Steps[0].Name);
            Assert.AreEqual(1f, plan.TotalCost);
        }

        [Test]
        public void Planner_FindsMultiStepPlan()
        {
            // World: nothing. Goal: enemyDead = true.
            var world = new WorldState();
            world.Set("hasWeapon", false);
            world.Set("nearEnemy", false);
            world.Set("enemyDead", false);

            var actions = new List<IAction>
            {
                new Action("PickUpWeapon", 1f,
                    new Dictionary<string, object> { { "hasWeapon", false } },
                    new Dictionary<string, object> { { "hasWeapon", true } }),
                new Action("MoveToEnemy", 2f,
                    new Dictionary<string, object> { { "nearEnemy", false } },
                    new Dictionary<string, object> { { "nearEnemy", true } }),
                new Action("AttackEnemy", 1f,
                    new Dictionary<string, object> { { "hasWeapon", true }, { "nearEnemy", true } },
                    new Dictionary<string, object> { { "enemyDead", true } }),
            };

            var goal = new Goal("KillEnemy",
                new Dictionary<string, object> { { "enemyDead", true } });

            var plan = new Planner().Plan(world, goal, actions);

            Assert.IsFalse(plan.IsEmpty);
            Assert.AreEqual(3, plan.Steps.Count);
            Assert.AreEqual("AttackEnemy", plan.Steps[2].Name);
        }

        [Test]
        public void Planner_ReturnsEmptyPlan_WhenUnreachable()
        {
            var world = new WorldState();
            var goal = new Goal("Impossible",
                new Dictionary<string, object> { { "miracle", true } });
            var plan = new Planner().Plan(world, goal, new List<IAction>());
            Assert.IsTrue(plan.IsEmpty);
        }

        [Test]
        public void Planner_AlreadySatisfied_ReturnsEmptyPlan()
        {
            var world = new WorldState();
            world.Set("alive", true);
            var goal = new Goal("StayAlive",
                new Dictionary<string, object> { { "alive", true } });
            var plan = new Planner().Plan(world, goal, new List<IAction>());
            Assert.IsTrue(plan.IsEmpty);
        }
    }
}
