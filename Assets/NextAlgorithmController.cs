using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAlgorithmController : MonoBehaviour
{
    public void BackToLevelSelectNext()
    {
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);

        string[] sortTypes = { "BubbleSort", "InsertionSort", "SelectionSort" };

        int nextIndex = (currentIndex + 1) % sortTypes.Length;

        PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
        PlayerPrefs.SetString("SortType", sortTypes[nextIndex]);
        PlayerPrefs.Save();

        // ✅ Only load the scene here
        SceneManager.LoadScene("LevelSelector");
    }

}
