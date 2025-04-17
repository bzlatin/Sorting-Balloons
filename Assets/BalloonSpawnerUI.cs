using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BalloonSpawnerUI : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform rightPanel;       // Drag in your RightPanel here
    public GameObject balloonPrefab;       // Drag in your UI_Balloon prefab
    public BubbleSortController sortController; 

    [Header("Spawner Settings")]
    public int numberOfBalloons = 10;
    public float spacing = 100f;           // pixels between balloons

    void Start()
    {
        // center row
        float offset = (numberOfBalloons - 1) * spacing / 2f;
        var spawned = new List<GameObject>();

        for (int i = 0; i < numberOfBalloons; i++)
        {
            // 1) Instantiate under RightPanel
            var go = Instantiate(balloonPrefab, rightPanel);
            
            // 2) Position via RectTransform
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * spacing - offset, 0);

            // 3) Assign random 0–20
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            label.text = Random.Range(0, 21).ToString();

            spawned.Add(go);
        }

        // 4) Send to your sort controller
        sortController.balloons = spawned;
        sortController.Initialize();
    }
}
