using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LifeSystem : MonoBehaviour
{
    public static LifeSystem Instance { get; private set; }

    public int maxLives = 5;
    private int currentLives;

    public BubbleSortController bubbleSortController;
    public InsertionSortController insertionSortController;
    public SelectionSortController selectionSortController; 

    public TextMeshProUGUI livesText;
    public TextMeshProUGUI statusText;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ResetLives();
    }

    public void LoseLife()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        UpdateLivesUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }
    }

    void GameOver()
    {
        if (statusText != null)
            statusText.text = "Game Over!";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        bubbleSortController?.DisableInput();
        insertionSortController?.DisableInput();
        selectionSortController?.DisableInput(); // Added

        Time.timeScale = 0f;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
