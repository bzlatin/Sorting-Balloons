using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum SortType
{
    Bubble,
    Insertion
}

public class BalloonSpawner : MonoBehaviour
{
    [Header("Prefab & Parent")]
    public GameObject balloonPrefab;
    public Canvas balloonsCanvas;

    [Header("Spawn Settings")]
    public int numberOfBalloons = 10;
    public float spacing = 1.5f;
    public float yPosition = 0f;

    [Header("Sorting Logic")]
    public BubbleSortController bubbleSortController;
    public InsertionSortController insertionSortController;
    public SortType sortType;
    public GameManager gameManager;

    private readonly List<GameObject> spawned = new();

    void Start()
    {
        string sortTypeStr = PlayerPrefs.GetString("SortType", "Bubble");
        sortType = sortTypeStr == "Insertion" ? SortType.Insertion : SortType.Bubble;

        SpawnBalloons();
        StartCoroutine(InitializeSortAfterFrame());
    }

    public void ResetSpawner()
    {
        foreach (var b in spawned) Destroy(b);
        spawned.Clear();

        SpawnBalloons();
        StartCoroutine(InitializeSortAfterFrame());
    }

    private void SpawnBalloons()
    {
        const int MIN_VALUE = 0;
        const int MAX_VALUE = 20;

        if (numberOfBalloons > (MAX_VALUE - MIN_VALUE + 1))
        {
            Debug.LogWarning($"Number of balloons ({numberOfBalloons}) exceeds unique range size; clamping.");
            numberOfBalloons = MAX_VALUE - MIN_VALUE + 1;
        }

        List<int> pool = new();
        for (int i = MIN_VALUE; i <= MAX_VALUE; i++) pool.Add(i);

        for (int i = 0; i < pool.Count; i++)
        {
            int j = Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        float offset = (numberOfBalloons - 1) * spacing * 0.5f;

        for (int i = 0; i < numberOfBalloons; i++)
        {
            Vector3 pos = new(i * spacing - offset, yPosition, 0f);
            Transform parent = balloonsCanvas != null ? balloonsCanvas.transform : transform;
            GameObject balloon = Instantiate(balloonPrefab, pos, Quaternion.identity, parent);

            TMP_Text tmp = balloon.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
                tmp.text = pool[i].ToString();
            else
                Debug.LogError("Balloon prefab has no TMP_Text component!");

            spawned.Add(balloon);
        }
    }

    private IEnumerator InitializeSortAfterFrame()
    {
        yield return null; // Wait one frame so GameManager has activated the correct controller
        gameManager.InitializeActiveSort(new List<GameObject>(spawned));
    }
}
