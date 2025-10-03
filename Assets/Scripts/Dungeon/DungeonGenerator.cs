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
        private GameObject fallbackRoomPrefab;

        public IReadOnlyDictionary<Vector2Int, GameObject> SpawnedRooms => spawnedRooms;
        public IReadOnlyList<Vector2Int> TraversalOrder => traversalOrder;

        public void GenerateDungeon()
        {
            PrepareGeneration();

            Vector2Int currentPosition = Vector2Int.zero;
            SpawnRoom(currentPosition, ResolveRoomPrefab(0, GetRoomBudget()));

            int roomBudget = GetRoomBudget();
            for (int i = 1; i < roomBudget; i++)
            {
                Vector2Int nextPosition = StepToNextCoordinate(currentPosition);
                if (spawnedRooms.ContainsKey(nextPosition))
                {
                    currentPosition = nextPosition;
                    continue;
                }

                GameObject prefab = ResolveRoomPrefab(i, roomBudget);
                SpawnRoom(nextPosition, prefab);
                currentPosition = nextPosition;
            }

            Debug.Log($"DungeonGenerator finished. Spawned {spawnedRooms.Count} rooms.");
        }

        private void PrepareGeneration()
        {
            if (dungeonRoot == null)
            {
                dungeonRoot = transform;
            }

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
            int seed = config != null ? config.Seed : 0;
            seed = seed == 0 ? Random.Range(int.MinValue, int.MaxValue) : seed;
            pseudoRandom = new System.Random(seed);
        }

        private void SpawnRoom(Vector2Int gridPosition, GameObject prefab)
        {
            Vector3 worldPosition = new(gridPosition.x * 20f, 0f, gridPosition.y * 20f);
            GameObject roomInstance;
            if (prefab != null)
            {
                roomInstance = Instantiate(prefab, worldPosition, Quaternion.identity, dungeonRoot);
            }
            else
            {
                roomInstance = CreateFallbackRoomInstance(worldPosition);
            }
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
            Vector2Int gridExtents = GetGridSize();
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

        private GameObject ResolveRoomPrefab(int index, int totalCount)
        {
            GameObject prefab = null;
            if (config != null)
            {
                prefab = config.DefaultRoomPrefab;
                if (config.SpecialRoomPrefabs != null && config.SpecialRoomPrefabs.Length > 0)
                {
                    float progress = totalCount <= 1 ? 1f : (float)index / (totalCount - 1);
                    if (progress > 0.8f)
                    {
                        prefab = config.SpecialRoomPrefabs[pseudoRandom.Next(config.SpecialRoomPrefabs.Length)];
                    }
                }
            }

            return prefab != null ? prefab : GetFallbackRoomPrefab();
        }

        private int GetRoomBudget()
        {
            return config != null ? config.RoomBudget : 12;
        }

        private Vector2Int GetGridSize()
        {
            return config != null ? config.GridSize : new Vector2Int(8, 8);
        }

        private GameObject GetFallbackRoomPrefab()
        {
            if (fallbackRoomPrefab != null)
            {
                return fallbackRoomPrefab;
            }

            fallbackRoomPrefab = new GameObject("FallbackRoomPrefab");
            fallbackRoomPrefab.hideFlags = HideFlags.HideAndDontSave;

            const float roomSize = 12f;
            const float wallHeight = 3f;
            const float wallThickness = 0.5f;

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(fallbackRoomPrefab.transform, false);
            floor.transform.localScale = new Vector3(roomSize, 0.2f, roomSize);
            floor.transform.localPosition = new Vector3(0f, -0.1f, 0f);

            CreateWall(new Vector3(0f, wallHeight / 2f, roomSize / 2f - wallThickness / 2f), new Vector3(roomSize, wallHeight, wallThickness));
            CreateWall(new Vector3(0f, wallHeight / 2f, -roomSize / 2f + wallThickness / 2f), new Vector3(roomSize, wallHeight, wallThickness));
            CreateWall(new Vector3(roomSize / 2f - wallThickness / 2f, wallHeight / 2f, 0f), new Vector3(wallThickness, wallHeight, roomSize));
            CreateWall(new Vector3(-roomSize / 2f + wallThickness / 2f, wallHeight / 2f, 0f), new Vector3(wallThickness, wallHeight, roomSize));

            return fallbackRoomPrefab;

            void CreateWall(Vector3 localPosition, Vector3 localScale)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "Wall";
                wall.transform.SetParent(fallbackRoomPrefab.transform, false);
                wall.transform.localPosition = localPosition;
                wall.transform.localScale = localScale;
            }
        }

        private GameObject CreateFallbackRoomInstance(Vector3 worldPosition)
        {
            GameObject template = GetFallbackRoomPrefab();
            GameObject instance = Instantiate(template, worldPosition, Quaternion.identity, dungeonRoot);
            instance.name = $"ProceduralRoom_{worldPosition.x}_{worldPosition.z}";
            foreach (Transform child in instance.transform)
            {
                if (child.TryGetComponent<MeshRenderer>(out var renderer))
                {
                    renderer.material.color = new Color(0.35f, 0.55f, 0.75f);
                }
            }
            return instance;
        }
    }
}
