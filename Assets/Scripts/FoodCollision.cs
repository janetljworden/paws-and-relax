using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpeechBubble"))
        {
            Debug.Log("Collided with speech bubble!");

            CustomerManager customer = collision.GetComponentInParent<CustomerManager>();
            if (customer != null && customer.CheckFood(gameObject))
            {
                Debug.Log("Correct item delivered!");

                GameManager.Instance.AddCoins(5);

                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Incorrect item delivered!");
            }
        }
    }
}
