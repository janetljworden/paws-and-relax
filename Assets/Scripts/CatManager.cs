using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class CatManager : MonoBehaviour
{
    public GameObject[] catPrefabs;
    private List<GameObject> availableCats;
    public Camera mainCamera;
    public TextMeshProUGUI catFactText;
    public TextMeshProUGUI nameText;
    public GameObject catViewDisplay;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        availableCats = new List<GameObject>(catPrefabs);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GetRandomCat()
    {
        if (availableCats.Count == 0) return null;

        int index = Random.Range(0, availableCats.Count);
        GameObject selectedCat = availableCats[index];

        // Remove the selected cat from the list
        availableCats.RemoveAt(index);

        return selectedCat;
    }

    public void ReturnCatToPool(GameObject cat)
    {
        if (cat != null && !availableCats.Contains(cat))
        {
            cat.SetActive(false); // Deactivate the cat to hide it
            availableCats.Add(cat);
        }
    }
}
