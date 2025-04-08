using UnityEngine;

public class BalloonHighlight : MonoBehaviour
{
    private SpriteRenderer sr;

    public Sprite normalSprite;
    public Sprite highlightSprite;
    public Sprite winnerSprite;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("SpriteRenderer missing from balloon or children.");
    }

    public void SetNormal()
    {
        if (sr != null && normalSprite != null)
            sr.sprite = normalSprite;
    }

    public void SetHighlighted()
    {
        if (sr != null && highlightSprite != null)
            sr.sprite = highlightSprite;
    }

    public void SetWinner() 
    {
        if (sr != null && winnerSprite != null)
            sr.sprite = winnerSprite;        
    }

   
}
