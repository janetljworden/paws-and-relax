using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventory;
    public ShopManager shopManager;
    public GameObject inventoryContent;
    public GameObject inventoryItemPrefab;
    private Dictionary<int, int> inventoryItems = new Dictionary<int, int>(); // Store itemID and quantity

    public Dictionary<int, string> itemNames = new Dictionary<int, string>();
    public Dictionary<int, Sprite> itemIcons = new Dictionary<int, Sprite>();

    // Adds an item to the inventory or updates the quantity if it already exists
    public void AddItemToInventory(string itemName, int itemID, int quantity, Sprite itemIconSprite, GameObject itemPrefab)
    {
        if (inventoryItems.ContainsKey(itemID))
        {
            inventoryItems[itemID]++; // Increment quantity
            UpdateInventoryUI(itemName, inventoryItems[itemID]);
        }
        else
        {
            inventoryItems[itemID] = 1; // Add new item
            itemNames[itemID] = itemName; // Store item name
            itemIcons[itemID] = itemIconSprite; // Store item icon

            // Debug to make sure the sprite is added to the dictionary
            Debug.Log($"Adding icon for item {itemID}: {itemIconSprite.name}");

            CreateInventoryItem(itemName, itemID, quantity, itemIconSprite, itemPrefab);
        }
    }

    private void CreateInventoryItem(string itemName, int itemID, int quantity, Sprite itemIconSprite, GameObject itemPrefab)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, inventoryContent.transform);
        newItem.SetActive(true);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();

        inventoryItem.itemNameTxt.text = itemName;
        inventoryItem.quantityTxt.text = quantity.ToString();

        if (inventoryItem.itemIcon != null && itemIconSprite != null)
        {
            inventoryItem.itemIcon.sprite = itemIconSprite;
            inventoryItem.itemIcon.preserveAspect = true;
        }

        Button itemButton = newItem.GetComponentInChildren<Button>();
        if (itemButton == null)
        {
            Debug.LogError("Button component is missing from the inventory item prefab!");
            return;
        }
        itemButton.onClick.AddListener(() => OnInventoryItemClicked(itemID, itemName, itemPrefab));
    }

    // Updates an existing inventory item's quantity
    private void UpdateInventoryUI(string itemName, int quantity)
    {
        foreach (Transform child in inventoryContent.transform)
        {
            InventoryItem inventoryItem = child.GetComponent<InventoryItem>();
            if (inventoryItem.itemNameTxt.text == itemName)
            {
                inventoryItem.quantityTxt.text = quantity.ToString();
                return;
            }
        }
    }

    private void OnInventoryItemClicked(int itemID, string itemName, GameObject clickedItemPrefab)
    {
        if (inventoryItems[itemID] > 0)
        {
            PlacementManager placementManager = FindObjectOfType<PlacementManager>();

            if (placementManager != null)
            {
                placementManager.StartPlacingFurniture(clickedItemPrefab);
            }
            else
            {
                Debug.LogError("PlacementManager not found!");
            }

            inventoryItems[itemID]--;
            UpdateInventoryUI(itemName, inventoryItems[itemID]);
            ToggleInventory();
        }
        else
        {
            Debug.Log("You have placed this item already.");
        }
    }

    public void ToggleInventory()
    {
        if (shopManager.shop.activeSelf)
        {
            shopManager.shop.SetActive(false);
        }
        inventory.SetActive(!inventory.activeSelf);
    }

    public void ResetInventory()
    {
        inventoryItems.Clear();

        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public List<InventoryItemData> GetInventoryData()
    {
        List<InventoryItemData> inventoryDataList = new List<InventoryItemData>();
        foreach (var item in inventoryItems)
        {
            inventoryDataList.Add(new InventoryItemData
            {
                itemID = item.Key,
                quantity = item.Value,
                itemName = itemNames[item.Key],
                itemIcon = itemIcons.ContainsKey(item.Key) ? itemIcons[item.Key] : null // Check if icon exists
            });
        }
        return inventoryDataList;
    }

    public void LoadInventoryData(List<InventoryItemData> inventoryData)
    {
        foreach (var itemData in inventoryData)
        {
            inventoryItems[itemData.itemID] = itemData.quantity;

            string itemName = itemData.itemName;
            Sprite itemIcon = itemData.itemIcon;
            if (itemIcon == null)
            {
                Debug.LogWarning($"Item icon not found for item ID {itemData.itemID}. Ensure the sprite is assigned.");
            }

            CreateInventoryItem(itemName, itemData.itemID, itemData.quantity, itemIcon, inventoryItemPrefab);
        }
    }
}
