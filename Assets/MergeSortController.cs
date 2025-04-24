using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MergeSortController : MonoBehaviour
{
    public List<GameObject> balloons;
    public GameManager gameManager;

    private bool isActive = false;

    private int groupSize;
    private int mergeGroupStart;
    private int leftIndex, rightIndex;
    private List<GameObject> mergeBuffer;
    private Vector3[] targetPositions;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("MergeSortController: need at least 2 balloons.");
            return;
        }

        isActive       = false;               // lock input until first message
        mergeBuffer    = new List<GameObject>();
        targetPositions = new Vector3[balloons.Count];
        for (int i = 0; i < balloons.Count; i++)
            targetPositions[i] = balloons[i].transform.position;

        groupSize       = 2;
        mergeGroupStart = 0;

        StartCoroutine(DelayedFirstHighlight());
    }

    private IEnumerator DelayedFirstHighlight()
    {
        yield return null;
        gameManager.UpdateStatus("Press ← or → to merge the highlighted balloons.");
        BeginNextMergeGroup();
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))  HandleUserPick(true);
        if (Input.GetKeyDown(KeyCode.RightArrow)) HandleUserPick(false);
    }

    private void HandleUserPick(bool pickLeft)
    {
        int mid = mergeGroupStart + groupSize/2;
        int end = Mathf.Min(mergeGroupStart + groupSize, balloons.Count);

        GameObject left  = leftIndex  < mid ? balloons[leftIndex]  : null;
        GameObject right = rightIndex < end ? balloons[rightIndex] : null;

        // auto-pick when one side is gone
        if (left == null && right != null)      pickLeft = false;
        else if (right == null && left != null) pickLeft = true;

        // validate correct choice
        if (left != null && right != null)
        {
            bool shouldPickLeft = GetValue(left) <= GetValue(right);
            if (pickLeft != shouldPickLeft)
            {
                gameManager.OnPlayerMistake("Oops—that wasn't the smaller balloon.");
                return;
            }
        }

        // record pick
        GameObject chosen = pickLeft ? left : right;
        mergeBuffer.Add(chosen);
        if (pickLeft) leftIndex++; else rightIndex++;

        int needed = Mathf.Min(groupSize, balloons.Count - mergeGroupStart);
        if (mergeBuffer.Count < needed)
        {
            UpdateHighlight();
        }
        else
        {
            StartCoroutine(AnimateGroupMerge());
        }
    }

    private IEnumerator AnimateGroupMerge()
    {
        isActive = false;
        gameManager.UpdateStatus("Merging…");

        // animate all picks into place
        for (int i = 0; i < mergeBuffer.Count; i++)
        {
            var b     = mergeBuffer[i];
            var start = b.transform.position;
            var end   = targetPositions[mergeGroupStart + i];
            float t   = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.25f;
                b.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }

        // write back
        for (int i = 0; i < mergeBuffer.Count; i++)
            balloons[mergeGroupStart + i] = mergeBuffer[i];

        gameManager.UpdateStatus("Group merged!");

        // advance
        mergeGroupStart += groupSize;
        if (mergeGroupStart >= balloons.Count)
        {
            if (groupSize >= balloons.Count)
            {
                Finish();
                yield break;
            }
            groupSize *= 2;
            mergeGroupStart = 0;
        }

        BeginNextMergeGroup();
        isActive = true;
    }

    private void BeginNextMergeGroup()
    {
        mergeBuffer.Clear();
        leftIndex  = mergeGroupStart;
        rightIndex = mergeGroupStart + groupSize/2;

        gameManager.UpdateStatus("Merging next group of balloons...");
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        int groupEnd = Mathf.Min(mergeGroupStart + groupSize, balloons.Count);

        for (int i = 0; i < balloons.Count; i++)
        {
            var hl = balloons[i].GetComponent<BalloonHighlight>();
            if (hl == null) continue;

            // dim everything outside current group
            if (i < mergeGroupStart || i >= groupEnd)
                hl.SetDimmed();
            else
            {
                // normal for in-group
                hl.SetNormal();
                // highlight the two under comparison
                if (i == leftIndex || i == rightIndex)
                    hl.SetHighlighted();
            }
        }
    }

    private int GetValue(GameObject b)
    {
        var tmp = b.GetComponentInChildren<TextMeshPro>();
        return tmp != null ? int.Parse(tmp.text) : 0;
    }

    private void Finish()
    {
        foreach (var b in balloons)
            b.GetComponent<BalloonHighlight>()?.SetWinner();

        gameManager.UpdateStatus($"All balloons sorted!");
        gameManager.OnLevelComplete();
    }

    public void DisableInput() => isActive = false;
    public void ResetSorting() => Initialize();
}
