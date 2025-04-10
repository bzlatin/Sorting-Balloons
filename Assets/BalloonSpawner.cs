using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BalloonSpawner : MonoBehaviour
{
    public GameObject balloonPrefab;         // Balloon prefab (must have TextMeshPro as child)
    public int numberOfBalloons = 10;        // Number of balloons
    public float spacing = 1.5f;             // Distance between them
    public float yPosition = 0f;             // Height where they appear
    public BubbleSortController sortController; // Bubble sort logic controller

    void Start()
    {
        float offset = (numberOfBalloons - 1) * spacing / 2f; // Center the row
        List<int> numbers = new List<int>();

        for (int i = 0; i < numberOfBalloons; i++) numbers.Add(i);
        Shuffle(numbers);

        List<GameObject> spawnedBalloons = new List<GameObject>();

        for (int i = 0; i < numberOfBalloons; i++)
        {
            Vector3 spawnPos = new Vector3(i * spacing - offset, yPosition, 0f);
            GameObject balloon = Instantiate(balloonPrefab, spawnPos, Quaternion.identity);

            // ✅ Use TextMeshPro (world space)
            TextMeshPro numberText = balloon.GetComponentInChildren<TextMeshPro>();
            if (numberText != null)
                numberText.text = numbers[i].ToString();

            spawnedBalloons.Add(balloon);
        }

        // Send to controller
        if (sortController != null)
        {
            sortController.balloons = spawnedBalloons;
            sortController.ResetSorting();
        }
    }

    // Fisher–Yates Shuffle
    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
