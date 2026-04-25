using OneTry.Core.Entities;

namespace OneTry.Core.Statuses.Effects
{
    public sealed class HealOverTimeEffect : StatusEffect
    {
        private readonly float _hps;
        private float _accumulated;

        public HealOverTimeEffect(float healPerSecond, float duration)
            : base(id: "HoT", duration: duration)
        {
            _hps = healPerSecond;
        }

        public override void OnTick(Creature host, float deltaTime)
        {
            base.OnTick(host, deltaTime);
            _accumulated += _hps * deltaTime;
            if (_accumulated >= 1f && host.Health != null)
            {
                int whole = (int)_accumulated;
                _accumulated -= whole;
                host.Health.ReceiveHealing(whole, source: null);
            }
        }
    }
}
