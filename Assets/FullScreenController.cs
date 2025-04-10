using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FullScreenController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leftPanel;
    public GameObject rightPanel;
    public Button algorithmButton;  // This is your algorithm start button
    public Button backButton;       // New Back button

    // (Optional) Keep the normal text if you want to update the algorithm button text.
    public string normalText = "QUICKSORT";

    // RectTransform caching
    private RectTransform rightPanelRT;
    private Vector2 rightPanelOriginalAnchoredPosition;
    private Vector2 rightPanelOriginalAnchorMin;
    private Vector2 rightPanelOriginalAnchorMax;
    private Transform rightPanelOriginalParent;
    private Vector2 rightPanelOriginalPivot;
    private Vector2 rightPanelOriginalSizeDelta;

    // UI Components
    private TMP_Text algorithmButtonText;  // Optional if you want to update text
    private Canvas targetCanvas;

    void Start()
    {
        InitializeReferences();
        VerifyComponents();
        SetupButtonListeners();

        // Hide the Back button initially
        backButton.gameObject.SetActive(false);
    }

    void InitializeReferences()
    {
        try
        {
            rightPanelRT = rightPanel.GetComponent<RectTransform>();

            // Cache all original layout values of rightPanel
            rightPanelOriginalParent = rightPanelRT.parent;
            rightPanelOriginalAnchorMin = rightPanelRT.anchorMin;
            rightPanelOriginalAnchorMax = rightPanelRT.anchorMax;
            rightPanelOriginalAnchoredPosition = rightPanelRT.anchoredPosition;
            rightPanelOriginalPivot = rightPanelRT.pivot;
            rightPanelOriginalSizeDelta = rightPanelRT.sizeDelta;

            targetCanvas = rightPanelRT.GetComponentInParent<Canvas>();

            // (Optional) Cache the algorithm button's text if needed
            algorithmButtonText = algorithmButton.GetComponentInChildren<TMP_Text>();

            Debug.Log("Original layout cached successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Initialization failed: {e.Message}");
        }
    }

    void VerifyComponents()
    {
        bool isValid = true;
        string errorMessage = "Missing references:";

        if (leftPanel == null) { errorMessage += "\n- LeftPanel"; isValid = false; }
        if (rightPanel == null) { errorMessage += "\n- RightPanel"; isValid = false; }
        if (algorithmButton == null) { errorMessage += "\n- AlgorithmButton"; isValid = false; }
        if (backButton == null) { errorMessage += "\n- BackButton"; isValid = false; }
        if (targetCanvas == null) { errorMessage += "\n- Canvas"; isValid = false; }

        if (!isValid)
        {
            Debug.LogError(errorMessage);
            enabled = false;
        }
    }

    void SetupButtonListeners()
    {
        // Remove all existing listeners
        algorithmButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();

        // When algorithm button is clicked, enter fullscreen, hide itself, show back button, and start timer.
        algorithmButton.onClick.AddListener(() =>
        {
            EnterFullscreenMode();
            algorithmButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
        });

        // When back button is clicked, exit fullscreen, hide itself, show algorithm button, and stop timer.
        backButton.onClick.AddListener(() =>
        {
            ExitFullscreenMode();
            backButton.gameObject.SetActive(false);
            algorithmButton.gameObject.SetActive(true);
        });
    }

    void EnterFullscreenMode()
    {
        // Change rightPanel layout to fill the screen
        rightPanelRT.SetParent(targetCanvas.transform, false);
        rightPanelRT.anchorMin = Vector2.zero;
        rightPanelRT.anchorMax = Vector2.one;
        rightPanelRT.pivot = new Vector2(0.5f, 0.5f);
        rightPanelRT.anchoredPosition = Vector2.zero;
        rightPanelRT.sizeDelta = Vector2.zero;

        // Hide the left panel
        leftPanel.SetActive(false);

        Debug.Log("Entered fullscreen mode");
    }

    void ExitFullscreenMode()
    {
        // Restore the original layout of rightPanel
        rightPanelRT.SetParent(rightPanelOriginalParent, false);
        rightPanelRT.anchorMin = rightPanelOriginalAnchorMin;
        rightPanelRT.anchorMax = rightPanelOriginalAnchorMax;
        rightPanelRT.anchoredPosition = rightPanelOriginalAnchoredPosition;
        rightPanelRT.pivot = rightPanelOriginalPivot;
        rightPanelRT.sizeDelta = rightPanelOriginalSizeDelta;

        // Force UI update
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightPanelRT);
        Canvas.ForceUpdateCanvases();

        // Show the left panel again
        leftPanel.SetActive(true);

        Debug.Log("Restored original layout");
    }

}
