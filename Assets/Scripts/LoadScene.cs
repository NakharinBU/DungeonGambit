using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    GameObject shopManager;
    GameObject soulUpgrade;
    GameObject player;
    GameObject gameManager;

    private void Awake()
    {
        GameObject shopManager = GameObject.Find("ShopManager");
        GameObject soulUpgrade = GameObject.Find("SoulUpgrade");
        GameObject gameManager = GameObject.Find("GameManager");
        GameObject player = GameObject.Find("PLAYER");
        Destroy(shopManager);
        Destroy(soulUpgrade);
        Destroy(gameManager);
        Destroy(player);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void BackMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
