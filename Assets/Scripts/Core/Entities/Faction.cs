namespace OneTry.Core.Entities
{
    public enum Faction
    {
        Neutral = 0,
        Player = 1,
        Hostile = 2,
        Friendly = 3,
        Wild = 4,
    }

    public static class FactionExtensions
    {
        public static bool IsHostileTo(this Faction self, Faction other)
        {
            if (self == Faction.Neutral || other == Faction.Neutral) return false;
            if (self == other) return false;
            if (self == Faction.Friendly || other == Faction.Friendly) return false;
            return true;
        }
    }
}
