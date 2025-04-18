using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BubbleSortController : MonoBehaviour
{
    [Header("Injected at runtime")]
    public List<GameObject> balloons;   
    public GameManager      gameManager;

    private int  index;        
    private int  end;          
    private bool isSwapping;
    private bool isActive;

    

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("BubbleSortManager: balloon list null or too small.");
            return;
        }

        index = 0;
        end   = balloons.Count;
        isActive   = true;
        isSwapping = false;

        UpdateHighlight();
    }

    public void DisableInput() => isActive = false;
    public void ResetSorting() => Initialize();

    

    private void Update()
    {
        if (!isActive || isSwapping || index + 1 >= end) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            HandleAction(attemptSwap: true);          // player *wants* to swap
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            HandleAction(attemptSwap: false);         // player *skips* swap
    }

    

    private void HandleAction(bool attemptSwap)
    {
        int a = GetValue(index);
        int b = GetValue(index + 1);
        bool shouldSwap = a > b;

        if (shouldSwap && !attemptSwap)
        {
            gameManager.OnPlayerMistake("You should have swapped!");
            return;
        }
        if (!shouldSwap && attemptSwap)
        {
            gameManager.OnPlayerMistake("You shouldn't have swapped!");
            return;
        }

        if (shouldSwap)
        {
            gameManager.UpdateStatus($"Swapped {a} and {b}");
            Swap(index, index + 1);
        }
        else
        {
            gameManager.UpdateStatus($"Correctly skipped {a} and {b}");
            AdvanceIndices();
        }
    }

    private int GetValue(int i)
    {
        TextMeshPro tmp = balloons[i].GetComponentInChildren<TextMeshPro>();
        if (tmp == null)
        {
            Debug.LogError($"Balloon at index {i} is missing a TextMeshPro component!");
            return 0;
        }
        return int.Parse(tmp.text);
    }

  

    private void Swap(int i, int j)
    {
        if (!isSwapping)
            StartCoroutine(SwapVisuals(i, j));
    }

    private IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        Transform aT = balloons[i].transform;
        Transform bT = balloons[j].transform;
        Vector3 startA = aT.position;
        Vector3 startB = bT.position;

        float dur = 0.25f, t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            aT.position = Vector3.Lerp(startA, startB, u);
            bT.position = Vector3.Lerp(startB, startA, u);
            yield return null;
        }

        aT.position = startB;
        bT.position = startA;
        (balloons[i], balloons[j]) = (balloons[j], balloons[i]);

        isSwapping = false;
        AdvanceIndices();
    }

   

    private void AdvanceIndices()
    {
        index++;
        if (index + 1 >= end)
        {
            index = 0;
            end--;
        }

        UpdateHighlight();

        if (end <= 1 || IsSorted())
        {
            HighlightAsWinners();
            isActive = false;
            gameManager.OnLevelComplete();
        }
    }

    private bool IsSorted()
    {
        for (int k = 0; k < balloons.Count - 1; k++)
            if (GetValue(k) > GetValue(k + 1))
                return false;
        return true;
    }

    private void UpdateHighlight()
    {
        for (int k = 0; k < balloons.Count; k++)
        {
            var h = balloons[k].GetComponent<BalloonHighlight>();
            if (h == null) continue;

            if (k == index || k == index + 1)
                h.SetHighlighted();
            else
                h.SetNormal();
        }
    }

    private void HighlightAsWinners()
    {
        foreach (var b in balloons)
            b.GetComponent<BalloonHighlight>()?.SetWinner();
    }
}
