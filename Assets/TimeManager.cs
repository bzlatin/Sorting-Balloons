using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance; // Singleton instance

    [Header("UI Reference")]
    public TMP_Text timerText; // Assign your TextMeshProUGUI component in the Inspector

    private float timer;
    private bool isRunning;

    void Awake()
    {
        // Ensure there is only one instance of TimerManager.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method to start the timer
    public void StartTimer()
    {
        timer = 0f;
        isRunning = true;
    }

    // Call this method to stop the timer and reset it
    public void StopTimer()
    {
        isRunning = false;
        ResetTimer();
    }

    // Reset the timer to zero and update the UI
    public void ResetTimer()
    {
        timer = 0f;
        UpdateTimerUI();
    }

    void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // Updates the timer UI in "MM:SS" format
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Optional: Call this to get the current timer value when needed.
    public float GetCurrentTime()
    {
        return timer;
    }
}
