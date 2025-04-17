using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleSortController : MonoBehaviour
{
    [Header("Injected at runtime")]
    public List<GameObject> balloons;   // UI balloons under RightPanel
    public GameManager gameManager;     // handles status, mistakes, level complete

    private int index = 0;
    private int end;
    private bool isSwapping = false;
    private bool isActive = true;

    /// <summary>
    /// Call this after assigning balloons to reset state and highlight the first pair.
    /// </summary>
    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("Balloon list not assigned or too short.");
            return;
        }

        index = 0;
        end = balloons.Count;
        isActive = true;
        UpdateHighlight();
    }

    void Update()
    {
        if (!isActive || isSwapping || index + 1 >= end) 
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            HandleAction( attemptSwap: true );
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            HandleAction( attemptSwap: false );
    }

    void HandleAction(bool attemptSwap)
    {
        int a = GetValue(index);
        int b = GetValue(index + 1);

        // If a > b we should swap; if not, we should skip
        if (a > b)
        {
            if (!attemptSwap)
            {
                gameManager.OnPlayerMistake("You should have swapped!");
                return;
            }

            Swap(index, index + 1);
            gameManager.UpdateStatus($"Swapped {a} and {b}");
        }
        else
        {
            if (attemptSwap)
            {
                gameManager.OnPlayerMistake("You shouldn't have swapped!");
                return;
            }

            gameManager.UpdateStatus($"Correctly skipped {a} and {b}");
        }

        // Advance to next pair
        index++;
        if (index + 1 >= end)
        {
            index = 0;
            end--;
        }

        UpdateHighlight();
    }

    int GetValue(int i)
    {
        // Use TextMeshProUGUI for UI balloons
        var tmp = balloons[i].GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogError($"Balloon at index {i} missing TextMeshProUGUI");
            return 0;
        }
        return int.Parse(tmp.text);
    }

    void Swap(int i, int j)
    {
        if (!isSwapping)
            StartCoroutine(SwapVisuals(i, j));
    }

    IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        // Grab RectTransforms
        var aRT = balloons[i].GetComponent<RectTransform>();
        var bRT = balloons[j].GetComponent<RectTransform>();
        Vector2 startA = aRT.anchoredPosition;
        Vector2 startB = bRT.anchoredPosition;

        // Animate over 0.25s
        float duration = 0.25f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            aRT.anchoredPosition = Vector2.Lerp(startA, startB, t);
            bRT.anchoredPosition = Vector2.Lerp(startB, startA, t);
            yield return null;
        }

        // Snap to final
        aRT.anchoredPosition = startB;
        bRT.anchoredPosition = startA;

        // Swap in list
        (balloons[i], balloons[j]) = (balloons[j], balloons[i]);

        UpdateHighlight();
        yield return new WaitForSeconds(0.05f);

        // If sorted, highlight winners and finish
        if (IsSorted())
        {
            HighlightAsWinners();
            isActive = false;
            gameManager.OnLevelComplete();
        }

        isSwapping = false;
    }

    bool IsSorted()
    {
        for (int k = 0; k < balloons.Count - 1; k++)
            if (GetValue(k) > GetValue(k + 1))
                return false;
        return true;
    }

    void HighlightAsWinners()
    {
        foreach (var b in balloons)
            b.GetComponent<BalloonHighlight>()?.SetWinner();
    }

    void UpdateHighlight()
    {
        for (int k = 0; k < balloons.Count; k++)
        {
            var highlight = balloons[k].GetComponent<BalloonHighlight>();
            if (highlight == null) continue;

            if (k == index || k == index + 1)
                highlight.SetHighlighted();
            else
                highlight.SetNormal();
        }
    }

    /// <summary>
    /// Stops input processing mid‑sort.
    /// </summary>
    public void DisableInput() => isActive = false;

    /// <summary>
    /// Resets to the initial unsorted state.
    /// </summary>
    public void ResetSorting() => Initialize();
}
