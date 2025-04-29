using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class LevelSelectController : MonoBehaviour
{
    [Header("UI References")]
    [Header("Background")]
    public Image backgroundImage;
    public Sprite bubbleSprite;
    public Sprite insertionSprite;
    public Sprite selectionSprite;
    public Button startButton;
    public Button leftArrow;
    public Button rightArrow;
    public TMP_Text startButtonText;        // Optional button label override

    [Header("Levels")]
    public List<SortingLevel> levels = new();

    private int currentIndex = 0;

    void Start()
    {
        SetupButtons();

        // Optional: reset previous selection
        PlayerPrefs.DeleteKey("SortType");

        // Set default view
        ShowLevel(0);
    }

    void SetupButtons()
    {

        if (startButton == null) Debug.LogError("Start Button not assigned!");
        if (leftArrow == null) Debug.LogError("Left Arrow not assigned!");
        if (rightArrow == null) Debug.LogError("Right Arrow not assigned!");
    }

    public void CycleLeft()
    {
        Cycle(-1);
    }

    public void CycleRight()
    {
        Cycle(1);
    }

    void Cycle(int direction)
    {
        if (levels.Count == 0) return;

        levels[currentIndex].panel.SetActive(false);
        currentIndex = (currentIndex + direction + levels.Count) % levels.Count;
        ShowLevel(currentIndex);
    }

    void ShowLevel(int index)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].panel.SetActive(i == index);
        }


        if (startButtonText != null){
            startButtonText.text = "Start " + levels[index].levelName;
        }
        if (backgroundImage != null){
            string key = levels[index].sortTypeKey;

            switch (key)
            {
                case "Bubble":
                    backgroundImage.sprite = bubbleSprite;
                    break;
                case "Insertion":
                    backgroundImage.sprite = insertionSprite;
                    break;
                case "Selection":
                    backgroundImage.sprite = selectionSprite;
                    break;
                default:
                    Debug.LogWarning("No matching background for sortType: " + key);
                    break;
            }
        }
        
    }

    public void StartSelectedLevel()
    {
        Debug.Log("StartSelectedLevel triggered");
        PlayerPrefs.SetInt("CurrentLevelIndex", currentIndex); // Save index
        PlayerPrefs.SetString("SortType", levels[currentIndex].sortTypeKey);
        PlayerPrefs.Save();
        SceneManager.LoadScene("InGame");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CycleLeft();
            UIAudioManager.Instance?.SwitchLevelButtonClick(); // 🔊 play sound
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CycleRight();
            UIAudioManager.Instance?.SwitchLevelButtonClick(); // 🔊 play sound
        }
    }
    public void StartNextLevel()
{
    Debug.Log("StartNextLevel triggered");

    int nextIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0) + 1;

    if (nextIndex >= levels.Count)
    {
        Debug.Log("No more levels. Returning to Main Menu or showing end screen.");
        SceneManager.LoadScene("MainMenu"); // You can change this to "Credits" or a "Game Complete" screen
        return;
    }

    PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
    PlayerPrefs.SetString("SortType", levels[nextIndex].sortTypeKey);
    PlayerPrefs.Save();

    SceneManager.LoadScene("InGame");
}

}
