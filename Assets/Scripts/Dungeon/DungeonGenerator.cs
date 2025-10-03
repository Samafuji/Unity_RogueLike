using System.Collections.Generic;
using UnityEngine;

namespace ChronoDepths.Dungeon
{
    /// <summary>
    /// Very early procedural dungeon prototype using a random walk to place room prefabs on a 2D grid.
    /// The generator keeps track of occupied coordinates to avoid overlap and exposes the generated
    /// rooms for other systems to consume (AI spawners, minimap, etc.).
    /// </summary>
    public sealed class DungeonGenerator : MonoBehaviour
    {
        [SerializeField]
        private DungeonConfig config;

        [SerializeField]
        private Transform dungeonRoot;

        [SerializeField]
        private bool clearPrevious = true;

        private readonly Dictionary<Vector2Int, GameObject> spawnedRooms = new();
        private readonly List<Vector2Int> traversalOrder = new();
        private System.Random pseudoRandom;

        public IReadOnlyDictionary<Vector2Int, GameObject> SpawnedRooms => spawnedRooms;
        public IReadOnlyList<Vector2Int> TraversalOrder => traversalOrder;

        public void GenerateDungeon()
        {
            if (config == null)
            {
                Debug.LogError("DungeonGenerator requires a DungeonConfig to operate.");
                return;
            }

            if (config.DefaultRoomPrefab == null)
            {
                Debug.LogError("DungeonConfig is missing a default room prefab reference.");
                return;
            }

            PrepareGeneration();

            Vector2Int currentPosition = Vector2Int.zero;
            SpawnRoom(currentPosition, config.DefaultRoomPrefab);

            for (int i = 1; i < config.RoomBudget; i++)
            {
                Vector2Int nextPosition = StepToNextCoordinate(currentPosition);
                if (spawnedRooms.ContainsKey(nextPosition))
                {
                    currentPosition = nextPosition;
                    continue;
                }

                GameObject prefab = SelectRoomPrefab(i, config.RoomBudget);
                SpawnRoom(nextPosition, prefab);
                currentPosition = nextPosition;
            }

            Debug.Log($"DungeonGenerator finished. Spawned {spawnedRooms.Count} rooms.");
        }

        private void PrepareGeneration()
        {
            if (clearPrevious)
            {
                foreach (Transform child in dungeonRoot)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }

            spawnedRooms.Clear();
            traversalOrder.Clear();
            int seed = config.Seed;
            seed = seed == 0 ? Random.Range(int.MinValue, int.MaxValue) : seed;
            pseudoRandom = new System.Random(seed);
        }

        private void SpawnRoom(Vector2Int gridPosition, GameObject prefab)
        {
            Vector3 worldPosition = new(gridPosition.x * 20f, 0f, gridPosition.y * 20f);
            GameObject roomInstance = Instantiate(prefab, worldPosition, Quaternion.identity, dungeonRoot);
            spawnedRooms.Add(gridPosition, roomInstance);
            traversalOrder.Add(gridPosition);
        }

        private Vector2Int StepToNextCoordinate(Vector2Int current)
        {
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            Vector2Int candidate;
            Vector2Int gridExtents = config.GridSize;
            int guard = 0;
            do
            {
                guard++;
                Vector2Int direction = directions[pseudoRandom.Next(directions.Length)];
                candidate = current + direction;
                candidate.x = Mathf.Clamp(candidate.x, -gridExtents.x / 2, gridExtents.x / 2);
                candidate.y = Mathf.Clamp(candidate.y, -gridExtents.y / 2, gridExtents.y / 2);
            } while (guard < 32 && spawnedRooms.ContainsKey(candidate));

            return candidate;
        }

        private GameObject SelectRoomPrefab(int index, int totalCount)
        {
            if (config.SpecialRoomPrefabs == null || config.SpecialRoomPrefabs.Length == 0)
            {
                return config.DefaultRoomPrefab;
            }

            float progress = (float)index / totalCount;
            if (progress > 0.8f)
            {
                return config.SpecialRoomPrefabs[pseudoRandom.Next(config.SpecialRoomPrefabs.Length)];
            }

            return config.DefaultRoomPrefab;
        }
    }
}
