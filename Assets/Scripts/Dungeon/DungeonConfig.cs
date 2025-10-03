using UnityEngine;

namespace ChronoDepths.Dungeon
{
    /// <summary>
    /// Scriptable configuration for procedural dungeon generation.
    /// Designers can create multiple assets to tweak prototype parameters.
    /// </summary>
    [CreateAssetMenu(menuName = "ChronoDepths/Dungeon Config", fileName = "DungeonConfig")]
    public sealed class DungeonConfig : ScriptableObject
    {
        [Header("Layout")]
        [SerializeField, Tooltip("Number of rooms to spawn in the generated dungeon.")]
        private int roomBudget = 12;

        [SerializeField, Tooltip("Size of the grid the random walk is allowed to explore.")]
        private Vector2Int gridSize = new Vector2Int(8, 8);

        [Header("Room Prefabs")]
        [SerializeField, Tooltip("Fallback room prefab used when no specialised prefab is available.")]
        private GameObject defaultRoomPrefab;

        [SerializeField, Tooltip("Optional special room prefabs that can be used for key encounters.")]
        private GameObject[] specialRoomPrefabs = System.Array.Empty<GameObject>();

        [Header("Generation Seed")]
        [SerializeField, Tooltip("Leave at 0 to randomise the seed on each generation.")]
        private int seed;

        public int RoomBudget => Mathf.Max(1, roomBudget);
        public Vector2Int GridSize => new Vector2Int(Mathf.Max(2, gridSize.x), Mathf.Max(2, gridSize.y));
        public GameObject DefaultRoomPrefab => defaultRoomPrefab;
        public GameObject[] SpecialRoomPrefabs => specialRoomPrefabs;
        public int Seed => seed;
    }
}
