using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{   
    public GameObject sceneManager;
    public GameObject cafeUI;
    public GameObject catRoomUI;
    public InventoryManager inventoryManager;

    void Start()
    {
        SetUpScene();
    }

    public void GoToCafe()
    {
        GameManager.Instance.SaveGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Cafe");
        StartCoroutine(WaitForSceneLoad("Cafe"));
        
    }

    public void GoToCatRoom()
    {
        GameManager.Instance.SaveCoins();
        inventoryManager.ResetInventory();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Cat Room");
        StartCoroutine(WaitForSceneLoad("Cat Room"));
        SetUpScene();
        GameManager.Instance.LoadGame();
    }

    private IEnumerator WaitForSceneLoad(string sceneName)
    {
        yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName);
        SetUpScene();
    }

    public void SetUpScene()
    {
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        if (currentScene.name == "Cafe")
        {
            cafeUI.SetActive(true);
            catRoomUI.SetActive(false);
        }
        else if (currentScene.name == "Cat Room")
        {
            catRoomUI.SetActive(true);
            cafeUI.SetActive(false);
        }
    }
}