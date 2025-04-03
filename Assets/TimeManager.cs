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

    // Call this method to stop the timer
    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // Updates the timer UI, if assigned
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Format the timer to display seconds with two decimals.
            timerText.text = timer.ToString("F2") + " seconds";
        }
    }

    // Optional: Call this to get the current timer value when needed.
    public float GetCurrentTime()
    {
        return timer;
    }
}
