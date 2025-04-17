using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InsertionSortController : MonoBehaviour
{
    public List<GameObject> balloons;
    public GameManager gameManager;

    private int i = 1;
    private int j;
    private bool isSwapping = false;
    private bool isActive = true;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("Balloon list not assigned or too short.");
            isActive = false; // Add this
            return;
        }

        i = 1;
        j = i;
        isActive = true;
        UpdateHighlight();
    }


    void Update()
    {
        if (!isActive || isSwapping) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))  // Try to shift left (swap)
        {
            HandleAction(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // Skip
        {
            HandleAction(false);
        }
    }

    void HandleAction(bool attemptSwap)
    {
        // Edge check
        if (j <= 0)
        {
            if (attemptSwap)
            {
                gameManager.OnPlayerMistake("No need to swap!");
            }
            else
            {
                gameManager.UpdateStatus($"Correctly skipped {GetValue(j)}");
            }

            i++;
            j = i;
            UpdateHighlight();

            if (i >= balloons.Count)
            {
                HighlightAsWinners();
                isActive = false;
                gameManager.OnLevelComplete();
            }

            return;
        }

        // Compare j and j-1
        if (GetValue(j - 1) <= GetValue(j))
        {
            if (attemptSwap)
            {
                gameManager.OnPlayerMistake("No need to swap!");
            }
            else
            {
                gameManager.UpdateStatus($"Correctly skipped {GetValue(j)}");
            }

            i++;
            j = i;
            UpdateHighlight();

            if (i >= balloons.Count)
            {
                HighlightAsWinners();
                isActive = false;
                gameManager.OnLevelComplete();
            }

            return;
        }

        // Needs a swap
        if (!attemptSwap)
        {
            gameManager.OnPlayerMistake("You should have swapped!");
            return;
        }

        Swap(j - 1, j);
        j--; // keep j here so player keeps comparing left until done
        gameManager.UpdateStatus($"Swapped {GetValue(j)} and {GetValue(j + 1)}");
    }




    int GetValue(int index)
    {
        if (index < 0 || index >= balloons.Count)
        {
            Debug.LogError($"Index {index} out of range for balloon list.");
            return 0;
        }

        var tmp = balloons[index].GetComponentInChildren<TextMeshPro>();
        return tmp != null ? int.Parse(tmp.text) : 0;
    }

    void Swap(int i, int j)
    {
        if (!isSwapping) StartCoroutine(SwapVisuals(i, j));
    }

    IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        GameObject a = balloons[i];
        GameObject b = balloons[j];

        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            a.transform.position = Vector3.Lerp(posA, posB, t);
            b.transform.position = Vector3.Lerp(posB, posA, t);
            yield return null;
        }

        a.transform.position = posB;
        b.transform.position = posA;
        (balloons[i], balloons[j]) = (balloons[j], balloons[i]);

        UpdateHighlight();
        isSwapping = false;
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

        if (k == j) // currently inserting
            highlight.SetHighlighted();
        else if (k < i) // already sorted
            highlight.SetDimmed(); // ✅ dimmed green
        else
            highlight.SetNormal();
    }
}



    public void DisableInput() => isActive = false;

    public void ResetSorting() => Initialize();
}
