using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public int gridWidth = 1;
    public int gridHeight = 1;
    public string furnitureName;
    public Transform[] catSpots;
    private bool isCatPlaced = false;
    public GameObject attachedCat;
    public int itemID;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            gridWidth = Mathf.CeilToInt(renderer.bounds.size.x); // Width in grid cells
            gridHeight = Mathf.CeilToInt(renderer.bounds.size.y); // Height in grid cells
            renderer.sortingOrder = -2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceCat(GameObject catPrefab)
    {
        if (attachedCat != null || isCatPlaced || catSpots.Length == 0) return;

        // Choose a random cat spot
        int randomIndex = Random.Range(0, catSpots.Length);
        Transform catSpot = catSpots[randomIndex];

        // Instantiate the cat at the chosen spot
        attachedCat = Instantiate(catPrefab, catSpot.position, Quaternion.identity);

        attachedCat.transform.SetParent(this.transform);

        SpriteRenderer spriteRenderer = attachedCat.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -1; // Set the order layer to -1
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on the cat prefab.");
        }

        // Randomize the cat animation
        Animator animator = attachedCat.GetComponent<Animator>();
        if (animator != null)
        {
            int randomAnimation = Random.Range(0, 4); // 0 to 3 for 4 animations
            animator.SetInteger("AnimationIndex", randomAnimation);
        }
        else
        {
            Debug.LogWarning("No Animator found on the cat prefab.");
        }

        isCatPlaced = true; // Mark this furniture as occupied by a cat
    }

    public GameObject DetachCat()
    {
        if (attachedCat != null)
        {
            GameObject tempCat = attachedCat;
            attachedCat.transform.SetParent(null); // Detach from furniture
            tempCat.SetActive(true);               // Ensure the cat is active
            attachedCat = null;                    // Clear reference
            return tempCat;
        }
        return null;
    }
}
