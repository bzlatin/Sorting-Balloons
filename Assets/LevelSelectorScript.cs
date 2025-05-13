    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;

    public class LevelSelectController : MonoBehaviour
    {
        [Header("UI References")]
        [Header("Background")]
        public Image backgroundImage;
        public Sprite bubbleSprite;
        public Sprite insertionSprite;
        public Sprite selectionSprite;
        public Sprite MergeSprite;
        public Button startButton;
        public Button leftArrow;
        public Button rightArrow;
        public TMP_Text startButtonText;        

        [Header("Instructions Panel")]
        public TextMeshProUGUI instructionsText; // Drag your panel’s TMP here

        [Header("Levels")]
        public List<SortingLevel> levels = new(); // Each has .panel and .sortTypeKey

        private int currentIndex = 0;
        private SortType activeSortType;           
        private string defaultInstructions;        

        void Start()
        {
            SetupButtons();
            UIAudioManager.Instance.PlayMenuMusic();


            // Remember whatever you typed into the Inspector
            if (instructionsText != null)
                defaultInstructions = instructionsText.text;

            // Optional: reset previous selection
            PlayerPrefs.DeleteKey("SortType");

            // Show the first level
            ShowLevel(0);
        }

        void SetupButtons()
        {
            if (startButton == null)      Debug.LogError("Start Button not assigned!");
            if (leftArrow == null)        Debug.LogError("Left Arrow not assigned!");
            if (rightArrow == null)       Debug.LogError("Right Arrow not assigned!");
            if (instructionsText == null) Debug.LogError("Instructions Text not assigned!");
        }

        public void CycleLeft()  => Cycle(-1);
        public void CycleRight() => Cycle(1);

        void Cycle(int direction)
        {
            if (levels.Count == 0) return;

            levels[currentIndex].panel.SetActive(false);
            currentIndex = (currentIndex + direction + levels.Count) % levels.Count;
            ShowLevel(currentIndex);
        }

        void ShowLevel(int index)
        {
            string key = levels[index].sortTypeKey;
            // 1) Activate only this panel
            for (int i = 0; i < levels.Count; i++)
                levels[i].panel.SetActive(i == index);

            // 2) Set the Start button label
            if (startButtonText != null){
                startButtonText.text = "Start " + levels[index].levelName;
            }
            if (backgroundImage != null){
                switch (key)
                {
                    case "Bubble":
                        backgroundImage.sprite = bubbleSprite;
                        break;
                    case "Insertion":
                        backgroundImage.sprite = insertionSprite;
                        break;
                    case "Selection":
                        backgroundImage.sprite = selectionSprite;
                        break;
                    case "Merge":
                        backgroundImage.sprite = MergeSprite;
                        break;                     
                    default:
                        Debug.LogWarning("No matching background for sortType: " + key);
                        break;
                }
            }
            

            // 3) Figure out its SortType
            if      (key == "Insertion") activeSortType = SortType.Insertion;
            else if (key == "Selection") activeSortType = SortType.Selection;
            else if (key == "Merge")     activeSortType = SortType.Merge;
            else                          activeSortType = SortType.Bubble;

            // 4) Override instructions only for Merge
            if (activeSortType == SortType.Merge)
            {
                instructionsText.text =
                    "Press the ARROW KEY corresponding to the side\n" +
                    "that the smaller highlighted element is on\n" +
                    "or the non-exhausted side.";
            }
            else
            {
                // Restore whatever you set up in the Inspector
                instructionsText.text = defaultInstructions;
            }
        }

        public void StartSelectedLevel()
        {
            PlayerPrefs.SetInt("CurrentLevelIndex", currentIndex);
            PlayerPrefs.SetString("SortType", levels[currentIndex].sortTypeKey);
            PlayerPrefs.SetString("Algo", levels[currentIndex].sortTypeKey + "Sort");
            PlayerPrefs.Save();
            SceneManager.LoadScene("InGame");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CycleLeft();
                UIAudioManager.Instance?.SwitchLevelButtonClick();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CycleRight();
                UIAudioManager.Instance?.SwitchLevelButtonClick();
            }
        }

        public void StartNextLevel()
        {
            int nextIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0) + 1;
            if (nextIndex >= levels.Count)
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }
            PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
            PlayerPrefs.SetString("SortType", levels[nextIndex].sortTypeKey);
            PlayerPrefs.Save();
            SceneManager.LoadScene("InGame");
        }
    }
