using System.Collections.Generic;
using OneTry.Core.Ai;
using OneTry.Core.Combat;
using OneTry.Core.Statuses;
using UnityEngine;

namespace OneTry.Core.Entities
{
    /// <summary>
    /// Single aggregate base for everything alive: player, enemy, NPC, summon,
    /// boss, even creature-as-projectile.
    ///
    /// Holds no game logic of its own — all behavior lives in pluggable
    /// modules (Health, Statuses, Weapons, Brain). Anything any creature can
    /// do, any other creature can do too: that is the entire point.
    /// </summary>
    [DisallowMultipleComponent]
    public class Creature : MonoBehaviour, IEntity, IStatusReceiver, IWeaponWielder, IHitTarget
    {
        private static int _nextId = 1;

        [SerializeField] private Faction _faction = Faction.Neutral;

        [SerializeField] private HealthComponent _health;
        [SerializeField] private StatusEffectController _statuses;
        [SerializeField] private WeaponHolder _weapons;
        [SerializeField] private AgentBrain _brain;

        private int _entityId;
        private readonly List<Resource> _resources = new();

        public int EntityId => _entityId;
        public Vector3 Position => transform.position;
        public Faction Faction { get => _faction; set => _faction = value; }
        public bool IsAlive => _health == null || _health.IsAlive;

        public HealthComponent Health => _health;
        public StatusEffectController Statuses => _statuses;
        public WeaponHolder Weapons => _weapons;
        public AgentBrain Brain => _brain;

        protected virtual void Awake()
        {
            _entityId = _nextId++;
            _health   ??= GetComponent<HealthComponent>()        ?? gameObject.AddComponent<HealthComponent>();
            _statuses ??= GetComponent<StatusEffectController>() ?? gameObject.AddComponent<StatusEffectController>();
            _weapons  ??= GetComponent<WeaponHolder>()           ?? gameObject.AddComponent<WeaponHolder>();
            _brain    ??= GetComponent<AgentBrain>();
            _statuses.Bind(this);
            _weapons.Bind(this);
            if (_brain != null) _brain.Bind(this);
        }

        public void RegisterResource(Resource resource)
        {
            if (resource == null) return;
            if (!_resources.Contains(resource)) _resources.Add(resource);
        }

        public IReadOnlyList<Resource> Resources => _resources;

        // IHitTarget — accept a HitContext and resolve all on-hit actions.
        public void OnHit(HitContext context)
        {
            if (!IsAlive || context == null) return;
            context.Victim = this;
            for (int i = 0; i < context.OnHitActions.Count; i++)
            {
                context.OnHitActions[i].Apply(context);
            }
        }
    }
}
