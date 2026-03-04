using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public GameObject food;
    private GameObject duplicate;
    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging && duplicate != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z - 1;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            duplicate.transform.position = worldPosition;

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                Destroy(duplicate);
            }
        }
    }

    void OnMouseDown()
    {
        duplicate = Instantiate(food);
        isDragging = true;

        duplicate.transform.position = transform.position + new Vector3(0.1f, 0, 0);

        BoxCollider2D collider = duplicate.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = duplicate.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;

        Rigidbody2D rb = duplicate.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        duplicate.AddComponent<FoodCollision>();
    }
}
