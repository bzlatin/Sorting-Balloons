using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleSortController : MonoBehaviour
{
    public List<GameObject> balloons;
    public TextMeshProUGUI statusText;

    private int index = 0;
    private int end;
    private bool swapped = false;
    private bool isSwapping = false;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("❌ Balloon list not assigned or too short.");
            return;
        }

        index = 0;
        swapped = false;
        end = balloons.Count;

        UpdateHighlight();
    }

    void Update()
    {
        if (balloons == null || index + 1 >= end || isSwapping) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) HandleAction(true);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) HandleAction(false);
    }

    void HandleAction(bool attemptSwap)
    {
        if (index + 1 >= balloons.Count)
        {
            Debug.LogWarning("Index out of range — cannot compare beyond balloon list.");
            return;
        }

        var leftTMP = balloons[index].GetComponentInChildren<TextMeshPro>();
        var rightTMP = balloons[index + 1].GetComponentInChildren<TextMeshPro>();

        if (leftTMP == null || rightTMP == null)
        {
            Debug.LogError("❌ One or both balloons are missing a TextMeshPro component.");
            return;
        }

        int a = int.Parse(leftTMP.text);
        int b = int.Parse(rightTMP.text);

        if (a > b)
        {
            if (!attemptSwap)
            {
                statusText.text = "❌ You should have swapped!";
                return;
            }

            Swap(index, index + 1);
            swapped = true;
            statusText.text = $"✅ Swapped {a} and {b}";
        }
        else
        {
            if (attemptSwap)
            {
                statusText.text = "❌ You shouldn't have swapped!";
                return;
            }

            statusText.text = $"✔️ Correctly skipped {a} and {b}";
        }

        index++;

        if (index + 1 >= end)
        {
            index = 0;
            end--;
            swapped = false;
        }

        UpdateHighlight();
    }

    void Swap(int i, int j)
    {
        if (!isSwapping) StartCoroutine(SwapVisuals(i, j));
    }

    IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        GameObject balloonA = balloons[i];
        GameObject balloonB = balloons[j];

        Vector3 posA = balloonA.transform.position;
        Vector3 posB = balloonB.transform.position;

        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            balloonA.transform.position = Vector3.Lerp(posA, posB, t);
            balloonB.transform.position = Vector3.Lerp(posB, posA, t);

            yield return null;
        }

        balloonA.transform.position = posB;
        balloonB.transform.position = posA;

        (balloons[i], balloons[j]) = (balloons[j], balloons[i]);

        UpdateHighlight();
        yield return new WaitForSeconds(0.05f);

        if (IsSorted())
        {
            statusText.text = "🎉 Sorted!";
            DimAllBalloons();
        }

        isSwapping = false;
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < balloons.Count; i++)
        {
            var balloon = balloons[i];
            if (balloon == null) continue;

            var highlight = balloon.GetComponent<BalloonHighlight>();
            if (highlight == null) continue;

            if (i == index || i == index + 1)
                highlight.SetHighlighted();
            else
                highlight.SetNormal();
        }
    }

    void DimAllBalloons()
    {
        foreach (var b in balloons)
        {
            var highlight = b.GetComponent<BalloonHighlight>();
            highlight?.SetDimmed();
        }
    }

    bool IsSorted()
    {
        for (int i = 0; i < balloons.Count - 1; i++)
        {
            var tmp1 = balloons[i].GetComponentInChildren<TextMeshPro>();
            var tmp2 = balloons[i + 1].GetComponentInChildren<TextMeshPro>();

            if (tmp1 == null || tmp2 == null) continue;

            int val1 = int.Parse(tmp1.text);
            int val2 = int.Parse(tmp2.text);

            if (val1 > val2)
                return false;
        }

        return true;
    }
}
