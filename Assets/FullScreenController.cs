using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FullScreenController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leftPanel;
    public GameObject rightPanel;
    public Button algorithmButton;
    public Button leftArrow;
    public Button rightArrow;

    [Header("Level Info")]
    public List<SortingLevel> levels = new();

    private int currentIndex = 0;
    private RectTransform rightPanelRT;
    private Vector2 rightPanelOriginalAnchoredPosition;
    private Vector2 rightPanelOriginalAnchorMin;
    private Vector2 rightPanelOriginalAnchorMax;
    private Transform rightPanelOriginalParent;
    private Vector2 rightPanelOriginalPivot;
    private Vector2 rightPanelOriginalSizeDelta;
    private Canvas targetCanvas;
    private TMP_Text algorithmButtonText;

    void Start()
    {
        InitializeReferences();
        VerifyComponents();
        SetupButtonListeners();
        SetLevel(0); // show first level by default
    }

    void InitializeReferences()
    {
        rightPanelRT = rightPanel.GetComponent<RectTransform>();
        rightPanelOriginalParent = rightPanelRT.parent;
        rightPanelOriginalAnchorMin = rightPanelRT.anchorMin;
        rightPanelOriginalAnchorMax = rightPanelRT.anchorMax;
        rightPanelOriginalAnchoredPosition = rightPanelRT.anchoredPosition;
        rightPanelOriginalPivot = rightPanelRT.pivot;
        rightPanelOriginalSizeDelta = rightPanelRT.sizeDelta;

        targetCanvas = rightPanelRT.GetComponentInParent<Canvas>();
        algorithmButtonText = algorithmButton.GetComponentInChildren<TMP_Text>();
    }

    void VerifyComponents()
    {
        if (leftPanel == null || rightPanel == null || algorithmButton == null || leftArrow == null || rightArrow == null || targetCanvas == null)
        {
            Debug.LogError("Missing required UI references.");
            enabled = false;
        }
    }

    void SetupButtonListeners()
    {
        algorithmButton.onClick.RemoveAllListeners();
        leftArrow.onClick.RemoveAllListeners();
        rightArrow.onClick.RemoveAllListeners();

        algorithmButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("SortType", levels[currentIndex].sortTypeKey);
            SceneManager.LoadScene("InGame");
        });

        leftArrow.onClick.AddListener(() => CycleLevel(-1));
        rightArrow.onClick.AddListener(() => CycleLevel(1));
    }

    void CycleLevel(int direction)
    {
        levels[currentIndex].panel.SetActive(false);
        currentIndex = (currentIndex + direction + levels.Count) % levels.Count;
        SetLevel(currentIndex);
    }

    void SetLevel(int index)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].panel.SetActive(i == index);
        }

        algorithmButtonText.text = "Start " + levels[index].levelName;
    }

    void EnterFullscreenMode()
    {
        rightPanelRT.SetParent(targetCanvas.transform, false);
        rightPanelRT.anchorMin = Vector2.zero;
        rightPanelRT.anchorMax = Vector2.one;
        rightPanelRT.pivot = new Vector2(0.5f, 0.5f);
        rightPanelRT.anchoredPosition = Vector2.zero;
        rightPanelRT.sizeDelta = Vector2.zero;

        leftPanel.SetActive(false);
    }

    void ExitFullscreenMode()
    {
        rightPanelRT.SetParent(rightPanelOriginalParent, false);
        rightPanelRT.anchorMin = rightPanelOriginalAnchorMin;
        rightPanelRT.anchorMax = rightPanelOriginalAnchorMax;
        rightPanelRT.anchoredPosition = rightPanelOriginalAnchoredPosition;
        rightPanelRT.pivot = rightPanelOriginalPivot;
        rightPanelRT.sizeDelta = rightPanelOriginalSizeDelta;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rightPanelRT);
        Canvas.ForceUpdateCanvases();
        leftPanel.SetActive(true);
    }
}
