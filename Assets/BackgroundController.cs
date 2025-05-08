using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [Header("UI References")]
    public Image bgImage;               

    [Header("Algorithm Backgrounds")]
    public Sprite bubbleBg;
    public Sprite mergeBg;
    public Sprite insertionBg;
    public Sprite selectionBg;

    void Start()
    {
        
        string algo = PlayerPrefs.GetString("Algo", "BubbleSort");

        // 2) Pick the matching sprite
        if (algo == "BubbleSort")
        {
            bgImage.sprite = bubbleBg;
        }
        else if (algo == "MergeSort")
        {
            bgImage.sprite = mergeBg;
        }
        else if (algo == "InsertionSort")
        {
            bgImage.sprite = insertionBg;
        }
        else if (algo == "SelectionSort")
        {
            bgImage.sprite = selectionBg;
        }
    }
}
