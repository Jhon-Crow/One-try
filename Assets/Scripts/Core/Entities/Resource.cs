using System;

namespace OneTry.Core.Entities
{
    /// <summary>
    /// Plain-C# bounded resource (health, stamina, energy). MonoBehaviour-free
    /// so it can be unit-tested without a GameObject and reused inside any
    /// component (Creature, Projectile, Weapon-as-creature, etc.).
    /// </summary>
    [Serializable]
    public sealed class Resource
    {
        private float _max;
        private float _current;

        public float Maximum => _max;
        public float Current => _current;
        public bool IsEmpty => _current <= 0f;
        public bool IsFull => _current >= _max;

        public event Action<float, float> Changed;

        public Resource(float maximum, float? current = null)
        {
            if (maximum <= 0f) throw new ArgumentOutOfRangeException(nameof(maximum));
            _max = maximum;
            _current = current ?? maximum;
            if (_current < 0f) _current = 0f;
            if (_current > _max) _current = _max;
        }

        public float Add(float delta)
        {
            float before = _current;
            _current = Math.Min(_max, _current + Math.Max(0f, delta));
            float diff = _current - before;
            if (diff != 0f) Changed?.Invoke(_current, _max);
            return diff;
        }

        public float Subtract(float delta)
        {
            float before = _current;
            _current = Math.Max(0f, _current - Math.Max(0f, delta));
            float diff = before - _current;
            if (diff != 0f) Changed?.Invoke(_current, _max);
            return diff;
        }

        public void SetMaximum(float newMax, bool refill = false)
        {
            if (newMax <= 0f) throw new ArgumentOutOfRangeException(nameof(newMax));
            _max = newMax;
            if (refill) _current = _max;
            else if (_current > _max) _current = _max;
            Changed?.Invoke(_current, _max);
        }

        public void Reset() => Add(_max);
    }
}
