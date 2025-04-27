using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InstructionsTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panel;       // your InstructionsPanel
    [SerializeField] private TMP_Text instructions;  // existing instructions text
    [SerializeField] private TMP_Text algoLabel;     // the new line for algorithm

    public void OnPointerEnter(PointerEventData e)
    {
        panel.SetActive(true);

        string sortKey = PlayerPrefs.GetString("SortType", "");
        string pretty;

        if (string.IsNullOrEmpty(sortKey))
        {
            pretty = "Unknown";
        }
        else if (sortKey.EndsWith("Sort"))
        {
            // take everything before "Sort" and add a space + "Sort"
            pretty = sortKey.Substring(0, sortKey.Length - 4) + " Sort";
        }
        else
        {
            // fallback: just show the raw key
            pretty = sortKey;
        }

        algoLabel.text = $"<b>Current Algorithm:</b> {pretty}";
    }

    public void OnPointerExit(PointerEventData e)
    {
        panel.SetActive(false);
    }
}
