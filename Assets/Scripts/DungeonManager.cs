using static EnumData;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance { get; private set; }

    private Tile[,] map;
    private List<Vector2Int> spawnPoints;
    private List<Enemy> enemiesOnFloor = new List<Enemy>(); // ต้อง Initialize ตั้งแต่แรก
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

    public int MapWidth = 10;
    public int MapHeight = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void GenerateFloor(int level)
    {
        // 1. เคลียร์ฉากเก่า
        ClearFloor();

        map = new Tile[MapWidth, MapHeight];
        spawnPoints = new List<Vector2Int>();
        currentFloor = level;

        if (tileParent == null)
        {
            tileParent = new GameObject("TileParent").transform;
        }

        // 2. สร้าง Data Grid (Tile Data) และ Visual Map
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                TileType type = (x == 0 || x == MapWidth - 1 || y == 0 || y == MapHeight - 1)
                                ? TileType.Wall
                                : TileType.Floor;

                map[x, y] = new Tile(new Vector2Int(x, y), type);

                // Instantiate Visual Tile
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

        // 3. Spawn Actors
        SpawnPlayer();
        SpawnEnemies(level * 2);
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null) return;

        // ตำแหน่ง 1, 1 คือตำแหน่งเริ่มต้นที่ดี
        Vector2Int spawnPos = new Vector2Int(1, 1);

        GameObject playerObj = Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, -0.1f), Quaternion.identity);
        currentPlayer = playerObj.GetComponent<Player>();
        currentPlayer.position = spawnPos;
        GetTile(spawnPos.x, spawnPos.y)?.SetOccupied(true);
        currentPlayer.name = "PLAYER";
    }

    public void SpawnEnemies(int count)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // วางศัตรูตัวแรกที่มุมตรงข้าม
        Vector2Int spawnPos = new Vector2Int(MapWidth - 2, MapHeight - 2);

        GameObject enemyObj = Instantiate(enemyPrefabs[0], new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity);
        Enemy newEnemy = enemyObj.GetComponent<Enemy>();
        newEnemy.position = spawnPos;
        GetTile(spawnPos.x, spawnPos.y)?.SetOccupied(true);
        enemiesOnFloor.Add(newEnemy);
        newEnemy.name = $"ENEMY_{spawnPos.x},{spawnPos.y}";
    }

    public Tile GetTile(int x, int y)
    {
        if (map == null || x < 0 || y < 0 || x >= MapWidth || y >= MapHeight) return null;
        return map[x, y];
    }

    public Player GetPlayer() => currentPlayer;
    public Character GetCharacterAtPosition(Vector2Int pos)
    {
        // ตรวจสอบ Player
        if (currentPlayer != null && currentPlayer.position == pos) return currentPlayer;
        // ตรวจสอบ Enemy
        return enemiesOnFloor.Find(e => e != null && e.position == pos);
    }

    public void ClearFloor()
    {
        // ทำลาย Player
        if (currentPlayer != null) Destroy(currentPlayer.gameObject);
        currentPlayer = null;

        // ทำลาย Enemy
        foreach (var enemy in enemiesOnFloor.ToList()) if (enemy != null) Destroy(enemy.gameObject);
        enemiesOnFloor.Clear();

        // ทำลาย Visual Tiles
        if (tileParent != null)
        {
            // ทำให้มั่นใจว่า Destroy ได้อย่างปลอดภัย
            List<GameObject> childrenToDestroy = new List<GameObject>();
            foreach (Transform child in tileParent) childrenToDestroy.Add(child.gameObject);
            childrenToDestroy.ForEach(Destroy);
        }
    }

    public void ResolveEnemyTurn()
    {
        // ทำสำเนาของ List ศัตรูเพื่อป้องกันการแก้ไขขณะวนลูป (เช่น ศัตรูตาย)
        foreach (var enemy in enemiesOnFloor.ToList())
        {
            if (enemy != null && currentPlayer != null)
            {
                // สั่งให้ศัตรูตัดสินใจและทำ Action
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
            if (enemy is Knight knight) // ตรวจสอบว่าเป็น Knight เท่านั้น
            {
                // ให้ Knight เป็นผู้สั่งให้ Highlighter แสดงผล
                knight.ShowIntent(player, highlighter);
            }
        }
    }
}