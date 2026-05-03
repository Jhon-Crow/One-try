using System.Collections.Generic;
using OneTry.Core.Entities;
using UnityEngine;

namespace OneTry.Core.Combat
{
    /// <summary>
    /// Per-creature inventory of weapons. Slots are indexed; weapons can be
    /// detached and attached freely (R6: enemies stealing player weapons).
    /// </summary>
    public sealed class WeaponHolder : MonoBehaviour
    {
        [SerializeField] private int _slotCount = 4;

        private IWeapon[] _slots;
        private Creature _owner;

        public int SlotCount => _slotCount;
        public Creature Owner => _owner;

        public void Bind(Creature owner)
        {
            _owner = owner;
            _slots ??= new IWeapon[_slotCount];
        }

        private void Awake() => _slots ??= new IWeapon[_slotCount];

        public IWeapon GetSlot(int index)
        {
            if (_slots == null || index < 0 || index >= _slots.Length) return null;
            return _slots[index];
        }

        public bool Attach(int index, IWeapon weapon)
        {
            if (_slots == null || index < 0 || index >= _slots.Length || weapon == null) return false;
            _slots[index] = weapon;
            weapon.Wielder = _owner;
            return true;
        }

        public IWeapon Detach(int index)
        {
            if (_slots == null || index < 0 || index >= _slots.Length) return null;
            var w = _slots[index];
            if (w != null) w.Wielder = null;
            _slots[index] = null;
            return w;
        }

        /// <summary>
        /// Move a weapon from this holder's slot to another holder's slot.
        /// Used by abilities like "steal weapon".
        /// </summary>
        public bool TransferTo(int fromSlot, WeaponHolder other, int toSlot)
        {
            if (other == null) return false;
            var w = Detach(fromSlot);
            if (w == null) return false;
            return other.Attach(toSlot, w);
        }

        public IEnumerable<IWeapon> AllWeapons()
        {
            if (_slots == null) yield break;
            for (int i = 0; i < _slots.Length; i++)
                if (_slots[i] != null) yield return _slots[i];
        }
    }
}
