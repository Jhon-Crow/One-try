using OneTry.Core.Statuses;

namespace OneTry.Core.Combat.HitActions
{
    public sealed class DamageAction : IOnHitAction
    {
        public float Amount;

        public DamageAction(float amount) => Amount = amount;

        public void Apply(HitContext context)
        {
            if (context?.Victim == null || context.Victim.Health == null) return;
            float final = Amount;
            if (context.Victim.Statuses != null)
            {
                final *= context.Victim.Statuses.GetMultiplier(ModifierKind.DamageTakenMultiplier);
                final += context.Victim.Statuses.GetFlat(ModifierKind.DamageTakenFlat);
            }
            if (final < 0f) final = 0f;
            context.DamageAmount += final;
            context.Victim.Health.ReceiveDamage(final, context.Source);
        }
    }
}
