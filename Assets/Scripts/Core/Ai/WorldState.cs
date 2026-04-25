using System.Collections.Generic;

namespace OneTry.Core.Ai
{
    public sealed class WorldState : IWorldState
    {
        private readonly Dictionary<string, object> _data;

        public WorldState() { _data = new Dictionary<string, object>(); }
        public WorldState(IReadOnlyDictionary<string, object> seed)
        {
            _data = new Dictionary<string, object>(seed);
        }

        public bool TryGet(string key, out object value) => _data.TryGetValue(key, out value);
        public void Set(string key, object value) => _data[key] = value;

        public IWorldState Clone() => new WorldState(_data);

        public bool Matches(IReadOnlyDictionary<string, object> conditions)
        {
            if (conditions == null || conditions.Count == 0) return true;
            foreach (var kv in conditions)
            {
                if (!_data.TryGetValue(kv.Key, out var have)) return false;
                if (!Equals(have, kv.Value)) return false;
            }
            return true;
        }

        public override string ToString()
        {
            var parts = new List<string>(_data.Count);
            foreach (var kv in _data) parts.Add($"{kv.Key}={kv.Value}");
            return "{" + string.Join(", ", parts) + "}";
        }
    }
}
