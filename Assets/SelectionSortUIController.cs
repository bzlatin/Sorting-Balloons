using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SelectionSortUIController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> balloons;

    [Header("References")]
    public BallonSpawnerUI_Selection spawner;

    [Header("Animation")]
    public float stepInterval = 1.5f;   

    [Header("Cycle")]
    public float restartDelay = 2f;

    private int  outer;        
    private int  scan;         
    private int  minIndex;    
    private bool isSwapping;
    private Coroutine autoRoutine;

    #region Public API
    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("SelectionSortUIController: balloon list null or too small.");
            return;
        }

        outer      = 0;
        scan       = outer + 1;
        minIndex   = outer;
        isSwapping = false;

        UpdateHighlight();

        if (autoRoutine != null) StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }
    #endregion

    #region Main loop
    private IEnumerator AutoSortLoop()
    {
        yield return new WaitForSeconds(restartDelay); 
        yield return new WaitForSeconds(stepInterval);

        while (outer < balloons.Count - 1)
        {
            yield return new WaitUntil(() => !isSwapping);

            StepAutomatic();

            yield return new WaitForSeconds(stepInterval);
        }

        ClearAllHighlights();
        SetAllFinal();

        yield return new WaitForSeconds(restartDelay);

        if (spawner != null) spawner.ResetSpawnerUI();
        else                 Debug.LogWarning("SelectionSortUIController: no spawner set.");
    }

    private void StepAutomatic()
    {

        if (scan < balloons.Count)
        {
            int valScan = GetValue(scan);
            int valMin  = GetValue(minIndex);

            if (valScan < valMin) minIndex = scan;

            scan++;
            UpdateHighlight();
            return;                         // continue scanning on next tick
        }

        /* 2️⃣  Finished scanning – swap if needed, then advance outer */
        if (minIndex != outer)
        {
            Swap(outer, minIndex);          // async; AdvanceOuter() after swap
        }
        else
        {
            AdvanceOuter();
        }
    }
    #endregion

    #region Helpers
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

        AdvanceOuter();                     // continue algorithm
    }

    private void AdvanceOuter()
    {
        outer++;
        scan     = outer + 1;
        minIndex = outer;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        // Highlight 'minIndex' (current best) and 'scan' (current candidate)
        int partner = (scan < balloons.Count) ? scan : minIndex;
        SetHighlighted(minIndex, partner);
    }

    private void SetHighlighted(int a, int b)
    {
        for (int k = 0; k < balloons.Count; k++)
        {
            var h = balloons[k].GetComponentInChildren<BalloonHighlightUI>();
            if (h == null) continue;

            if (k == a || k == b) h.SetHighlighted();   // yellow
            else                  h.SetNormal();        // neutral
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
            go.GetComponentInChildren<BalloonHighlightUI>()?.SetHighlighted();
    }
    #endregion
}
