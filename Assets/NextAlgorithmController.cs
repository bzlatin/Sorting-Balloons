using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAlgorithmController : MonoBehaviour
{
    public void BackToLevelSelectNext()
    {
        // grab where we are now (default to 0)
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);

        // list must match what LevelSelectController expects
        string[] sortTypes = { "BubbleSort", "InsertionSort", "SelectionSort" };
        
        // wrap around if we’re at the end
        int nextIndex = (currentIndex + 1) % sortTypes.Length;

        // save the new index & sort type
        PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
        PlayerPrefs.SetString("SortType", sortTypes[nextIndex]);
        PlayerPrefs.Save();

        // go back to your level-select scene
        SceneManager.LoadScene("LevelSelector");
    }
}
