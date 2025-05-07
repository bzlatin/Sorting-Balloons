using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAlgorithmController : MonoBehaviour
{
    public LevelSelectController levelSelector; // Only needed if you want to reuse level list, optional

    public void LoadNextAlgorithm()
    {
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        int nextIndex = currentIndex + 1;

        // Optional: Load levels from a shared manager or recreate the list
        string[] sortTypes = { "BubbleSort","InsertionSort", "SelectionSort"};
        
        if (nextIndex < sortTypes.Length)
        {
            PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
            PlayerPrefs.SetString("SortType", sortTypes[nextIndex]);
            PlayerPrefs.Save();

            SceneManager.LoadScene("InGame");
        }
        else
        {
            Debug.Log("All levels completed. Returning to main menu.");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
