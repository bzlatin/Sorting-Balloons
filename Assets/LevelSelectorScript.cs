using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorScript : MonoBehaviour
{
    // This function is called when the user clicks the QuickSort button
    public void OnBubbleSortButtonClick()
    {
        // Load the InGame scene
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}
