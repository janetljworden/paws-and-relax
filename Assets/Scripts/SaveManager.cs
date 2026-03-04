using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class InventoryItemData
{
    public int itemID;
    public int quantity;
    public string itemName;
    public Sprite itemIcon;
}

[System.Serializable]
public class GameData
{
    public int coins;
    public List<FurnitureData> placedFurniture;
    public List<InventoryItemData> inventoryItems;
    public string currentScene;
}

[System.Serializable]
public class FurnitureData
{
    public string furnitureName;
    public Vector3 position;
    public int gridWidth;
    public int gridHeight;
    public string attachedCatName;
    public int attachedCatAnimationIndex;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            Debug.Log($"Save file path: {saveFilePath}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game saved.");
    }

    public GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        Debug.LogWarning("No save file found.");
        return null;
    }

    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }
}