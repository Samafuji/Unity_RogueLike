using UnityEngine;

namespace ChronoDepths.Cards
{
    public enum CardRarity
    {
        Common,
        Rare,
        Legendary
    }

    /// <summary>
    /// Basic data representation for an ability card. The effect execution will be supplied by
    /// derived components or ability scripts in future milestones.
    /// </summary>
    [CreateAssetMenu(menuName = "ChronoDepths/Card", fileName = "CardData")]
    public sealed class CardData : ScriptableObject
    {
        [SerializeField]
        private string displayName;

        [SerializeField, TextArea]
        private string description;

        [SerializeField]
        private Sprite artwork;

        [SerializeField]
        private CardRarity rarity = CardRarity.Common;

        [SerializeField, Tooltip("Energy or resource cost to play the card during a run.")]
        private int cost = 1;

        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Artwork => artwork;
        public CardRarity Rarity => rarity;
        public int Cost => Mathf.Max(0, cost);
    }
}
