using System.Collections.Generic;
using UnityEngine;

namespace ChronoDepths.Cards
{
    /// <summary>
    /// Lightweight deck implementation that can draw cards, discard them and reshuffle when exhausted.
    /// This will be expanded later to interact with combat states and UI widgets.
    /// </summary>
    public sealed class CardDeckController : MonoBehaviour
    {
        [SerializeField]
        private List<CardData> startingDeck = new();

        [SerializeField]
        private int handSize = 5;

        private readonly List<CardData> drawPile = new();
        private readonly List<CardData> hand = new();
        private readonly List<CardData> discardPile = new();
        private System.Random rng;

        public IReadOnlyList<CardData> Hand => hand;
        public IReadOnlyCollection<CardData> DrawPile => drawPile;
        public IReadOnlyCollection<CardData> DiscardPile => discardPile;

        public void InitialiseDeck(int? seed = null)
        {
            drawPile.Clear();
            discardPile.Clear();
            hand.Clear();

            drawPile.AddRange(startingDeck);
            rng = new System.Random(seed ?? Random.Range(int.MinValue, int.MaxValue));
            Shuffle(drawPile);
            DrawToFullHand();
        }

        public void DrawToFullHand()
        {
            while (hand.Count < handSize)
            {
                if (!TryDrawOne())
                {
                    break;
                }
            }
        }

        public bool TryDrawOne()
        {
            if (drawPile.Count == 0)
            {
                if (discardPile.Count == 0)
                {
                    return false;
                }

                drawPile.AddRange(discardPile);
                discardPile.Clear();
                Shuffle(drawPile);
            }

            int index = drawPile.Count - 1;
            CardData card = drawPile[index];
            drawPile.RemoveAt(index);
            hand.Add(card);
            return true;
        }

        public bool TryPlayCard(CardData card)
        {
            int handIndex = hand.IndexOf(card);
            if (handIndex < 0)
            {
                return false;
            }

            hand.RemoveAt(handIndex);
            discardPile.Add(card);
            return true;
        }

        public void DiscardHand()
        {
            discardPile.AddRange(hand);
            hand.Clear();
        }

        private void Shuffle(List<CardData> pile)
        {
            for (int i = pile.Count - 1; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                (pile[i], pile[swapIndex]) = (pile[swapIndex], pile[i]);
            }
        }
    }
}
