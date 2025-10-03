using UnityEngine;

namespace ChronoDepths.Equipment
{
    public enum EquipmentSlot
    {
        Weapon,
        Armour,
        Accessory
    }

    [CreateAssetMenu(menuName = "ChronoDepths/Equipment Item", fileName = "EquipmentItem")]
    public sealed class EquipmentItem : ScriptableObject
    {
        [SerializeField]
        private string displayName;

        [SerializeField]
        private EquipmentSlot slot;

        [SerializeField, TextArea]
        private string description;

        [SerializeField]
        private int power;

        [SerializeField]
        private int defence;

        [SerializeField]
        private int focus;

        public string DisplayName => displayName;
        public EquipmentSlot Slot => slot;
        public string Description => description;
        public int Power => power;
        public int Defence => defence;
        public int Focus => focus;
    }
}
