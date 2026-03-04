using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject OptionsPanel;

    public void SaveAndQuit()
    {
        GameManager.Instance.SaveGame();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void NewGame()
    {
        SaveManager.Instance.DeleteSave();
        GameManager.Instance.coins = 0;
        GameManager.Instance.UpdateCoinsUI();

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.ResetInventory();
        }

        PlacementManager placementManager = FindObjectOfType<PlacementManager>();
        if (placementManager != null)
        {
            placementManager.ClearFurniture();
        }
    }

    public void ToggleOptions()
    {
        OptionsPanel.SetActive(!OptionsPanel.activeSelf);
    }
}
