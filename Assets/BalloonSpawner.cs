using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum SortType
{
    Bubble,
    Insertion,
    Selection,
    Merge
}

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class BalloonSpawner : MonoBehaviour
{
    [Header("Prefab & Parent")]
    public GameObject balloonPrefab;
    public Canvas balloonsCanvas;

    [Header("Spawn Settings")]
    public int numberOfBalloons = 10;
    public float yPosition = 0f;
    [Range(0.5f, 1f)]
    public float paddingRatio = 0.9f; // % of screen width to use
    [Range(0.1f, 2f)]
    public float scaleMultiplier = .75f; // tweak to enlarge/shrink balloons

    [Header("Sorting Logic")]
    public BubbleSortController bubbleSortController;
    public InsertionSortController insertionSortController;
    public SelectionSortController selectionSortController;
    public MergeSortController mergeSortController;

    public SortType sortType;
    public GameManager gameManager;

    private readonly List<GameObject> spawned = new();

    void Start()
    {
        // Determine balloon count based on saved difficulty
        Difficulty difficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty", 0); // Default to Easy
        numberOfBalloons = difficulty switch
        {
            Difficulty.Easy => 10,
            Difficulty.Medium => 16,
            Difficulty.Hard => 20,
            _ => 10
        };

        // Pick sort type
        string sortTypeStr = PlayerPrefs.GetString("SortType", "Bubble");
        if      (sortTypeStr == "Insertion") sortType = SortType.Insertion;
        else if (sortTypeStr == "Selection") sortType = SortType.Selection;
        else if (sortTypeStr == "Merge")     sortType = SortType.Merge;
        else                                  sortType = SortType.Bubble;

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
        // Clamp to unique range
        const int MIN_VALUE = 0, MAX_VALUE = 20;
        if (numberOfBalloons > (MAX_VALUE - MIN_VALUE + 1))
        {
            Debug.LogWarning($"Clamping numberOfBalloons from {numberOfBalloons} to max {(MAX_VALUE - MIN_VALUE + 1)}");
            numberOfBalloons = MAX_VALUE - MIN_VALUE + 1;
        }

        // Build & shuffle pool
        List<int> pool = new List<int>();
        for (int i = MIN_VALUE; i <= MAX_VALUE; i++) pool.Add(i);
        for (int i = 0; i < pool.Count; i++)
        {
            int j = Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        // Camera dimensions
        Camera cam = Camera.main;
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        // Usable width + margin
        float usableWidth = worldWidth * paddingRatio;
        float margin = (worldWidth - usableWidth) * 0.5f;

        // Dynamic spacing
        float spacing = usableWidth / numberOfBalloons;

        // Compute startX
        float startX = cam.transform.position.x - worldWidth / 2f + margin + spacing / 2f;

        // Instantiate & scale each balloon
        for (int i = 0; i < numberOfBalloons; i++)
        {
            Vector3 pos = new Vector3(startX + i * spacing, yPosition, 0f);
            Transform parent = balloonsCanvas != null ? balloonsCanvas.transform : transform;
            GameObject balloon = Instantiate(balloonPrefab, pos, Quaternion.identity, parent);

            // Scale based on spacing + multiplier
            var sr = balloon.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                float originalWidth = sr.bounds.size.x;
                float scaleFactor = (spacing / originalWidth) * scaleMultiplier;
                balloon.transform.localScale = Vector3.one * scaleFactor;
            }

            // Set the number text
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
        yield return null;
        gameManager.InitializeActiveSort(new List<GameObject>(spawned));
    }
}
