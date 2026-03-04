using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInfo : MonoBehaviour
{
    public int itemID;
    public int price;
    public Text nameTxt;
    public Text priceTxt;
    public Text quantityTxt;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public GameObject shopManager;

    // Start is called before the first frame update
    void Start()
    {
        priceTxt.text = price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
