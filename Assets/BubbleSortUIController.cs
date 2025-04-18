using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleSortUIController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> balloons;   // set by BalloonSpawnerUI

    [Header("References")]
    public BalloonSpawnerUI spawner;                      // drag your UI spawner here

    [Header("Animation")]
    public float stepInterval = 0.6f;                     // seconds between compare steps

    [Header("Cycle")]
    public float restartDelay = 1.0f;                     // pause before spawning next row

    private int  index;
    private int  end;
    private bool isSwapping;
    private Coroutine autoRoutine;


    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("BubbleSortUIController: balloon list null or too small.");
            return;
        }

        index      = 0;
        end        = balloons.Count;
        isSwapping = false;

        // Highlight the first pair immediately
        UpdateHighlight();

        // Restart the auto‑sort coroutine
        if (autoRoutine != null) StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }


    private IEnumerator AutoSortLoop()
    {
        // Wait a full stepInterval before the very first move
        yield return new WaitForSeconds(stepInterval);

        while (end > 1)
        {
            // Wait for any ongoing swap to finish
            yield return new WaitUntil(() => !isSwapping);

            // Perform one compare-and-swap step
            StepAutomatic();

            // Wait before the next step
            yield return new WaitForSeconds(stepInterval);
        }

        // Clear highlights when sorted
        ClearAllHighlights();

        // Wait before spawning a new row
        yield return new WaitForSeconds(restartDelay);

        // Spawn next batch and re-initialize
        if (spawner != null)
            spawner.ResetSpawnerUI();
        else
            Debug.LogWarning("BubbleSortUIController: no spawner reference set.");
    }


    private void StepAutomatic()
    {
        if (isSwapping || index + 1 >= end) return;

        int a = GetValue(index);
        int b = GetValue(index + 1);

        if (a > b) 
            Swap(index, index + 1);
        else       
            AdvanceIndices();
    }


    private int GetValue(int i) =>
        int.Parse(balloons[i].GetComponentInChildren<TextMeshProUGUI>().text);

    private void Swap(int i, int j)
    {
        if (!isSwapping)
            StartCoroutine(SwapVisuals(i, j));
    }

    private IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        // Keep the two balloons highlighted while they move
        SetHighlighted(i, j);

        RectTransform aRT = balloons[i].GetComponent<RectTransform>();
        RectTransform bRT = balloons[j].GetComponent<RectTransform>();
        Vector2 startA = aRT.anchoredPosition;
        Vector2 startB = bRT.anchoredPosition;

        const float DUR = 0.25f;
        float t = 0f;
        while (t < DUR)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / DUR);
            aRT.anchoredPosition = Vector2.Lerp(startA, startB, u);
            bRT.anchoredPosition = Vector2.Lerp(startB, startA, u);
            yield return null;
        }

        // Snap to final positions and swap references
        aRT.anchoredPosition = startB;
        bRT.anchoredPosition = startA;
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
    }

    

    private void UpdateHighlight() => SetHighlighted(index, index + 1);

    private void SetHighlighted(int a, int b)
    {
        for (int k = 0; k < balloons.Count; k++)
        {
            var h = balloons[k].GetComponent<BalloonHighlightUI>();
            if (h == null) continue;

            if (k == a || k == b) h.SetHighlighted();
            else                  h.SetNormal();
        }
    }

    private void ClearAllHighlights()
    {
        foreach (var go in balloons)
            go.GetComponent<BalloonHighlightUI>()?.SetNormal();
    }
}
