namespace OneTry.Core.Combat
{
    /// <summary>
    /// Atomic effect that runs when a projectile (or melee swing) connects
    /// with a target. Stack of actions is mutable so item effects can
    /// rewrite it (e.g. "bullets become healing").
    /// </summary>
    public interface IOnHitAction
    {
        void Apply(HitContext context);
    }
}
