using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class LevelSelectController : MonoBehaviour
{
    [Header("UI References")]
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


        if (startButtonText != null)
            startButtonText.text = "Start " + levels[index].levelName;
    }

    public void StartSelectedLevel()
    {
        Debug.Log("StartSelectedLevel triggered");

        PlayerPrefs.SetString("SortType", levels[currentIndex].sortTypeKey);
        PlayerPrefs.Save();
        SceneManager.LoadScene("InGame");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CycleLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CycleRight();
        }
    }


}
