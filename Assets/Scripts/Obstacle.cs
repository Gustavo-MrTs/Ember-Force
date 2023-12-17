using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float speedV = -5;
    private bool playerCollided = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCollided)
        {
            // Change the movement direction to Vector3.back when player collides
            transform.Translate(Vector3.zero * speedV * Time.deltaTime);
        }
        else
        {
            // Continue moving down if no collision
            transform.Translate(Vector3.down * speedV * Time.deltaTime);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Set the playerCollided flag to true when collision with the player occurs
            playerCollided = true;

        }
    }
}
