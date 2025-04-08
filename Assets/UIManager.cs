using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject winPanel;

    public void ShowWinScreen()
    {
        Debug.Log("Showing win screen!"); // ✅ Add this line to test
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

