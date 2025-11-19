using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public DungeonManager dungeonManager;

    public int CurrentFloor { get; private set; } = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);

        dungeonManager = FindFirstObjectByType<DungeonManager>();
    }

    private void Start()
    {
        StartCoroutine(LoadFloorSceneAsync());
    }

    public void GameOver() { Debug.Log("--- GAME OVER ---"); }

    public void NextFloor()
    {
        CurrentFloor++;
        StartCoroutine(LoadFloorSceneAsync());
    }


    public IEnumerator LoadFloorSceneAsync()
    {
        string sceneName;

        // Logic การเลือก Scene
        if (CurrentFloor == 1) sceneName = "Main";
        else if (CurrentFloor >= 2 && CurrentFloor <= 5) sceneName = $"Floor{CurrentFloor}";
        else
        {
            sceneName = "Mainmenu";
            CurrentFloor = 1;
        }

        // **FIX 3: ใช้ SceneManager.LoadSceneAsync**
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // ... (yield return null) ...
            yield return null;
        }

        // เมื่อ Scene โหลดเสร็จ DungeonManager ตัวใหม่จะถูกสร้างและรัน Start()
    }
}
