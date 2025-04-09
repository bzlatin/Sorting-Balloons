using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called when the user clicks the Play button
    public void PlayGame()
    {
        // Loads the LevelSelector scene by name
        SceneManager.LoadScene("LevelSelector");
    }

    // Called when the user clicks the Help button
    public void ShowHelp()
    {
        // Implement the logic you want for Help, e.g. show a panel or transition to a Help scene
        Debug.Log("Help button clicked!");
    }

    // Called when the user clicks the Quit button
    public void QuitGame()
    {
        // Quits the application (won't show effect in editor, but works in builds)
        Application.Quit();

        // In the editor, this is how you can simulate quitting:
        // UnityEditor.EditorApplication.isPlaying = false;
    }
}
