using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject gameOverPanel;

    public TextMeshProUGUI statusText;
    public Timer timer;
    public LifeSystem lifeSystem;
    public GameObject pausePanel;
    public BalloonSpawner balloonSpawner;

    public BubbleSortController bubbleSortController;
    public InsertionSortController insertionSortController;
    public SelectionSortController selectionSortController;


    private Dictionary<SortType, MonoBehaviour> sortControllers = new();
    private SortType activeSortType;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        // Map available sort controllers
        sortControllers = new Dictionary<SortType, MonoBehaviour>
        {
            { SortType.Bubble, bubbleSortController },
            { SortType.Insertion, insertionSortController },
            { SortType.Selection, selectionSortController } 

        };



        // Determine selected sort type
        string type = PlayerPrefs.GetString("SortType", "Bubble");

        if (type == "Insertion")
            activeSortType = SortType.Insertion;
        else if (type == "Selection")
            activeSortType = SortType.Selection;
        else
            activeSortType = SortType.Bubble;


        // Enable only the selected controller
        foreach (var kvp in sortControllers)
        {
            bool isActive = kvp.Key == activeSortType;
            if (kvp.Value != null)
                kvp.Value.gameObject.SetActive(isActive);
        }

        // BalloonSpawner will call InitializeActiveSort after spawning
    }

    public void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

   public void TogglePause()
    {
        if ((winPanel != null && winPanel.activeInHierarchy) ||
            (gameOverPanel != null && gameOverPanel.activeInHierarchy))
        {
            Debug.Log("Pause disabled because win or game over panel is active.");
            return;
        }

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }


    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void OnPlayerMistake(string message)
    {
        UpdateStatus(message);
        lifeSystem.LoseLife();
    }

    public void OnLevelComplete()
    {
        UpdateStatus("Sorted!");
        ShowWinScreen();
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        Debug.Log("Showing win screen!");
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        UpdateStatus("");

        // Disable all controllers
        foreach (var controller in sortControllers.Values)
        {
            if (controller is BubbleSortController bubble)
                bubble.DisableInput();
            else if (controller is InsertionSortController insertion)
                insertion.DisableInput();
            else if (controller is SelectionSortController selection)
                selection.DisableInput();
        }

        // Enable only active controller
        foreach (var kvp in sortControllers)
        {
            bool isActive = kvp.Key == activeSortType;
            if (kvp.Value != null)
                kvp.Value.gameObject.SetActive(isActive);
        }

        StopAllCoroutines();
        timer?.ResetTimer();
        lifeSystem?.ResetLives();

        // BalloonSpawner will call InitializeActiveSort when it's done
        balloonSpawner?.ResetSpawner();
    }

    public void InitializeActiveSort(List<GameObject> balloons)
    {
        if (sortControllers.TryGetValue(activeSortType, out var controller))
        {
            if (controller == null || !controller.gameObject.activeInHierarchy)
            {
                Debug.LogWarning($"Sort controller for {activeSortType} is not active. Skipping init.");
                return;
            }

            if (controller is BubbleSortController bubble)
            {
                bubble.balloons = balloons;
                bubble.Initialize();
            }
            else if (controller is InsertionSortController insertion)
            {
                insertion.balloons = balloons;
                insertion.Initialize();
            } else if (controller is SelectionSortController selection)
            {
                selection.balloons = balloons;
                selection.Initialize();
            }

        }
    }
}
