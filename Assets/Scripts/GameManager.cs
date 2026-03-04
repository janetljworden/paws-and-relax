using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private SceneManager sceneManager;
    public int coins = 0;
    public Text coinsTxt;
    private GameData data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        data = new GameData();
        sceneManager = FindObjectOfType<SceneManager>();
        LoadGame();
    }

    public void SaveGame()
    {
        data.coins = coins;

        PlacementManager placementManager = FindObjectOfType<PlacementManager>();
        if (placementManager != null)
        {
            data.placedFurniture = placementManager.GetPlacedFurnitureData();
        }

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            data.inventoryItems = inventoryManager.GetInventoryData();
        }

        data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        SaveManager.Instance.SaveGame(data);
    }

    public void SaveCoins()
    {
        data.coins = coins;

        data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        SaveManager.Instance.SaveGame(data);
    }

    public void LoadGame()
    {
        GameData data = SaveManager.Instance.LoadGame();
        if (data != null)
        {
            coins = data.coins;
            UpdateCoinsUI();

            PlacementManager placementManager = FindObjectOfType<PlacementManager>();
            if (placementManager != null)
            {
                placementManager.LoadPlacedFurniture(data.placedFurniture);
            }

            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.LoadInventoryData(data.inventoryItems);
            }

            if (!string.IsNullOrEmpty(data.currentScene) && data.currentScene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(data.currentScene);
            }

            StartCoroutine(WaitForSceneSetup());
        }
    }

    private IEnumerator WaitForSceneSetup()
    {
        yield return new WaitUntil(() => UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded);
        PlacementManager placementManager = FindObjectOfType<PlacementManager>();
        if (placementManager != null)
        {
            GameData data = SaveManager.Instance.LoadGame();
            if (data != null)
            {
                placementManager.LoadPlacedFurniture(data.placedFurniture);
            }
        }
        sceneManager?.SetUpScene();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinsTxt.text = coins.ToString();
    }

    public void UpdateCoinsUI()
    {
        if (coinsTxt != null)
        {
            coinsTxt.text = coins.ToString();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
