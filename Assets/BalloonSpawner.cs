using UnityEngine;
using TMPro;                                 // base class for all TextMeshPro types
using System.Collections.Generic;

public class BalloonSpawner : MonoBehaviour
{
    [Header("Prefab & Parent")]
    public GameObject balloonPrefab;          // must contain a TMP text child
    public Canvas balloonsCanvas;             // parent canvas (optional for UI)

    [Header("Spawn Settings")]
    public int numberOfBalloons = 10;         
    public float spacing = 1.5f;              // distance between balloons (world units)
    public float yPosition = 0f;              // row height

    [Header("Sorting Logic")]
    public BubbleSortController sortController;

    
    private readonly List<GameObject> spawned = new();   // track active balloons

   
    private void Start() => SpawnBalloons();              // first row

   
    public void ResetSpawner()
    {
        
        foreach (var b in spawned) Destroy(b);
        spawned.Clear();

        
        SpawnBalloons();
    }

 
    private void SpawnBalloons()
    {
        
        const int MIN_VALUE = 0;
        const int MAX_VALUE = 20;             // inclusive

        if (numberOfBalloons > (MAX_VALUE - MIN_VALUE + 1))
        {
            Debug.LogWarning($"Number of balloons ({numberOfBalloons}) exceeds unique range size; clamping.");
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

       
        float offset = (numberOfBalloons - 1) * spacing * 0.5f;

       
        for (int i = 0; i < numberOfBalloons; i++)
        {
            Vector3 pos = new(i * spacing - offset, yPosition, 0f);

            // parent to canvas if supplied; otherwise to spawner GameObject
            Transform parent = balloonsCanvas != null ? balloonsCanvas.transform : transform;
            GameObject balloon = Instantiate(balloonPrefab, pos, Quaternion.identity, parent);

            // set number text
            TMP_Text tmp = balloon.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
                tmp.text = pool[i].ToString();
            else
                Debug.LogError("Balloon prefab has no TMP_Text component!");

            spawned.Add(balloon);
        }

       
        if (sortController != null)
        {
            sortController.balloons = new List<GameObject>(spawned);  // copy so controller can reorder
            sortController.Initialize();
        }
    }
}
