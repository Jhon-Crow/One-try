using OneTry.Core.Statuses;

namespace OneTry.Core.Combat.HitActions
{
    public sealed class HealAction : IOnHitAction
    {
        public float Amount;

        public HealAction(float amount) => Amount = amount;

        public void Apply(HitContext context)
        {
            if (context?.Victim == null || context.Victim.Health == null) return;
            float final = Amount;
            if (context.Victim.Statuses != null)
            {
                final *= context.Victim.Statuses.GetMultiplier(ModifierKind.HealingMultiplier);
            }
            if (final < 0f) final = 0f;
            context.HealAmount += final;
            context.Victim.Health.ReceiveHealing(final, context.Source);
        }
    }
}
