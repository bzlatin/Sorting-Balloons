using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleSortController : MonoBehaviour
{
    public List<GameObject> balloons;
    public GameManager gameManager;

    private int index = 0;
    private int end;
    private bool isSwapping = false;
    private bool isActive = true;

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
        if (!isActive || isSwapping || index + 1 >= end) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left pressed");
            HandleAction(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right pressed");
            HandleAction(false);
        }
    }

    void HandleAction(bool attemptSwap)
    {
        int a = GetValue(index);
        int b = GetValue(index + 1);

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
        var tmp = balloons[i].GetComponentInChildren<TextMeshPro>();
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
        yield return new WaitForSeconds(0.05f);

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
        for (int i = 0; i < balloons.Count - 1; i++)
        {
            if (GetValue(i) > GetValue(i + 1))
                return false;
        }

        return true;
    }

    void HighlightAsWinners()
    {
        foreach (var b in balloons)
            b.GetComponent<BalloonHighlight>()?.SetWinner();
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < balloons.Count; i++)
        {
            var highlight = balloons[i].GetComponent<BalloonHighlight>();
            if (highlight == null) continue;

            if (i == index || i == index + 1)
                highlight.SetHighlighted();
            else
                highlight.SetNormal();
        }
    }

    public void DisableInput() => isActive = false;

    public void ResetSorting()
    {        
        Initialize();
    }
}
