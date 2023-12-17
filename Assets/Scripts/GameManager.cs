using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject creditsCanvas;
    public GameObject configCanvas;
    public GameObject exitCreditsCanvas;
    public GameObject exitConfigCanvas;
    public GameObject menuCanvas;
    public GameObject scores;
    public GameObject obstacleNormal, obstacleNormal2;
    public GameObject obstacleLarge, obstacleLarge2;
    public GameObject helmet;
    public GameObject coin;
    public GameObject menu;
    public Scrollbar immortalityToggle;
    public GameObject obstacleTall, obstacleTall2;
    public Transform spawnPoint;
    public int score = 0;
    public TextMeshProUGUI helmetCountText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText2;
    public GameObject playButton;
    public GameObject player;
    public GameObject pauseConfigCanvas;
    public GameObject credits;

    int progress = 0;
    int end = 0;
    public float spawnTime1 = 0.5f, spawnTime2 = 1.5f;

    public Slider slider;
    public Slider sliderVolume;
    public Text valueText;
    public void OnSliderHelmet(int value)
    {
        valueText.text = value.ToString();
    }

    public void Facil(int value)
    {
        spawnTime1 = 3f; spawnTime2 = 3.5f;
        scores.SetActive(false);

    }
    public void Medio(int value)
    {
        spawnTime1 = 1.5f; spawnTime2 = 2.5f;
        scores.SetActive(false);
    }
    public void Dificil(int value)
    {
        spawnTime1 = 0.5f; spawnTime2 = 1.5f;
        scores.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
        pauseConfigCanvas.SetActive(true); // Show the pauseConfigCanvas
        scores.SetActive(false);
    }
    public void ExitCredits()
    {
        credits.SetActive(false);
    }

    public void OpenCredits()
    {
        credits.SetActive(true);
    }

    public void ExitMenuPause()
    {
        Time.timeScale = 1;
        pauseConfigCanvas.SetActive(false);
        SceneManager.LoadScene(0);
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1; 
        pauseConfigCanvas.SetActive(false); 
        scores.SetActive(true);
    }

    // Attach this method to the button that opens the Credits canvas
    public void OpenCreditsCanvas()
    {
        creditsCanvas.SetActive(true);
    }

    // Attach this method to the button that closes the Credits canvas
    public void CloseCreditsCanvas()
    {
        creditsCanvas.SetActive(false);
    }
    public void CloseMenuCanvas()
    {
        menuCanvas.SetActive(false);
    }
    public void OpenMenu()
    {
        menu.SetActive(true);
    }

    // Attach this method to the button that opens the Config canvas
    public void OpenConfigCanvas()
    {
        configCanvas.SetActive(true);
    }

    // Attach this method to the button that closes the Config canvas
    public void CloseConfigCanvas()
    {
        configCanvas.SetActive(false);
    }

    // Attach this method to the button that exits the game
    public void StopGame()
    {
        Application.Quit();
        // Note: Application.Quit() may not work in the Unity Editor, so you should test it in a build.
    }

    // Probability of spawning an obstacle at 1.0y
    public float chanceToSpawnAt1Y = 0.2f;

    // Probability of spawning another obstacle at 0.0y when one spawns at 1.0y
    public float chanceToSpawnUnderneath = 0.5f;


    // Update is called once per frame
    void Update()
    {
        end = player.GetComponent<PlayerControler>().lives;
        Debug.Log("EndGame called. Lives: " + end);

        if (end <= -1)
        {
            Debug.Log("Morreu");
            // Stop the spawning coroutines
            StopCoroutine("SpawnHelmet");
            StopCoroutine("SpawnObstacles");
            StopCoroutine("SpawnCoins");
            

            // Destroy all existing obstacles, coins, and helmets
            DestroyAllGameObjects("Obstacle");
            DestroyAllGameObjects("Coin");
            DestroyAllGameObjects("Helmet");
        }
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            
            float waitTime = Random.Range(spawnTime1, spawnTime2);
            yield return new WaitForSeconds(waitTime);

            // Generate random x and y offsets among the specified values
            float[] possibleXOffsets = new float[] { -1.0f, -0.5f, 0.0f, 0.5f, 1.0f };
            float xOffset = possibleXOffsets[Random.Range(0, possibleXOffsets.Length)];

            // Randomly determine the y-coordinate based on the chance
            float yOffset = Random.value <= chanceToSpawnAt1Y ? 0.8f : -0.5f;

            // Create a new position with the x and y offsets
            Vector3 spawnPosition = spawnPoint.position + new Vector3(xOffset, yOffset, 0f);
            Quaternion obstacleRotation = Quaternion.Euler(-90f, 0, 0);

            // Randomly choose which obstacle to spawn
            GameObject[] obstaclePrefabs = new GameObject[] { obstacleNormal, obstacleNormal2, obstacleTall, obstacleTall2, obstacleLarge, obstacleLarge2};

            
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            // Instantiate the chosen obstacle at the calculated position
            GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, obstacleRotation);
        }
    }

    
    IEnumerator SpawnCoins()
    {
        while (true)
        {
            float waitTime = Random.Range(1f, 2f); // Adjust wait time as needed

            yield return new WaitForSeconds(waitTime);

            // Generate random x and y offsets among the specified values
            float[] possibleXOffsets = new float[] { -1.0f, -0.5f, 0.0f, 0.5f, 1.0f };
            float xOffset = possibleXOffsets[Random.Range(0, possibleXOffsets.Length)];

            // Randomly determine the y-coordinate based on the chance
            float yOffset = Random.value <= chanceToSpawnAt1Y ? 1.0f : 0.0f;

            // Create a new position with the x and y offsets
            Vector3 spawnPosition = spawnPoint.position + new Vector3(xOffset, yOffset, 0f);

            // Check for collisions with obstacles and helmets at the spawn position
            Collider2D[] colliders = Physics2D.OverlapBoxAll(spawnPosition, new Vector2(0.5f, 0.5f), 0);
            bool canSpawnCoin = true;

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Obstacle") || collider.CompareTag("Coin"))
                {
                    canSpawnCoin = false;
                    break;
                }
            }

            if (canSpawnCoin)
            {
                Quaternion coinRotation = Quaternion.Euler(-90f, -90f, 90f);
                Instantiate(coin, spawnPosition, coinRotation);
            }
        }
    }




    IEnumerator SpawnHelmet()
    {
        while (true)
        {
            float waitTime = Random.Range(8f, 10f);

            yield return new WaitForSeconds(waitTime);

            // Generate random x and y offsets among the specified values
            float[] possibleXOffsets = new float[] { -1.0f, -0.5f, 0.0f, 0.5f, 1.0f };
            float xOffset = possibleXOffsets[Random.Range(0, possibleXOffsets.Length)];

            // Randomly determine the y-coordinate based on the chance
            float yOffset = Random.value <= chanceToSpawnAt1Y ? 1.0f : 0.0f;

            // Create a new position with the x and y offsets
            Vector3 spawnPosition = spawnPoint.position + new Vector3(xOffset, yOffset, 0f);

            Quaternion helmetRotation = Quaternion.Euler(-90f, 0, 0);
   

            // Instantiate the obstacle at the calculated position
            GameObject newHelmet = Instantiate(helmet, spawnPosition, helmetRotation);

        }
    }
    public void UpdateHelmetCount()
    {
        progress = player.GetComponent<PlayerControler>().lives;
        if (progress <= 10) { slider.value = progress; }
    }
    public void UpdateAudiot()
    {

       
    }


    // Helper method to destroy all game objects with a specific tag
    private void DestroyAllGameObjects(string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }

    void ScoreUp() {

        score++;
        scoreText.text = score.ToString();
        scoreText2.text = score.ToString();
        

    }
    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
        scoreText2.text = score.ToString();
    }
    public void GameStart()
    {
        player.SetActive(true);
        menuCanvas.SetActive(false);
        scores.SetActive(true);
        player.GetComponent<PlayerControler>().gameManager = this; // Set the reference to the GameManager
        StartCoroutine("SpawnHelmet");
        StartCoroutine("SpawnObstacles");
        StartCoroutine("SpawnCoins");
        InvokeRepeating("ScoreUp", 2f, 1f);
    }
}
