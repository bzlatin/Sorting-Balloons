using UnityEngine;
using UnityEngine.SceneManagement;

public class NextAlgorithmHandler : MonoBehaviour
{
    // Matches what LevelSelector uses
    public string[] sortTypeKeys = { "Bubble", "Insertion", "Selection" };

    public void StartNextLevel()
    {
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        int nextIndex = currentIndex + 1;

        if (nextIndex >= sortTypeKeys.Length)
        {
            Debug.Log("No more levels. Going to Main Menu.");
            SceneManager.LoadScene("MainMenu"); // Or end scene
            return;
        }

        PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
        PlayerPrefs.SetString("SortType", sortTypeKeys[nextIndex]);
        PlayerPrefs.Save();

        Debug.Log("Next sort type is: " + sortTypeKeys[nextIndex]);

        SceneManager.LoadScene("InGame");
    }
}
