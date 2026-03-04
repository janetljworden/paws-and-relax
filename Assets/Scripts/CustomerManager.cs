using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public GameObject orderPrefab;
    public List<Sprite> foodSprites;
    private GameObject speechBubbleInstance;
    private Sprite currentFoodSprite;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowSpeechBubble());
    }

    private IEnumerator ShowSpeechBubble()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));

            if (speechBubbleInstance == null)
            {
                speechBubbleInstance = Instantiate(orderPrefab, transform);
                speechBubbleInstance.transform.localPosition = new Vector3(-1f, 2f, 0);
                speechBubbleInstance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }

            Transform foodItemTransform = speechBubbleInstance.transform.Find("FoodItem");
            if (foodItemTransform != null)
            {
                SpriteRenderer foodItemRenderer = foodItemTransform.GetComponent<SpriteRenderer>();
                if (foodItemRenderer != null && foodSprites.Count > 0)
                {
                    currentFoodSprite = foodSprites[Random.Range(0, foodSprites.Count)];
                    foodItemRenderer.sprite = currentFoodSprite;
                }
            }
            else
            {
                Debug.LogWarning("FoodItem GameObject not found in the speech bubble prefab.");
            }

            yield return new WaitForSeconds(5f);

            if (speechBubbleInstance != null)
            {
                Destroy(speechBubbleInstance);
            }
        }
    }

    public bool CheckFood(GameObject food)
    {
        SpriteRenderer foodRenderer = food.GetComponent<SpriteRenderer>();
        if (foodRenderer != null)
        {
            return foodRenderer.sprite == currentFoodSprite;
        }
        return false;
    }
}