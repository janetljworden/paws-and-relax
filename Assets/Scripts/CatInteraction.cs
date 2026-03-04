using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class CatInteraction : MonoBehaviour
{
    public CatManager catManager;
    public InventoryManager inventoryManager;
    public ShopManager shopManager;
    public PlacementManager placementManager;
    public float zoomSize = 2.5f;
    public float zoomSpeed = 2f;
    public float zoomDuration = 3f;
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private bool isZoomedIn = false;
    public string catName;

    private void Start()
    {
        catManager = FindObjectOfType<CatManager>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        shopManager = FindObjectOfType<ShopManager>();
        placementManager = FindObjectOfType<PlacementManager>();

        if (catManager == null)
        {
            Debug.LogError("CatManager not assigned in the Inspector!");
            return;
        }

        catManager.exitButton.onClick.AddListener(OnExitButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!isZoomedIn)
        {
            StartCoroutine(ZoomIn());
            StartCoroutine(FetchCatFact());
        }
    }

    private IEnumerator ZoomIn()
    {
        if (!inventoryManager.inventory.activeSelf && !shopManager.shop.activeSelf && !placementManager.furnitureOptionsPanel.activeSelf)
        {
            isZoomedIn = true;

            // Save original camera state
            originalCameraPosition = catManager.mainCamera.transform.position;
            originalCameraSize = catManager.mainCamera.orthographicSize;

            // Target camera state
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 0.15f, catManager.mainCamera.transform.position.z);
            float targetSize = zoomSize;

            // Smoothly move the camera and zoom
            float elapsed = 0f;
            while (elapsed < zoomSpeed)
            {
                elapsed += Time.deltaTime;
                catManager.mainCamera.transform.position = Vector3.Lerp(originalCameraPosition, targetPosition, elapsed / zoomSpeed);
                catManager.mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, targetSize, elapsed / zoomSpeed);
                yield return null;
            }

            catManager.catViewDisplay.SetActive(true);

            // Hold zoom
            yield return new WaitForSeconds(zoomDuration);
        }
    }

    private void OnExitButtonClick()
    {
        StartCoroutine(ZoomOut());
        catManager.catViewDisplay.SetActive(false); // Hide the cat view display
    }

    private IEnumerator ZoomOut()
    {
        // Smoothly return the camera to its original state
        float elapsed = 0f;
        while (elapsed < zoomSpeed)
        {
            elapsed += Time.deltaTime;
            catManager.mainCamera.transform.position = Vector3.Lerp(catManager.mainCamera.transform.position, originalCameraPosition, elapsed / zoomSpeed);
            catManager.mainCamera.orthographicSize = Mathf.Lerp(catManager.mainCamera.orthographicSize, originalCameraSize, elapsed / zoomSpeed);
            yield return null;
        }

        isZoomedIn = false;
    }

    private IEnumerator FetchCatFact()
    {
        string url = "https://catfact.ninja/fact";
        int maxLength = 120;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching cat fact: " + request.error);
                catManager.catFactText.text = "Could not load cat fact. Try again later!";
            }
            else
            {
                // Parse the JSON response
                string jsonResponse = request.downloadHandler.text;
                CatFactResponse catFactResponse = JsonUtility.FromJson<CatFactResponse>(jsonResponse);

                string fact = catFactResponse.fact;
                if (fact.Length > maxLength)
                {
                    fact = fact.Substring(0, maxLength) + "..."; // Trim and add ellipsis if it exceeds the max length
                }

                // Display the cat fact
                catManager.catFactText.text = fact;
                catManager.nameText.text = catName;
            }
        }
    }

    [System.Serializable]
    private class CatFactResponse
    {
        public string fact;
    }

}
