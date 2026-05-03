using System;
using OneTry.Core.Statuses;

namespace OneTry.Core.Combat.HitActions
{
    public sealed class ApplyStatusAction : IOnHitAction
    {
        private readonly Func<IStatusEffect> _factory;

        public ApplyStatusAction(Func<IStatusEffect> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void Apply(HitContext context)
        {
            if (context?.Victim == null || context.Victim.Statuses == null) return;
            var effect = _factory();
            if (effect != null) context.Victim.Statuses.Apply(effect);
        }
    }
}
