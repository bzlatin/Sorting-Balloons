using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BalloonSpawner : MonoBehaviour
{
    public GameObject balloonPrefab;     // Your balloon prefab with TMP text
    public Transform spawnParent;        // Optional: empty GameObject to group balloons
    public int numberOfBalloons = 10;    // Should stay 10 for 0-9
    public float spacing = 1.5f;     // Distance between balloons
    public float yPosition = 0f;     // Height where balloons appear


    void Start()
    {
        float offset = (numberOfBalloons - 1) * spacing / 2f; // Center the line

        // Step 1: Create a list of numbers from 0 to 9
        List<int> numbers = new List<int>();
        for (int i = 0; i < 10; i++) numbers.Add(i);

        // Step 2: Shuffle the list to randomize order
        Shuffle(numbers);

        // Step 3: Instantiate balloons and assign numbers
        for (int i = 0; i < numberOfBalloons; i++)
        {
            Vector3 spawnPos = new Vector3(i * spacing - offset, yPosition, 0f);
            GameObject balloon = Instantiate(balloonPrefab, spawnPos, Quaternion.identity, spawnParent);
            
            TextMeshProUGUI tmp = balloon.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = numbers[i].ToString();
        }

    }

    // Fisher–Yates Shuffle to randomize the list
    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
