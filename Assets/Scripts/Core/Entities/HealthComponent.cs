using System;
using UnityEngine;

namespace OneTry.Core.Entities
{
    public interface IDamageReceiver
    {
        void ReceiveDamage(float amount, IEntity source);
        void ReceiveHealing(float amount, IEntity source);
    }

    /// <summary>
    /// Health module. The only part of the framework that knows about death.
    /// Other components subscribe to <see cref="Died"/> to react.
    /// </summary>
    public sealed class HealthComponent : MonoBehaviour, IDamageReceiver
    {
        [SerializeField] private float _maxHealth = 100f;

        private Resource _resource;

        public Resource Resource => _resource ??= new Resource(_maxHealth);
        public bool IsAlive => !Resource.IsEmpty;

        public event Action<float, IEntity> DamageTaken;
        public event Action<float, IEntity> HealingReceived;
        public event Action<IEntity> Died;

        private void Awake()
        {
            _resource ??= new Resource(_maxHealth);
        }

        public void ReceiveDamage(float amount, IEntity source)
        {
            if (amount <= 0f || !IsAlive) return;
            float dealt = Resource.Subtract(amount);
            if (dealt > 0f) DamageTaken?.Invoke(dealt, source);
            if (Resource.IsEmpty) Died?.Invoke(source);
        }

        public void ReceiveHealing(float amount, IEntity source)
        {
            if (amount <= 0f || !IsAlive) return;
            float healed = Resource.Add(amount);
            if (healed > 0f) HealingReceived?.Invoke(healed, source);
        }
    }
}
