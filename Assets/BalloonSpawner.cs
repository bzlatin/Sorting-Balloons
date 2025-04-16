using UnityEngine; 
using TMPro;
using System.Collections.Generic;

public class BalloonSpawner : MonoBehaviour
{
    public GameObject balloonPrefab;         // Balloon prefab (must have TextMeshPro as child)
    public int numberOfBalloons = 10;          // Number of balloons
    public float spacing = 1.5f;               // Distance between them
    public float yPosition = 0f;               // Height where they appear
    public BubbleSortController sortController; // Bubble sort logic controller
    public Canvas balloonsCanvas;            // Reference to the BalloonsCanvas

    void Start()
    {
        float offset = (numberOfBalloons - 1) * spacing / 2f; // Center the row

        // Create a list to store random numbers for each balloon.
        List<int> numbers = new List<int>();

        // Generate a random number for each balloon from 0 to 20.
        for (int i = 0; i < numberOfBalloons; i++)
        {
            numbers.Add(Random.Range(0, 21)); // Random.Range with upper bound exclusive when using int, so 21 makes it 0-20.
        }

        List<GameObject> spawnedBalloons = new List<GameObject>();

        // Instantiate balloons and assign the random numbers.
        for (int i = 0; i < numberOfBalloons; i++)
        {
            Vector3 spawnPos = new Vector3(i * spacing - offset, yPosition, 0f);
            GameObject balloon = Instantiate(balloonPrefab, spawnPos, Quaternion.identity);

            // Set the balloon as a child of the BalloonsCanvas.
            balloon.transform.SetParent(balloonsCanvas.transform);

            // Set the random number on the balloon using TextMeshPro (world space).
            TextMeshPro numberText = balloon.GetComponentInChildren<TextMeshPro>();
            if (numberText != null)
                numberText.text = numbers[i].ToString();

            spawnedBalloons.Add(balloon);
        }

        // Pass the spawned balloons to the sort controller.
        if (sortController != null)
        {
            sortController.balloons = spawnedBalloons;
            sortController.Initialize();
        }
    }

}
