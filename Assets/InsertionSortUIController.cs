using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InsertionSortUIController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> balloons;   // set by spawner

    [Header("References")]
    public BallonSpawnerUI_Insertion spawner;

    [Header("Animation")]
    public float stepInterval = 1.5f;

    [Header("Cycle")]
    public float restartDelay = 2f;

    private int outer;        // index being inserted
    private int inner;        // scans left
    private bool isSwapping;  // true while swap coroutine runs
    private Coroutine autoRoutine;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("InsertionSortUIController: balloon list null or too small.");
            return;
        }

        outer      = 1;
        inner      = outer;
        isSwapping = false;

        UpdateHighlight();

        if (autoRoutine != null) StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }

    private IEnumerator AutoSortLoop()
    {
        yield return new WaitForSeconds(restartDelay); 
        yield return new WaitForSeconds(stepInterval);

        while (outer < balloons.Count)
        {
            yield return new WaitUntil(() => !isSwapping);

            StepAutomatic();

            yield return new WaitForSeconds(stepInterval);
        }

        ClearAllHighlights();
        SetAllFinal();

        yield return new WaitForSeconds(restartDelay);

        if (spawner != null)
            spawner.ResetSpawnerUI();
        else
            Debug.LogWarning("InsertionSortUIController: no spawner reference set.");
    }

    private void StepAutomatic()
    {
        if (inner > 0)
        {
            int left  = GetValue(inner - 1);
            int right = GetValue(inner);

            if (left > right)
            {
                Swap(inner - 1, inner);
                return;   // wait for swap coroutine
            }
        }

        AdvanceOuter();
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

        inner--;
        UpdateHighlight();
    }

    private void AdvanceOuter()
    {
        outer++;
        inner = outer;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (outer >= balloons.Count) { ClearAllHighlights(); return; }

        int left  = Mathf.Max(inner - 1, 0);
        int right = Mathf.Clamp(inner, 0, balloons.Count - 1);

        SetHighlighted(left, right);
    }

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
            go.GetComponentInChildren<BalloonHighlightUI>()?.SetHighlighted();
    }
}
