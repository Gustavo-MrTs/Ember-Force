using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerControler : MonoBehaviour

{
    private bool isImmortal = false; // Flag to track player's immortality status
    public GameManager gameManager;
    public int lives = 0;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    public GameObject scores;
    public GameObject exit;
    public GameObject manager;
    public GameObject pauseConfigCanvas;
    private bool canJump;
    private bool canSlide;
    private float swipeThreshold = 100f; // Adjust this threshold as needed
    private Rigidbody rb;
    public float moveSpeed = 5f; // Adjust the speed of left/right movement
    public float jumpForce = 10f; // Adjust the force for jumping
    public bool morta = false;
    public int scoreP;

    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            anim.SetBool("Jump", false);
            canJump = true;
            canSlide = true;
            Debug.Log("canJump tá true");
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" )
        {
            anim.SetBool("Jump", true);
            canJump = false;
            canSlide = false;
            Debug.Log("canJump tá false");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            StartCoroutine(RotateObjectSmoothly(other.gameObject, new Vector3(90f, 0f, 0f), 1.0f)); // Rotate over 1 second
            if (!isImmortal) // Check for immortality status

            {
                lives--;
                if (lives < 0)
                {
                    lives = -1;
                    anim.SetBool("Death",true);
                    StartCoroutine(LoadFirstSceneAfterDeathAnimation());
                    morta = true;

                }
                gameManager.UpdateHelmetCount();
            }
        }
        else if (other.gameObject.CompareTag("Helmet"))
        {
            if (lives < 10)
            {
                lives++;
                gameManager.UpdateHelmetCount();
            }
            else if (lives == 10)
            {
                lives = 10;
            }
            else
            {
                lives = 10;
            }
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            // When the player collides with a coin, increase the score by 10
            gameManager.IncreaseScore(10);

            // Optionally, you can destroy the coin if you don't want it to persist in the scene
            Destroy(other.gameObject);
        }
    }
    

    private IEnumerator RotateObjectSmoothly(GameObject obj, Vector3 targetRotation, float duration)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = obj.transform.rotation;
        Quaternion targetRotationQuaternion = Quaternion.Euler(targetRotation);

        while (elapsedTime < duration)
        {
            obj.transform.rotation = Quaternion.Slerp(startRotation, targetRotationQuaternion, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends exactly at the target rotation
        obj.transform.rotation = targetRotationQuaternion;

        // Optionally, you can delay destroying the object to allow the player to see the rotated object for a moment.
        // You can adjust the delay time as needed.
        float delayBeforeDestroy = 1.0f;
        yield return new WaitForSeconds(delayBeforeDestroy);

        // Destroy the object after the delay
        Destroy(obj);
    }


    void ToggleImmortality()
    {
        isImmortal = !isImmortal;
        Debug.Log("Immortality toggled: " + isImmortal);
    }

    // Update is called once per frame
    IEnumerator LoadFirstSceneAfterDeathAnimation()
    {
        // Wait for the duration of the death animation
        yield return new WaitForSeconds(7f);

        // Load the first scene
        Time.timeScale = 0; // Pause the game
        pauseConfigCanvas.SetActive(true); // Show the pauseConfigCanvas
        scores.SetActive(false);
        exit.SetActive(false);
    }
    void Update()
    {
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began)
        {
            ToggleImmortality();
        }
        if (!morta) { 
        if (Input.touchCount > 0 )
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                float swipeDistanceX = touchEndPos.x - touchStartPos.x;
                float swipeDistanceY = touchEndPos.y - touchStartPos.y;

                // Calculate the absolute values of swipe distances
                float absSwipeDistanceX = Mathf.Abs(swipeDistanceX);
                float absSwipeDistanceY = Mathf.Abs(swipeDistanceY);

                // Check if the horizontal swipe is greater and not diagonal
                if (absSwipeDistanceX >= absSwipeDistanceY && absSwipeDistanceX >= swipeThreshold)
                {
                    if (swipeDistanceX > 0 && canSlide && transform.position.x < 0.9f)
                    {
                        // Swipe right
                        rb.AddForce(Vector3.right * moveSpeed, ForceMode.Impulse);
                        Debug.Log("right");
                    }
                    else if (swipeDistanceX < 0 && canSlide && transform.position.x > -0.9f)
                    {
                        // Swipe left
                        rb.AddForce(Vector3.left * moveSpeed, ForceMode.Impulse);
                        Debug.Log("left");
                    }
                }
                // Check if the vertical swipe is greater and not diagonal
                else if (absSwipeDistanceY >= absSwipeDistanceX && absSwipeDistanceY >= swipeThreshold)
                {
                    if (swipeDistanceY > 0)
                    {
                        // Swipe up (jump)
                        if (canJump)
                        {
                            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                            Debug.Log("Up");
                        }
                    }
                    else
                    {
                        rb.AddForce(Vector3.up / jumpForce, ForceMode.Impulse);
                        Debug.Log("Down");
                    }
                }
            }
        }
    }
  }
}