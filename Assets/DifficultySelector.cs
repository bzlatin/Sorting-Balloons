using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    void Start()
    {
        int saved = PlayerPrefs.GetInt("Difficulty", 0);
        SetDifficulty(saved); // highlight saved choice
    }

    public void SetDifficulty(int level)
    {
        PlayerPrefs.SetInt("Difficulty", level);
        UpdateButtonColors(level);
    }

    private void UpdateButtonColors(int selected)
    {
        easyButton.image.color = selected == 0 ? selectedColor : defaultColor;
        mediumButton.image.color = selected == 1 ? selectedColor : defaultColor;
        hardButton.image.color = selected == 2 ? selectedColor : defaultColor;
    }
}
