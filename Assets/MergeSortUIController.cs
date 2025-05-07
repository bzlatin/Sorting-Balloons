using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MergeSortUIController : MonoBehaviour,ISortUIController
{
    [HideInInspector] public List<GameObject> balloons;

    [Header("References")]
    public BallonSpawnerUI_Merge  spawner;

    [Header("Timing")]
    public float stepInterval = 1.5f;   // pause between algorithm steps
    public float restartDelay = 1.0f;   // wait before respawn

    
    private int  runSize;        // current merge‐run width (1,2,4,8 …)
    private int  leftStart;      // first index of the segment we’re merging
    private int  mid;            // segment midpoint       (leftStart+runSize−1)
    private int  rightEnd;       
    private int  iPtr;          
    private int  jPtr;           // pointer in right half

    private bool isMerging;      
    private bool isShifting;     // true while ShiftRight coroutine runs

    private Coroutine autoRoutine;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("MergeSortUIController: balloon list null or too small.");
            return;
        }

        runSize    = 1;
        leftStart  = 0;
        isMerging  = false;
        isShifting = false;

        UpdateHighlight();

        if (autoRoutine != null) StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }

    
    private IEnumerator AutoSortLoop()
    {
        yield return new WaitForSeconds(restartDelay);

        while (runSize < balloons.Count)
        {
            yield return new WaitUntil(() => !isShifting);

            StepAutomatic();

            yield return new WaitForSeconds(stepInterval);
        }

        // Done – mark everything green & respawn
        ClearAllHighlights();
        SetAllFinal();

        yield return new WaitForSeconds(restartDelay);
        spawner?.ResetSpawnerUI();
    }

    /* ────────────────── one algorithm step ───────────────── */
    private void StepAutomatic()
    {
        /* 1️⃣  If not currently inside a merge segment, prepare one */
        if (!isMerging)
        {
            if (leftStart >= balloons.Count - 1)   // finished this pass
            {
                // Start next pass with double run width
                runSize   *= 2;
                leftStart  = 0;
                return;
            }

            // Define current segment boundaries
            mid      = Mathf.Min(leftStart + runSize - 1, balloons.Count - 1);
            rightEnd = Mathf.Min(leftStart + 2 * runSize - 1, balloons.Count - 1);

            iPtr     = leftStart;
            jPtr     = mid + 1;
            isMerging = true;

            UpdateHighlight();
            return;  // wait a beat before first compare
        }

        /* 2️⃣  Merge left & right halves by shifting elements in place */
        if (iPtr <= mid && jPtr <= rightEnd)
        {
            int leftVal  = GetValue(iPtr);
            int rightVal = GetValue(jPtr);

            if (leftVal <= rightVal)
            {
                iPtr++;                         // left item already in place
            }
            else
            {
                // Need to bring jPtr item forward to iPtr position
                StartCoroutine(ShiftRight(iPtr, jPtr));
                return;                         // rest handled after coroutine
            }
        }
        else
        {
            /* Segment finished → advance to next segment */
            leftStart += 2 * runSize;
            isMerging  = false;
        }

        UpdateHighlight();
    }

    /* ────────────────── helpers ─────────────────────────── */
    private int GetValue(int idx) =>
        int.Parse(balloons[idx].GetComponentInChildren<TextMeshProUGUI>().text);

    /// <summary>
    /// Moves the element at index <paramref name="from"/> forward to <paramref name="to"/>,
    /// shifting every element in between by one position to the right.
    /// Achieved via a sequence of pair-swaps so we can re-use SwapVisuals().
    /// </summary>
    private IEnumerator ShiftRight(int to, int from)
    {
        isShifting = true;
        for (int k = from; k > to; k--)
        {
            // highlight current swap partners
            SetHighlighted(k - 1, k);
            yield return SwapVisuals(k - 1, k);
        }

        // update pointers after the shift
        iPtr++;
        mid++;
        jPtr++;

        isShifting = false;
        UpdateHighlight();
    }

    private IEnumerator SwapVisuals(int i, int j)
    {
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

        // commit the swap in the list
        aRT.anchoredPosition = startB;
        bRT.anchoredPosition = startA;
        (balloons[i], balloons[j]) = (balloons[j], balloons[i]);
    }

    /* ────────────────── highlighting & finish helpers ───── */
    private void UpdateHighlight()
    {
        if (runSize >= balloons.Count) { ClearAllHighlights(); return; }

        int a = Mathf.Clamp(iPtr, 0, balloons.Count - 1);
        int b = Mathf.Clamp(jPtr, 0, balloons.Count - 1);
        SetHighlighted(a, b);
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
    private void OnEnable()  => Initialize();
}
