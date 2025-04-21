using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BalloonHighlightUI : MonoBehaviour
{
    [Header("Sprite States")]
    public Sprite normalSprite;
    public Sprite highlightSprite;

    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        if (img == null)
            Debug.LogError("BalloonHighlightUI: no Image on " + name);
    }

    public void SetNormal()
    {
        if (img != null && normalSprite != null)
            img.sprite = normalSprite;
    }

    public void SetHighlighted()
    {
        if (img != null && highlightSprite != null)
            img.sprite = highlightSprite;
    }

    public void SetFinal()
    {
        if (img != null && highlightSprite != null)
            img.sprite = highlightSprite;
    }

}