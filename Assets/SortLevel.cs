using UnityEngine;

[System.Serializable]
public class SortingLevel
{
    public string levelName;         // Display name (e.g., "Bubble Sort")
    public string sortTypeKey;       // PlayerPrefs key value (e.g., "Bubble", "Insertion")
    public GameObject panel;         // UI panel to show info
}
