using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MergeSortUIController : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> balloons;

    [Header("References")]
    public BallonSpawnerUI_Merge spawner;

    [Header("Timing")]
    public float stepInterval = 1.0f;   // pause between moves
    public float restartDelay = 2.0f;   // wait before respawn

    private int runSize;
    private int leftStart;
    private bool isMerging;

    // for visual merge
    private List<GameObject> leftRun, rightRun;
    private int leftPtr, rightPtr, mergeIndex;
    private List<Vector2> originalPositions;

    private Coroutine autoRoutine;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("MergeSortUIController: balloon list null or too small.");
            return;
        }

        // capture original row positions
        originalPositions = new List<Vector2>(balloons.Count);
        foreach (var go in balloons)
            originalPositions.Add(go.GetComponent<RectTransform>().anchoredPosition);

        runSize    = 1;
        leftStart  = 0;
        isMerging  = false;

        if (autoRoutine != null)
            StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }

    private IEnumerator AutoSortLoop()
    {
        yield return new WaitForSeconds(restartDelay);

        while (runSize < balloons.Count)
        {
            StepAutomatic();
            yield return new WaitForSeconds(stepInterval);
        }

        // final sorted: highlight green
        SetAllFinal();

        yield return new WaitForSeconds(restartDelay);
        spawner?.ResetSpawnerUI();
    }

    private void StepAutomatic()
    {
        if (!isMerging)
        {
            if (leftStart >= balloons.Count - 1)
            {
                runSize *= 2;
                leftStart = 0;
                return;
            }

            int mid = Mathf.Min(leftStart + runSize - 1, balloons.Count - 1);
            int rightEnd = Mathf.Min(leftStart + 2 * runSize - 1, balloons.Count - 1);

            leftRun  = new List<GameObject>();
            rightRun = new List<GameObject>();
            for (int i = leftStart; i <= mid; i++)      leftRun.Add(balloons[i]);
            for (int j = mid + 1; j <= rightEnd; j++)  rightRun.Add(balloons[j]);

            // display runs above with extra left shift on the right run
            PositionRunsAbove(leftRun, true);
            PositionRunsAbove(rightRun, false);

            leftPtr    = 0;
            rightPtr   = 0;
            mergeIndex = leftStart;
            isMerging  = true;
            return;
        }

        if (leftPtr < leftRun.Count && rightPtr < rightRun.Count)
        {
            var next = (GetValue(leftRun[leftPtr]) <= GetValue(rightRun[rightPtr]))
                ? leftRun[leftPtr++] : rightRun[rightPtr++];
            StartCoroutine(MoveToMainRow(next, mergeIndex++));
        }
        else if (leftPtr < leftRun.Count)
        {
            StartCoroutine(MoveToMainRow(leftRun[leftPtr++], mergeIndex++));
        }
        else if (rightPtr < rightRun.Count)
        {
            StartCoroutine(MoveToMainRow(rightRun[rightPtr++], mergeIndex++));
        }
        else
        {
            leftStart += 2 * runSize;
            isMerging = false;
        }
    }

    private int GetValue(GameObject go) =>
        int.Parse(go.GetComponentInChildren<TextMeshProUGUI>().text);

    private void PositionRunsAbove(List<GameObject> run, bool isLeft)
    {
        float spacing = 50f;
        float baseAirShift = spacing * 0.5f;
        float extraShiftRight = run.Count * spacing * 0.1f; // extra left shift based on run length
        int count = run.Count;

        for (int i = 0; i < count; i++)
        {
            RectTransform rt = run[i].GetComponent<RectTransform>();
            Vector2 basePos = originalPositions[leftStart + (isLeft ? i : runSize + i)];
            float xShift = (isLeft ? -spacing : spacing)
                           - baseAirShift
                           - (isLeft ? 0f : extraShiftRight);
            Vector2 offset = new Vector2(xShift, spacing * 1.5f);
            rt.anchoredPosition = basePos + offset + new Vector2(i * spacing * 0.1f, 0);
        }
    }

    private IEnumerator MoveToMainRow(GameObject go, int targetIdx)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end   = originalPositions[targetIdx];
        const float DUR = 0.3f;
        float t = 0f;
        while (t < DUR)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / DUR);
            rt.anchoredPosition = Vector2.Lerp(start, end, u);
            yield return null;
        }

        // reorder in list
        balloons.Remove(go);
        balloons.Insert(targetIdx, go);
    }

    private void SetAllFinal()
    {
        // highlight entire array green
        foreach (var go in balloons)
        {
            var h = go.GetComponentInChildren<BalloonHighlightUI>();
            if (h != null)
                h.SetHighlighted(); // ensure this makes them green
        }
    }

    public void StopSorting()
    {
        if (autoRoutine != null)
            StopCoroutine(autoRoutine);
        StopAllCoroutines();
    }

    private void OnDisable() => StopSorting();
}
