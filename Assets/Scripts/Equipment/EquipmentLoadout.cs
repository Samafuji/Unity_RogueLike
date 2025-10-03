using System.Collections.Generic;
using UnityEngine;

namespace ChronoDepths.Equipment
{
    /// <summary>
    /// Holds the player's currently equipped items and exposes combined stat modifiers. Future iterations
    /// can extend this with status effects, sockets and random affixes.
    /// </summary>
    public sealed class EquipmentLoadout : MonoBehaviour
    {
        [SerializeField]
        private EquipmentItem weapon;

        [SerializeField]
        private EquipmentItem armour;

        [SerializeField]
        private EquipmentItem accessory;

        public IReadOnlyList<EquipmentItem> EquippedItems
        {
            get
            {
                return new List<EquipmentItem>
                {
                    weapon,
                    armour,
                    accessory
                };
            }
        }

        public int PowerModifier => (weapon?.Power ?? 0) + (accessory?.Power ?? 0);
        public int DefenceModifier => (armour?.Defence ?? 0) + (accessory?.Defence ?? 0);
        public int FocusModifier => (weapon?.Focus ?? 0) + (armour?.Focus ?? 0) + (accessory?.Focus ?? 0);

        public void ApplyLoadoutModifiers()
        {
            Debug.Log($"Applying loadout: Power {PowerModifier}, Defence {DefenceModifier}, Focus {FocusModifier}");
        }

        public void EquipItem(EquipmentItem item)
        {
            switch (item.Slot)
            {
                case EquipmentSlot.Weapon:
                    weapon = item;
                    break;
                case EquipmentSlot.Armour:
                    armour = item;
                    break;
                case EquipmentSlot.Accessory:
                    accessory = item;
                    break;
            }
        }
    }
}
