using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// UI‑side, fully automatic bubble‑sort visualiser for the right panel.
/// Spawn balloons with BalloonSpawnerUI, then call <see cref="Initialize()"/>.
/// </summary>
public class BubbleSortUIController : MonoBehaviour
{
    [HideInInspector] public List<GameObject> balloons;      // set by BalloonSpawnerUI

    [Header("Animation")]
    public float stepInterval = 0.6f;                        // seconds between steps

    private int  index;
    private int  end;
    private bool isSwapping;
    private Coroutine autoRoutine;

    /* ───────────────────────────────────────── PUBLIC ───────────────────────────────────────── */

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("BubbleSortUIController: balloon list null or too small.");
            return;
        }

        index = 0;
        end   = balloons.Count;
        isSwapping = false;

        UpdateHighlight();

        if (autoRoutine != null) StopCoroutine(autoRoutine);
        autoRoutine = StartCoroutine(AutoSortLoop());
    }

    /* ───────────────────────────────────────── AUTO LOOP ────────────────────────────────────── */

    private IEnumerator AutoSortLoop()
    {
        yield return null;                                    // let first frame render

        while (end > 1)
        {
            yield return new WaitUntil(() => !isSwapping);
            StepAutomatic();

            if (end <= 1) break;
            yield return new WaitForSeconds(stepInterval);
        }

        HighlightAsWinners();
    }

    /* ───────────────────────────────────────── SINGLE STEP ─────────────────────────────────── */

    private void StepAutomatic()
    {
        if (isSwapping || index + 1 >= end) return;

        int a = GetValue(index);
        int b = GetValue(index + 1);

        if (a > b) Swap(index, index + 1);
        else       AdvanceIndices();
    }

    /* ───────────────────────────────────────── CORE UTILS ──────────────────────────────────── */

    private int GetValue(int i) =>
        int.Parse(balloons[i].GetComponentInChildren<TextMeshProUGUI>().text);

    private void Swap(int i, int j)
    {
        if (!isSwapping) StartCoroutine(SwapVisuals(i, j));
    }

    private IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        /* NEW — keep the two balloons highlighted while they move */
        balloons[i].GetComponent<BalloonHighlight>()?.SetHighlighted();
        balloons[j].GetComponent<BalloonHighlight>()?.SetHighlighted();

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

        /* NEW — now move on to the next pair and refresh highlights */
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
