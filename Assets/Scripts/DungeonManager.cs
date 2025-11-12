using static EnumData;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    private Tile[,] map;
    private List<Vector2Int> spawnPoints;
    private List<Enemy> enemiesOnFloor;
    private Player currentPlayer;
    public int currentFloor = 1;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void GenerateFloor(int level)
    {
        // ... (Map Generation Logic) ...
        // Mock Map Generation (10x10)
        int width = 10; int height = 10;
        map = new Tile[width, height];
        spawnPoints = new List<Vector2Int>();
        enemiesOnFloor = new List<Enemy>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileType type = (x == 5 && y == 5) ? TileType.Exit : TileType.Floor;
                map[x, y] = new Tile(new Vector2(x, y), type);
                if (map[x, y].IsWalkable()) spawnPoints.Add(new Vector2Int(x, y));
            }
        }

        SpawnPlayer();
        SpawnEnemies(level * 2);
    }

    private void SpawnPlayer()
    {
        if (spawnPoints.Count == 0 || playerPrefab == null) return;
        Vector2Int spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)];
        spawnPoints.Remove(spawnPos);

        GameObject playerObj = Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        currentPlayer = playerObj.GetComponent<Player>();
        currentPlayer.position = spawnPos;
    }

    public void SpawnEnemies(int count)
    {
        for (int i = 0; i < count && spawnPoints.Count > 0; i++)
        {
            Vector2Int spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)];
            spawnPoints.Remove(spawnPos);

            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemyObj = Instantiate(enemyPrefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
            Enemy newEnemy = enemyObj.GetComponent<Enemy>();

            enemiesOnFloor.Add(newEnemy);
        }
    }

    public void ClearFloor()
    {
        if (enemiesOnFloor != null)
        {
            foreach (Enemy enemy in enemiesOnFloor.Where(e => e != null)) Destroy(enemy.gameObject);
            enemiesOnFloor.Clear();
        }
        if (currentPlayer != null) Destroy(currentPlayer.gameObject);
    }

    public Tile GetTile(int x, int y)
    {
        if (map == null || x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return null;
        return map[x, y];
    }

    public Player GetPlayer() => currentPlayer;
    public List<Enemy> GetEnemies() => enemiesOnFloor.Where(e => e != null).ToList();

    // --- New Method: GetCharacterAtPosition (เพื่อใช้ใน Player.Move) ---
    public Character GetCharacterAtPosition(Vector2Int pos)
    {
        if (currentPlayer != null && currentPlayer.position == pos) return currentPlayer;
        // ค้นหาในศัตรูที่ยังมีชีวิตอยู่
        return enemiesOnFloor.Find(e => e != null && e.position == pos);
    }
}