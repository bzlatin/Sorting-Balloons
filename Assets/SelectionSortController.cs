using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionSortController : MonoBehaviour
{
    public List<GameObject> balloons;
    public GameManager gameManager;

    [Header("Marker")]
    public GameObject markerPrefab;
    private GameObject markerInstance;

    private int currentIndex = 0;
    private int minIndex = 0;
    private int compareIndex = 0;
    private bool isSwapping = false;
    private bool isActive = true;

    public void Initialize()
    {
        if (balloons == null || balloons.Count < 2)
        {
            Debug.LogError("SelectionSortController: balloon list not assigned or too small.");
            isActive = false;
            return;
        }

        currentIndex = 0;
        minIndex = currentIndex;
        compareIndex = currentIndex + 1;
        isActive = true;

        if (markerInstance == null && markerPrefab != null)
        {
            markerInstance = Instantiate(markerPrefab, transform);
        }

        UpdateHighlight();
        UpdateMarker();
    }

    void Update()
    {
        if (!isActive || isSwapping || currentIndex >= balloons.Count - 1)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HandleAction(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandleAction(false);
        }
    }

    void HandleAction(bool attemptMarkAsMin)
    {
        if (compareIndex >= balloons.Count)
        {
            // End of scan, do one swap
            if (minIndex != currentIndex)
            {
                Swap(currentIndex, minIndex);
            }
            else
            {
                ProceedToNext();
            }
            return;
        }

        int currentVal = GetValue(compareIndex);
        int minVal = GetValue(minIndex);

        if (currentVal < minVal)
        {
            if (!attemptMarkAsMin)
            {
                gameManager.OnPlayerMistake("You missed a smaller value!");
                return;
            }

            minIndex = compareIndex;
            gameManager.UpdateStatus($"Marked {currentVal} as new minimum");
        }
        else
        {
            if (attemptMarkAsMin)
            {
                gameManager.OnPlayerMistake("That's not the smallest value.");
                return;
            }

            gameManager.UpdateStatus($"Skipped {currentVal}, still min is {minVal}");
        }

        compareIndex++;
        UpdateHighlight();
        UpdateMarker();
    }

    void ProceedToNext()
    {
        currentIndex++;
        minIndex = currentIndex;
        compareIndex = currentIndex + 1;

        if (currentIndex >= balloons.Count - 1)
        {
            HighlightAsWinners();
            isActive = false;
            gameManager.OnLevelComplete();
        }

        UpdateHighlight();
        UpdateMarker();
    }

    void Swap(int i, int j)
    {
        if (!isSwapping)
            StartCoroutine(SwapVisuals(i, j));
    }

    IEnumerator SwapVisuals(int i, int j)
    {
        isSwapping = true;

        GameObject a = balloons[i];
        GameObject b = balloons[j];

        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            a.transform.position = Vector3.Lerp(posA, posB, t);
            b.transform.position = Vector3.Lerp(posB, posA, t);
            yield return null;
        }

        a.transform.position = posB;
        b.transform.position = posA;

        (balloons[i], balloons[j]) = (balloons[j], balloons[i]);

        ProceedToNext();
        UpdateMarker(); 

        isSwapping = false;
    }

    int GetValue(int i)
    {
        var tmp = balloons[i].GetComponentInChildren<TextMeshPro>();
        return tmp != null ? int.Parse(tmp.text) : 0;
    }

    void HighlightAsWinners()
    {
        foreach (var b in balloons)
            b.GetComponent<BalloonHighlight>()?.SetWinner();
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < balloons.Count; i++)
        {
            var highlight = balloons[i].GetComponent<BalloonHighlight>();
            if (highlight == null) continue;

            if (i == minIndex || i == compareIndex)
                highlight.SetHighlighted();
            else
                highlight.SetNormal();
        }
    }

    void UpdateMarker()
    {
        if (markerInstance != null && minIndex >= 0 && minIndex < balloons.Count)
        {
            Vector3 pos = balloons[minIndex].transform.position;
            pos.y += 1.2f;
            markerInstance.transform.position = pos;
        }
    }

    public void DisableInput()
    {
        isActive = false;
        if (markerInstance != null)
            Destroy(markerInstance);
    }

    public void ResetSorting() => Initialize();
}
