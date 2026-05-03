using OneTry.Core.Entities;

namespace OneTry.Core.Statuses.Effects
{
    public sealed class BleedEffect : StatusEffect
    {
        private readonly float _dps;
        private float _accumulated;

        public BleedEffect(float damagePerSecond, float duration)
            : base(id: "Bleed", duration: duration)
        {
            _dps = damagePerSecond;
        }

        public override void OnTick(Creature host, float deltaTime)
        {
            base.OnTick(host, deltaTime);
            _accumulated += _dps * deltaTime;
            if (_accumulated >= 1f && host.Health != null)
            {
                int whole = (int)_accumulated;
                _accumulated -= whole;
                host.Health.ReceiveDamage(whole, source: null);
            }
        }
    }
}
