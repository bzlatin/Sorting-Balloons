using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleSortUIController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> balloons;

    [Header("References")]
    public BalloonSpawnerUI_Bubble spawner;

    [Header("Animation")]
    public float stepInterval = 0.6f;

    [Header("Cycle")]
    public float restartDelay = 1.0f;

    private int index;
    private int end;
    private bool isSwapping;
    private Coroutine autoRoutine;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("BubbleSortUIController: balloon list null or too small.");
            return;
        }

        index = 0;
        end = balloons.Count;
        isSwapping = false;

        UpdateHighlight();

        if (autoRoutine != null) StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }

    private IEnumerator AutoSortLoop()
    {
        yield return new WaitForSeconds(stepInterval);

        while (end > 1)
        {
            yield return new WaitUntil(() => !isSwapping);

            StepAutomatic();

            yield return new WaitForSeconds(stepInterval);
        }

        ClearAllHighlights();
        SetAllFinal(); // Mark all as sorted

        yield return new WaitForSeconds(restartDelay);

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
            var h = balloons[k].GetComponentInChildren<BalloonHighlightUI>();
            if (h == null) continue;

            if (k == a || k == b) h.SetHighlighted();
            else                  h.SetNormal();
        }
    }

    private void ClearAllHighlights()
    {
        foreach (var go in balloons)
            go.GetComponentInChildren<BalloonHighlightUI>()?.SetNormal();
    }

    
    private void SetAllFinal()
    {
        foreach (var go in balloons)
        {
            var h = go.GetComponentInChildren<BalloonHighlightUI>();
            h?.SetHighlighted(); 
        }
    }
    
    public void StopSorting()
    {
        if (autoRoutine != null)
        {
            StopCoroutine(autoRoutine);
            autoRoutine = null;
        }
        StopAllCoroutines();         
    }

    private void OnDisable() => StopSorting();
    

}
