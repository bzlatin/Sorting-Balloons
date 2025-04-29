using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private string levelSelectorScene = "LevelSelector";

    [SerializeField] private CanvasGroup menuCanvas;          // drag your menu panel

    [SerializeField] private BalloonSpawnerUI_Bubble spawner;
    [SerializeField] private BallonSpawnerUI_Insertion spawner1;
    [SerializeField] private BallonSpawnerUI_Selection spawner2;
    [SerializeField] private BallonSpawnerUI_Merge spawner3;


    public void PlayGame()
    {
        // 1️⃣  If the LevelSelector scene isn’t loaded yet → load it.
        if (!SceneManager.GetSceneByName(levelSelectorScene).isLoaded)
        {
            Time.timeScale = 1f;                    // just in case we were paused
            SceneManager.LoadScene(levelSelectorScene, LoadSceneMode.Single);
            return;
        }

        // 2️⃣  Scene is already active → resume gameplay in-place.
        //     Hide the menu overlay & restart the sorter.
        if (menuCanvas != null){
            menuCanvas.gameObject.SetActive(false);
        }
            

        Time.timeScale = 1f;                        // un-pause time

        // respawn row + kick animation
        if (spawner != null){
            spawner.ResetSpawnerUI();
        }
          if (spawner1 != null){
            spawner1.ResetSpawnerUI();
        }
          if (spawner2 != null){
            spawner2.ResetSpawnerUI();
        }
          if (spawner3 != null){
            spawner3.ResetSpawnerUI();
        }

        else{
            Debug.LogWarning("MainMenu: Spawner reference missing.");
        }
    }

    
    public void ShowHelp()
    {
        Debug.Log("Help button clicked!");
        // TODO: enable a help panel or load a Help scene
    }

    
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
