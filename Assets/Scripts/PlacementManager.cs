using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public GameObject furniturePrefab;
    public float cellSize = 1f;
    public CatManager catManager;
    public GameObject furnitureOptionsPanel;
    private GameObject clickedFurniture;
    private GameObject currentFurniture;
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    private List<GameObject> placedFurniture = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Move the furniture with the mouse position (or touch input)
        if (currentFurniture != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;  // Ensure the Z position stays at 0 (2D)

            // Snap the furniture to the grid
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(mouseWorldPosition.x / cellSize) * cellSize,
                Mathf.Round(mouseWorldPosition.y / cellSize) * cellSize,
                0
            );

            // Prevent the furniture from being dragged off-screen
            snappedPosition = RestrictPositionToScreen(snappedPosition);

            // Check if the furniture can be placed
            Furniture furnitureScript = currentFurniture.GetComponent<Furniture>();
            if (furnitureScript != null && CanPlaceFurniture(snappedPosition, furnitureScript.gridWidth, furnitureScript.gridHeight))
            {
                // Update the furniture position
                currentFurniture.transform.position = snappedPosition;

                // Place the furniture
                if (Input.GetMouseButtonDown(0)) // Left-click to place
                {
                    PlaceFurniture(snappedPosition, furnitureScript.gridWidth, furnitureScript.gridHeight);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DetectFurnitureClick();
        }

    }

    private void DetectFurnitureClick()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

        if (hit.collider != null)
        {
            clickedFurniture = hit.collider.gameObject;

            // Check if the clicked object is tagged as Furniture
            if (clickedFurniture.CompareTag("furniture"))
            {
                Debug.Log("Clicked on furniture: " + clickedFurniture.name);

                furnitureOptionsPanel.SetActive(true);
            }
        }
    }

    public void PutAwayFurniture()
    {
        if (clickedFurniture != null)
        {
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                Furniture furnitureScript = clickedFurniture.GetComponent<Furniture>();
                if (furnitureScript != null)
                {
                    string furnitureName = furnitureScript.furnitureName;

                    // Detach the cat from the furniture
                    GameObject detachedCat = furnitureScript.DetachCat();
                    if (detachedCat != null)
                    {
                        catManager.ReturnCatToPool(detachedCat); // Return the cat to the pool
                    }

                    inventoryManager.AddItemToInventory(furnitureName, furnitureScript.itemID, 0, null, clickedFurniture);

                    // Free up occupied space
                    SetUnoccupied(clickedFurniture.transform.position, furnitureScript.gridWidth, furnitureScript.gridHeight);

                    placedFurniture.Remove(clickedFurniture);
                }
                else
                {
                    Debug.LogWarning("Furniture script missing on the clicked object.");
                }
            }
            Destroy(clickedFurniture);

            ExitFurnitureOptions();
        }
    }

    public void RelocateFurniture()
    {
        if (clickedFurniture != null)
        {
            Furniture furnitureScript = clickedFurniture.GetComponent<Furniture>();
            if (furnitureScript != null)
            {
                SetUnoccupied(clickedFurniture.transform.position, furnitureScript.gridWidth, furnitureScript.gridHeight);

                currentFurniture = clickedFurniture;

                Transform[] childTransforms = clickedFurniture.GetComponentsInChildren<Transform>();

                foreach (Transform child in childTransforms)
                {
                    if (child != clickedFurniture.transform)
                    {
                        child.SetParent(currentFurniture.transform);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Furniture script missing on the clicked object.");
            }
            
            clickedFurniture = null;

            ExitFurnitureOptions();
        }
    }

    public void ExitFurnitureOptions()
    {
        furnitureOptionsPanel.SetActive(false);
    }

    public void StartPlacingFurniture(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            furniturePrefab = itemPrefab;
            currentFurniture = Instantiate(furniturePrefab);
            SpriteRenderer spriteRenderer = currentFurniture.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = -1;
            }
        }
    }

    private void PlaceFurniture(Vector3 position, int width, int height)
    {
        if (currentFurniture != null)
        {
            SetOccupied(position, width, height);
            currentFurniture.transform.position = position;

            placedFurniture.Add(currentFurniture);

            Furniture furnitureScript = currentFurniture.GetComponent<Furniture>();
            SpriteRenderer renderer = currentFurniture.GetComponent<SpriteRenderer>();
            if (furnitureScript != null && renderer != null && catManager != null)
            {
                renderer.sortingOrder = -2;
                GameObject randomCat = catManager.GetRandomCat();
                if (randomCat != null)
                {
                    furnitureScript.PlaceCat(randomCat);
                }
            }
            currentFurniture = null;
        }
    }

    private Vector3 RestrictPositionToScreen(Vector3 position)
    {
        Vector3 screenMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 0.66f, 0));

        float furnitureWidth = currentFurniture.GetComponent<SpriteRenderer>().bounds.size.x;
        float furnitureHeight = currentFurniture.GetComponent<SpriteRenderer>().bounds.size.y;

        float halfWidth = furnitureWidth / 2;
        float halfHeight = furnitureHeight / 2;

        position.x = Mathf.Clamp(position.x, screenMin.x + halfWidth, screenMax.x - halfWidth);
        position.y = Mathf.Clamp(position.y, screenMin.y + halfHeight, screenMax.y - halfHeight);

        return position;
    }

    public bool CanPlaceFurniture(Vector3 position, int width, int height)
    {
        Vector2Int startCell = WorldToGrid(position);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int cell = startCell + new Vector2Int(x, y);
                if (occupiedCells.Contains(cell))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SetOccupied(Vector3 position, int width, int height)
    {
        Vector2Int startCell = WorldToGrid(position);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                occupiedCells.Add(startCell + new Vector2Int(x, y));
            }
        }
    }

    public void SetUnoccupied(Vector3 position, int width, int height)
    {
        Vector2Int startCell = WorldToGrid(position);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int cell = startCell + new Vector2Int(x, y);
                if (occupiedCells.Contains(cell))
                {
                    occupiedCells.Remove(cell);
                }
            }
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);
        return new Vector2Int(x, y);
    }

    public List<FurnitureData> GetPlacedFurnitureData()
    {
        List<FurnitureData> furnitureDataList = new List<FurnitureData>();

        placedFurniture.RemoveAll(furniture => furniture == null);

        foreach (GameObject furniture in placedFurniture)
        {
            Furniture furnitureScript = furniture.GetComponent<Furniture>();
            if (furnitureScript != null)
            {
                FurnitureData data = new FurnitureData
                {
                    furnitureName = furnitureScript.furnitureName,
                    position = furniture.transform.position,
                    gridWidth = furnitureScript.gridWidth,
                    gridHeight = furnitureScript.gridHeight
                };

                if (furnitureScript.attachedCat != null)
                {
                    Animator catAnimator = furnitureScript.attachedCat.GetComponent<Animator>();
                    data.attachedCatName = furnitureScript.attachedCat.name.Replace("(Clone)", "").Trim();
                    data.attachedCatAnimationIndex = catAnimator != null ? catAnimator.GetInteger("AnimationIndex") : 0;
                }

                furnitureDataList.Add(data);
            }
        }
        return furnitureDataList;
    }

    public void LoadPlacedFurniture(List<FurnitureData> furnitureDataList)
    {
        foreach (FurnitureData data in furnitureDataList)
        {
            GameObject prefab = Resources.Load<GameObject>($"Furniture/{data.furnitureName}");
            if (prefab != null)
            {
                GameObject furniture = Instantiate(prefab, data.position, Quaternion.identity);
                Furniture furnitureScript = furniture.GetComponent<Furniture>();
                if (furnitureScript != null)
                {
                    furnitureScript.gridWidth = data.gridWidth;
                    furnitureScript.gridHeight = data.gridHeight;

                    if (!string.IsNullOrEmpty(data.attachedCatName))
                    {
                        GameObject catPrefab = Resources.Load<GameObject>($"Cats/{data.attachedCatName}");
                        if (catPrefab != null)
                        {
                            furnitureScript.PlaceCat(catPrefab);

                            Animator catAnimator = furnitureScript.attachedCat.GetComponent<Animator>();
                            if (catAnimator != null)
                            {
                                catAnimator.SetInteger("AnimationIndex", data.attachedCatAnimationIndex);
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Cat prefab not found: {data.attachedCatName}");
                        }
                    }
                }
                placedFurniture.Add(furniture);
                SetOccupied(data.position, data.gridWidth, data.gridHeight);
            }
            else
            {
                Debug.LogWarning($"Furniture prefab not found: {data.furnitureName}");
            }
        }
    }

    public void ClearFurniture()
    {
        foreach (GameObject furniture in placedFurniture)
        {
            Destroy(furniture);
        }
        placedFurniture.Clear();
        occupiedCells.Clear();
    }
}
