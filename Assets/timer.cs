using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.UI; // For UI elements

public class Timer : MonoBehaviour
{
    public float timeRemaining = 180f; // Set the initial time to 3 minutes (180 seconds)
    public TextMeshProUGUI timerText;  // Reference to the TextMeshProUGUI component to display the timer
    public GameObject gameOverPanel;   // Reference to the Game Over panel (UI)
    private bool isGameOver = false;


    void Start()
    {
        // Ensure the TextMeshProUGUI component is assigned
        if (timerText == null)
        {
            Debug.LogError("Timer TextMeshProUGUI is not assigned.");
        }

        // Ensure the GameOverPanel is inactive at the start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Hide Game Over panel initially
        }
    }

    void Update()
    {
        // If time remaining is greater than 0, keep counting down
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Subtract the elapsed time since last frame
            UpdateTimerText(); // Update the UI with the new time
        }
        else
        {
            // Trigger the Game Over panel and display "Time's Up!" when the timer reaches zero
            TriggerGameOver();
        }
    }

    // Update the UI text to reflect the current time remaining
    void UpdateTimerText()
    {
        // Format the timer as minutes:seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Show the Game Over panel and change the timer text
void TriggerGameOver()
{
    if (isGameOver) return;  // ✅ Prevent multiple triggers
    isGameOver = true;

    if (timerText != null)
    {
        timerText.text = "Time's Up!";
    }

    if (LifeSystem.Instance != null)
    {
        LifeSystem.Instance.GameOver();
    }

    Debug.Log("Game Over!");
}



    public void ResetTimer()
    {
        timeRemaining = 180f;
        UpdateTimerText();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }

}
