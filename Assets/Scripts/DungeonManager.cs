using static EnumData;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    private Tile[,] map;
    private List<Vector2Int> spawnPoints;
    private List<Enemy> enemiesOnFloor = new List<Enemy>();
    private Player currentPlayer;
    public int currentFloor = 1;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;

    [Header("Tile Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    [Header("Parents")]
    public Transform tileParent;

    public InteractableObject chestPrefab;
    public InteractableObject exitPrefab;

    public int MinSpawnDistance = 4;

    public int MapWidth = 10;
    public int MapHeight = 10;

    private Dictionary<Vector2Int, InteractableObject> interactables = new Dictionary<Vector2Int, InteractableObject>();

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.dungeonManager = this;

            GenerateFloor(GameManager.Instance.CurrentFloor);
        }
        else
        {
            Debug.LogError("GameManager not found! Dungeon cannot be generated.");
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void GenerateFloor(int level)
    {
        Inventory InventoryUI = FindFirstObjectByType<Inventory>();

        ClearFloor();

        map = new Tile[MapWidth, MapHeight];
        spawnPoints = new List<Vector2Int>();
        currentFloor = level;

        if (tileParent == null)
        {
            tileParent = new GameObject("TileParent").transform;
        }

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                TileType type = (x == 0 || x == MapWidth - 1 || y == 0 || y == MapHeight - 1)
                                ? TileType.Wall
                                : TileType.Floor;

                map[x, y] = new Tile(new Vector2Int(x, y), type);

                GameObject tileToSpawn = (type == TileType.Wall) ? wallPrefab : floorPrefab;
                if (tileToSpawn != null)
                {
                    GameObject instance = Instantiate(tileToSpawn, new Vector3(x, y, 1), Quaternion.identity);
                    instance.transform.parent = tileParent;
                    instance.name = $"Tile_{type}_{x},{y}";
                }

                if (map[x, y].IsWalkable()) spawnPoints.Add(new Vector2Int(x, y));
            }
        }

        Player player = Player.Instance;

        SkillUIManager skillUI = FindFirstObjectByType<SkillUIManager>();
        if (skillUI != null)
        {
            skillUI.SetupUI(player);
            Debug.Log("[DM] SkillUIManager Setup completed.");
        }

        SpawnPlayer();

        InvokeSetupCurrencyUI();

        SpawnEnemies(level * 2);
    }

    private void SpawnPlayer()
    {
        currentPlayer = Player.Instance;
        Vector2Int playerSpawnPos = new Vector2Int(1, 1);

        if (currentPlayer != null)
        {
            currentPlayer.position = playerSpawnPos;
            currentPlayer.transform.position = new Vector3(playerSpawnPos.x, playerSpawnPos.y, -0.1f);
            Tile spawnTile = GetTile(playerSpawnPos.x, playerSpawnPos.y);
            if (spawnTile != null)
            {
                spawnTile.SetOccupied(true);
            }
            currentPlayer.highlighter?.ShowHighlights(currentPlayer);
            return;
        }

        GameObject playerObj = Instantiate(playerPrefab, new Vector3(playerSpawnPos.x, playerSpawnPos.y, -0.1f), Quaternion.identity);
        currentPlayer = playerObj.GetComponent<Player>();
        currentPlayer.position = playerSpawnPos;
        GetTile(playerSpawnPos.x, playerSpawnPos.y)?.SetOccupied(true);
        currentPlayer.name = "PLAYER";
    }

    public void SpawnEnemies(int count)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        enemiesOnFloor.Clear();

        for (int i = 0; i < count; i++)
        {
            Vector2Int spawnPos = GetValidEnemySpawnPosition(MinSpawnDistance);

            if (spawnPos != Vector2Int.zero)
            {
                GameObject enemyPrefabToUse = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                GameObject enemyObj = Instantiate(enemyPrefabToUse, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
                Enemy newEnemy = enemyObj.GetComponent<Enemy>();

                newEnemy.position = spawnPos;
                GetTile(spawnPos.x, spawnPos.y)?.SetOccupied(true);
                enemiesOnFloor.Add(newEnemy);
                newEnemy.name = $"ENEMY_{i}_{spawnPos.x},{spawnPos.y}";
            }
            else
            {
                Debug.LogWarning("Could not find a valid spawn position for an enemy.");
                break;
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (map == null || x < 0 || y < 0 || x >= MapWidth || y >= MapHeight) return null;
        return map[x, y];
    }

    public Player GetPlayer() => currentPlayer;
    public Character GetCharacterAtPosition(Vector2Int pos)
    {
        if (currentPlayer != null && currentPlayer.position == pos) return currentPlayer;
        return enemiesOnFloor.Find(e => e != null && e.position == pos);
    }

    public void ClearFloor()
    {
        if (currentPlayer != null) Destroy(currentPlayer.gameObject);
        currentPlayer = null;

        foreach (var enemy in enemiesOnFloor.ToList()) if (enemy != null) Destroy(enemy.gameObject);
        enemiesOnFloor.Clear();

        if (tileParent != null)
        {
            List<GameObject> childrenToDestroy = new List<GameObject>();
            foreach (Transform child in tileParent) childrenToDestroy.Add(child.gameObject);
            childrenToDestroy.ForEach(Destroy);
        }
    }

    public void ResolveEnemyTurn()
    {
        foreach (var enemy in enemiesOnFloor.ToList())
        {
            if (enemy != null && currentPlayer != null)
            {
                enemy.DecideAction(currentPlayer);
            }
        }
    }

    public void ShowAllEnemyIntent(Player player)
    {
        if (player == null) return;

        TileHighlighter highlighter = player.GetComponent<TileHighlighter>();
        if (highlighter == null) return;

        foreach (var enemy in enemiesOnFloor.ToList())
        {
            if (enemy is Knight knight)
            {
                knight.ShowIntent(player, highlighter);
            }
            if (enemy is Archer archer)
            {
                archer.ShowIntent(player, highlighter);
            }
            if (enemy is Bishop bishop)
            {
                bishop.ShowIntent(player, highlighter);
            }
            if (enemy is Boss boss)
            {
                boss.ShowIntent(player, highlighter);
            }
        }
    }

    public void RemoveCharacter(Character character)
    {
        if (character is Enemy enemy)
        {
            if (enemiesOnFloor.Contains(enemy))
            {
                enemiesOnFloor.Remove(enemy);
                Debug.Log($"Enemy {enemy.characterName} removed from tracking.");
            }
        }
        else if (character is Player player)
        {
            Debug.Log("Game Over: Player has died.");
        }

    }

    public IInteractable GetInteractableAtPosition(Vector2Int pos)
    {
        if (interactables.ContainsKey(pos))
        {
            return interactables[pos];
        }
        return null;
    }

    public void AddInteractable(InteractableObject obj, Vector2Int pos)
    {
        if (!interactables.ContainsKey(pos))
        {
            interactables.Add(pos, obj);
            obj.position = pos;
        }
    }

    public void RemoveInteractable(InteractableObject obj)
    {
        if (interactables.ContainsKey(obj.position))
        {
            interactables.Remove(obj.position);
        }
    }

    public InteractableObject SpawnInteractable(InteractableObject prefab, Vector2Int pos)
    {
        Tile targetTile = GetTile(pos.x, pos.y);
        if (targetTile == null)
        {
            Debug.LogWarning($"Spawn failed: Target tile at {pos} is outside map bounds.");
            return null;
        }

        if (GetInteractableAtPosition(pos) != null)
        {
            Debug.LogWarning($"Spawn failed: Interactable already exists at {pos}.");
            return null;
        }

        if (GetCharacterAtPosition(pos) != null)
        {
            Debug.LogWarning($"Spawn failed: Character already exists at {pos}.");
            return null;
        }

        InteractableObject newObject = Instantiate(prefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);

        newObject.Initialize(this);

        AddInteractable(newObject, pos);

        return newObject;
    }

    public void CheckLevelCompletion()
    {
        if (enemiesOnFloor.Count == 0)
        {
            Debug.Log("All enemies defeated! Spawning exit/reward chest.");
            int soulRewardAmount = currentFloor * 2;

            Player player = Player.Instance;

            if (player != null && player.currencies != null)
            {
                player.currencies.Add(EnumData.CurrencyType.SoulPoint, soulRewardAmount);

                Debug.Log($"Floor Clear! Player rewarded with {soulRewardAmount} Soul Points.");
            }

            if (SkillUpgrade.Instance != null)
            {
                StartCoroutine(WaitForUpgradeManagerAndShowSelection());
            }
            else
            {
                Debug.LogError("SkillUpgrade Manager Instance is NULL. Did you forget to place it in the scene?");
            }
            SpawnChest();
            SpawnExit();
        }
    }
    public void SpawnChest()
    {
        if (chestPrefab == null)
        {
            Debug.LogError("Chest Prefab is not assigned in DungeonManager!");
            return;
        }

        if (interactables.Values.Any(obj => obj.GetType() == chestPrefab.GetType()))
        {
            return;
        }

        Vector2Int spawnPos = GetRandomEmptyWalkablePosition();

        if (spawnPos != Vector2Int.zero)
        {
            SpawnInteractable(chestPrefab, spawnPos);
            Debug.Log($"Chest spawned at {spawnPos}");
        }
        else
        {
            Debug.LogWarning("Could not find an empty tile to spawn the chest.");
        }
    }

    private Vector2Int GetRandomEmptyWalkablePosition()
    {
        List<Vector2Int> validSpots = new List<Vector2Int>();

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (GetTile(x, y).IsWalkable() && GetCharacterAtPosition(pos) == null && GetInteractableAtPosition(pos) == null)
                {
                    validSpots.Add(pos);
                }
            }
        }

        if (validSpots.Count > 0)
        {
            return validSpots[Random.Range(0, validSpots.Count)];
        }

        return Vector2Int.zero;
    }

    private int GetManhattanDistance(Vector2Int p1, Vector2Int p2)
    {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

    private Vector2Int GetValidEnemySpawnPosition(int minDistanceToPlayer)
    {
        List<Vector2Int> potentialSpots = new List<Vector2Int>();
        Vector2Int playerPos = currentPlayer != null ? currentPlayer.position : Vector2Int.zero;

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Tile tile = GetTile(x, y);

                if (tile.IsWalkable() &&
                    GetCharacterAtPosition(pos) == null &&
                    GetInteractableAtPosition(pos) == null)
                {
                    if (currentPlayer == null || GetManhattanDistance(pos, playerPos) >= minDistanceToPlayer)
                    {
                        potentialSpots.Add(pos);
                    }
                }
            }
        }

        if (potentialSpots.Count > 0)
        {
            return potentialSpots[Random.Range(0, potentialSpots.Count)];
        }

        return Vector2Int.zero;
    }

    public void SpawnExit()
    {
        if (exitPrefab == null)
        {
            Debug.LogError("Exit Prefab is not assigned in DungeonManager!");
            return;
        }

        if (interactables.Values.Any(obj => obj.GetType() == exitPrefab.GetType()))
        {
            return;
        }

        Vector2Int exitSpawnPos = GetRandomEmptyWalkablePosition();

        if (exitSpawnPos != Vector2Int.zero)
        {
            SpawnInteractable(exitPrefab, exitSpawnPos);
            Debug.Log($"Exit spawned at {exitSpawnPos}");
        }
        else
        {
            Debug.LogWarning("Could not find an empty tile to spawn the chest.");
        }
    }

    private IEnumerator WaitForUpgradeManagerAndShowSelection()
    {
        yield return null;


        if (SkillUpgrade.Instance != null)
        {
            Debug.Log("Level Cleared! Initiating Skill Upgrade Selection.");
            SkillUpgrade.Instance.ShowUpgradeSelection();
        }
        else
        {
            Debug.LogError("FATAL ERROR: SkillUpgrade Manager Instance is NULL after waiting.");
            // จัดการ Flow เกมต่อไป (ถ้าจำเป็น)
        }
    }

    private void InvokeSetupCurrencyUI()
    {
        Player player = GetPlayer(); // หรือ Player.Instance;

        if (player != null)
        {
            // ใช้วิธีหาแบบเร็วที่สุดเพื่อสั่งให้ CurrencyUI ทำงาน
            CurrencyUI[] allCurrencyUIs = FindObjectsOfType<CurrencyUI>();

            foreach (var ui in allCurrencyUIs)
            {
                if (ui != null)
                {
                    // เรียกเมธอดที่เราสร้างขึ้น
                    ui.SetupSelf();
                }
            }
        }
    }
}