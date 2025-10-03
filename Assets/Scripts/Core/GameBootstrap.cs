using UnityEngine;

namespace ChronoDepths.Core
{
    /// <summary>
    /// Simple bootstrapper that wires together the main gameplay systems in the scene.
    /// In early prototypes this component is intended to live on an empty GameObject
    /// and expose fields for assigning the other managers via the inspector.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField]
        private Dungeon.DungeonGenerator dungeonGenerator;

        [SerializeField]
        private TimeSystem.TimeController timeController;

        [SerializeField]
        private Cards.CardDeckController cardDeckController;

        [SerializeField]
        private Equipment.EquipmentLoadout equipmentLoadout;

        private bool hasGeneratedDungeon;

        private void Awake()
        {
            if (dungeonGenerator == null)
            {
                Debug.LogWarning("GameBootstrap is missing a DungeonGenerator reference.");
            }

            if (timeController == null)
            {
                Debug.LogWarning("GameBootstrap is missing a TimeController reference.");
            }

            if (cardDeckController == null)
            {
                Debug.LogWarning("GameBootstrap is missing a CardDeckController reference.");
            }

            if (equipmentLoadout == null)
            {
                Debug.LogWarning("GameBootstrap is missing an EquipmentLoadout reference.");
            }
        }

        private void Start()
        {
            if (dungeonGenerator != null && !hasGeneratedDungeon)
            {
                dungeonGenerator.GenerateDungeon();
                hasGeneratedDungeon = true;
            }

            if (timeController != null)
            {
                timeController.BeginExplorationPhase();
            }

            if (cardDeckController != null)
            {
                cardDeckController.InitialiseDeck();
            }

            if (equipmentLoadout != null)
            {
                equipmentLoadout.ApplyLoadoutModifiers();
            }
        }
    }
}
