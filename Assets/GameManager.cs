using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("UI Texts")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI instructionsText;   // 🔧 New field

    [Header("Gameplay")]
    public Timer timer;
    public LifeSystem lifeSystem;
    public BalloonSpawner balloonSpawner;

    [Header("Sort Controllers")]
    public BubbleSortController bubbleSortController;
    public InsertionSortController insertionSortController;
    public SelectionSortController selectionSortController;
    public MergeSortController mergeSortController;

    private Dictionary<SortType, MonoBehaviour> sortControllers = new();
    private SortType activeSortType;
    private bool isPaused = false;





    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        // 1) Map controllers
        sortControllers = new Dictionary<SortType, MonoBehaviour>
        {
            { SortType.Bubble, bubbleSortController },
            { SortType.Insertion, insertionSortController },
            { SortType.Selection, selectionSortController },
            { SortType.Merge, mergeSortController }
        };

        // 2) Pick the active sort
        string type = PlayerPrefs.GetString("SortType", "Bubble");

        if (type == "Insertion")
            activeSortType = SortType.Insertion;
        else if (type == "Selection")
            activeSortType = SortType.Selection;
        else
            activeSortType = SortType.Bubble;


        // 3) Enable only that controller
        foreach (var kvp in sortControllers)
        {
            if (kvp.Value != null)
                kvp.Value.gameObject.SetActive(kvp.Key == activeSortType);
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
            return;

        if (isPaused) ResumeGame();
        else          PauseGame();
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
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
        PlayWinGameSound();
        Time.timeScale = 0f;
    }

    public void ShowWinScreen()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null)     pausePanel.SetActive(false);
        if (winPanel != null)       winPanel.SetActive(false);
        UpdateStatus("");

        // Disable all controllers
        foreach (var c in sortControllers.Values)
        {
            if (c is BubbleSortController b) b.DisableInput();
            if (c is InsertionSortController ins) ins.DisableInput();
            if (c is SelectionSortController sel) sel.DisableInput();
            if (c is MergeSortController m) m.DisableInput();
        }

        // Re‐enable only the active one
        foreach (var kvp in sortControllers)
        {
            if (kvp.Value != null)
                kvp.Value.gameObject.SetActive(kvp.Key == activeSortType);
        }

        StopAllCoroutines();
        timer?.ResetTimer();
        lifeSystem?.ResetLives();
        balloonSpawner?.ResetSpawner();
    }

    public void InitializeActiveSort(List<GameObject> balloons)
    {
        if (!sortControllers.TryGetValue(activeSortType, out var controller) ||
            controller == null || !controller.gameObject.activeInHierarchy)
            return;

        switch (controller)
        {
            case BubbleSortController b:
                b.balloons = balloons; b.Initialize(); break;
            case InsertionSortController ins:
                ins.balloons = balloons; ins.Initialize(); break;
            case SelectionSortController sel:
                sel.balloons = balloons; sel.Initialize(); break;
            case MergeSortController m:
                m.balloons = balloons; m.Initialize(); break;
        }
    }

    public void PlayCorrectSwapSound() => UIAudioManager.Instance?.PlayCorrectSwap();
    public void PlayWrongSwapSound()   => UIAudioManager.Instance?.PlayWrongSwap();
    public void PlayCorrectSkipSound() => UIAudioManager.Instance?.PlayCorrectSkip();
    public void PlayWinGameSound()     => UIAudioManager.Instance?.PlayWinSound();
}
