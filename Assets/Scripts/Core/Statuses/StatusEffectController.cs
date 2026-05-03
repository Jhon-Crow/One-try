using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Statuses
{
    /// <summary>
    /// Per-creature status registry. Ticks active effects, removes expired,
    /// and aggregates modifiers for combat queries.
    /// </summary>
    public sealed class StatusEffectController : MonoBehaviour
    {
        private readonly List<IStatusEffect> _effects = new();
        private Creature _host;

        public IReadOnlyList<IStatusEffect> Active => _effects;

        public void Bind(Creature host) => _host = host;

        public void Apply(IStatusEffect effect)
        {
            if (effect == null || _host == null) return;
            _effects.Add(effect);
            effect.OnApplied(_host);
        }

        public bool Remove(IStatusEffect effect)
        {
            if (effect == null) return false;
            if (!_effects.Remove(effect)) return false;
            effect.OnRemoved(_host);
            return true;
        }

        public bool RemoveById(string id)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i].Id == id)
                {
                    var e = _effects[i];
                    _effects.RemoveAt(i);
                    e.OnRemoved(_host);
                    return true;
                }
            }
            return false;
        }

        public void Tick(float deltaTime)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                var e = _effects[i];
                e.OnTick(_host, deltaTime);
                if (e.IsExpired)
                {
                    _effects.RemoveAt(i);
                    e.OnRemoved(_host);
                }
            }
        }

        private void Update()
        {
            if (_effects.Count > 0) Tick(Time.deltaTime);
        }

        public float GetMultiplier(ModifierKind kind, float baseValue = 1f)
        {
            float mult = baseValue;
            for (int i = 0; i < _effects.Count; i++)
            {
                var mods = _effects[i].Modifiers;
                for (int j = 0; j < mods.Count; j++)
                {
                    if (mods[j].Kind == kind) mult *= mods[j].Value;
                }
            }
            return mult;
        }

        public float GetFlat(ModifierKind kind)
        {
            float sum = 0f;
            for (int i = 0; i < _effects.Count; i++)
            {
                var mods = _effects[i].Modifiers;
                for (int j = 0; j < mods.Count; j++)
                {
                    if (mods[j].Kind == kind) sum += mods[j].Value;
                }
            }
            return sum;
        }
    }
}
