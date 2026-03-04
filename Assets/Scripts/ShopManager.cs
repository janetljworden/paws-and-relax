using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public int[,] shopItems = new int[3,35];
    private int coins;
    public Text coinsTxt;
    public GameObject shop;
    public InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        coins = GameManager.Instance.coins;
        coinsTxt.text = coins.ToString();

        // ids and quantities
        for (int i = 1; i < shopItems.GetLength(1); i++) {
            shopItems[1, i] = i;
            shopItems[2, i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Buy()
    {
        GameObject buttonRef = EventSystem.current.currentSelectedGameObject;
        ShopInfo shopInfo = buttonRef.GetComponent<ShopInfo>();

        if (shopInfo != null)
        {
            int itemPrice = shopInfo.price; 

            if (GameManager.Instance.coins >= itemPrice)
            {
                GameManager.Instance.coins -= itemPrice;

                coinsTxt.text = GameManager.Instance.coins.ToString();

                shopInfo.quantityTxt.text = (++shopItems[2, shopInfo.itemID]).ToString();

                inventoryManager.AddItemToInventory(
                    shopInfo.nameTxt.text, 
                    shopInfo.itemID, 
                    shopItems[2, shopInfo.itemID], 
                    shopInfo.itemIcon,
                    shopInfo.itemPrefab);
            }
            else
            {
                Debug.Log("Not enough coins to buy this item.");
            }
        }
        else
        {
            Debug.LogError("ShopInfo component not found on the clicked object.");
        }
    }


    public void ToggleShop()
    {
        if (inventoryManager.inventory.activeSelf)
        {
            inventoryManager.inventory.SetActive(false);
        }
        shop.SetActive(!shop.activeSelf);
    }
}
