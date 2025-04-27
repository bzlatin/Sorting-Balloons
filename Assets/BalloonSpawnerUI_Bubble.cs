using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BalloonSpawnerUI_Bubble : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform        rightPanel;      // drag your RightPanel here
    public GameObject           balloonPrefab;   // UI‑balloon prefab (Image + TMPUGUI)
    public BubbleSortUIController sortController;
   

    [Header("Spawner Settings")]
    public int   numberOfBalloons = 10;
    public float spacing           = 100f;       // pixels between balloons

    
    private readonly List<GameObject> spawned = new();   // keeps track of current row

    private void Start() => SpawnRow();

    public void ResetSpawnerUI()
    {
        foreach (var b in spawned) Destroy(b);
        spawned.Clear();
        SpawnRow();
    }

    private void SpawnRow()
    {
        /* 1. Build a shuffled pool of unique numbers 0‑20 */
        const int MIN_VALUE = 0;
        const int MAX_VALUE = 20;

        if (numberOfBalloons > MAX_VALUE - MIN_VALUE + 1)
        {
            Debug.LogWarning($"BalloonSpawnerUI: Clamping count to {MAX_VALUE - MIN_VALUE + 1} to keep numbers unique.");
            numberOfBalloons = MAX_VALUE - MIN_VALUE + 1;
        }

        List<int> pool = new();
        for (int i = MIN_VALUE; i <= MAX_VALUE; i++) pool.Add(i);

        // Fisher‑Yates shuffle
        for (int i = 0; i < pool.Count; i++)
        {
            int j = Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        /* 2. Centre the row horizontally in the panel */
        float offset = (numberOfBalloons - 1) * spacing / 2f;

        /* 3. Instantiate balloons */
        for (int i = 0; i < numberOfBalloons; i++)
        {
            GameObject go = Instantiate(balloonPrefab, rightPanel);
            RectTransform rt = go.GetComponent<RectTransform>();
            float rightMargin = 10f; // adjust as needed
            rt.anchoredPosition = new Vector2(i * spacing - offset - rightMargin, 0);


            TextMeshProUGUI label = go.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null) label.text = pool[i].ToString();
            else               Debug.LogError("UI balloon prefab missing TextMeshProUGUI!");

            spawned.Add(go);
        }

        /* 4. Hand the list to the UI‑side sort controller */
        if (sortController != null)
        {
            sortController.balloons = new List<GameObject>(spawned);
            sortController.Initialize();
        }
    }
}