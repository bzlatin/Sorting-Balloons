using UnityEngine;
using UnityEngine.SceneManagement;

public class AboutMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Reset in case it was paused
        SceneManager.LoadScene("MainMenu");
    }
}
