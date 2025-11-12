using System.Collections.Generic;
using UnityEngine;
using static EnumData;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    // 
    private Tile[,] map;
    private List<Vector2Int> spawnPoints;
    private List<Enemy> enemiesOnFloor;
    private Player currentPlayer;
    public int currentFloor = 1;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;

    private const int MapWidth = 10;
    private const int MapHeight = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        enemiesOnFloor = new List<Enemy>(); // Initialize list
    }

    public void GenerateFloor(int level)
    {
        // ... (Mock Map Generation) ...
        map = new Tile[MapWidth, MapHeight];
        spawnPoints = new List<Vector2Int>();
        ClearFloor(); // Clear existing objects

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                TileType type = (x == 0 || x == MapWidth - 1 || y == 0 || y == MapHeight - 1) ? TileType.Wall : TileType.Floor;
                map[x, y] = new Tile(new Vector2Int(x, y), type);
                if (map[x, y].IsWalkable()) spawnPoints.Add(new Vector2Int(x, y));
            }
        }

        SpawnPlayer();
        SpawnEnemies(level * 2);
    }

    private void SpawnPlayer()
    {
        // ป้องกันการ spawn ซ้ำ
        if (currentPlayer != null) return;

        Vector2Int spawnPos = new Vector2Int(1, 1);

        GameObject playerObj = Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, -0.1f), Quaternion.identity);
        currentPlayer = playerObj.GetComponent<Player>();
        currentPlayer.position = spawnPos;
        GetTile(spawnPos.x, spawnPos.y)?.SetOccupied(true);
    }

    public void SpawnEnemies(int count)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        Vector2Int spawnPos = new Vector2Int(MapWidth - 2, MapHeight - 2);

        GameObject enemyObj = Instantiate(enemyPrefabs[0], new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        Enemy newEnemy = enemyObj.GetComponent<Enemy>();
        newEnemy.position = spawnPos;
        GetTile(spawnPos.x, spawnPos.y)?.SetOccupied(true);
        enemiesOnFloor.Add(newEnemy);
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
        enemiesOnFloor.RemoveAll(e => e == null || e.gameObject == null);
        // Logic for destroying all objects on map...
    }
}
