using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    public GameObject winPanel;
    public TextMeshProUGUI statusText;
    public Timer timer;
    public LifeSystem lifeSystem;
    public BubbleSortController sortController;
    private bool isPaused = false;
    public GameObject pausePanel;

    void Start()
    {
        // Ensure game time is running
        Time.timeScale = 1f;
        isPaused = false;

        // Hide pause panel if it is visible
        if (pausePanel != null)
            pausePanel.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }



    public void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    public void TogglePause()
    {
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
        // Unpause time in case it was paused
        Time.timeScale = 1f;
        isPaused = false;

        // Hide pause panel if active
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateStatus("");

        timer?.ResetTimer();
        sortController?.ResetSorting();
        lifeSystem?.ResetLives();
    }
}
