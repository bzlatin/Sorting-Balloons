
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelectorScript : MonoBehaviour
{
    // This function is called when the user clicks the QuickSort button
    public void OnBubbleSortButtonClick()
    {
       // Get the scene using its name.
        Scene inGameScene = SceneManager.GetSceneByName("InGame");

        // If the scene is not valid or not loaded, then load it additively.
        if (!inGameScene.isLoaded)
        {
            SceneManager.LoadScene("InGame", LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log("InGame scene is already loaded.");
        }
    }
}
