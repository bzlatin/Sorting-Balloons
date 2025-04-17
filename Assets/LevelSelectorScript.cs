using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorScript : MonoBehaviour
{
    public void OnBubbleSortButtonClick()
    {
        PlayerPrefs.SetString("SortType", "Bubble"); 
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }

    public void OnInsertionSortButtonClick()
    {
        PlayerPrefs.SetString("SortType", "Insertion");
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}
