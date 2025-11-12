using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public DungeonManager dungeonManager;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        dungeonManager = FindObjectOfType<DungeonManager>();
    }

    private void Start()
    {
        if (dungeonManager != null)
        {
            StartGame();
        }
        else
        {
            Debug.LogError("Setup Error: DungeonManager not found in scene.");
        }
    }

    public void StartGame()
    {
        Debug.Log("--- GAME START (Free Move Mode) ---");
        dungeonManager.GenerateFloor(1);
    }

    public void GameOver() { Debug.Log("--- GAME OVER ---"); }
    public void NextFloor()
    {
        dungeonManager.ClearFloor();
        dungeonManager.GenerateFloor(dungeonManager.currentFloor + 1);
    }
}
