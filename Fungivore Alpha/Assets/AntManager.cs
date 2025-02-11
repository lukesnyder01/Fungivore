using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    public List<Ant> ants = new List<Ant>();

    public int antCount = 10;

    private float timer;
    public float timeBetweenSteps = 0.01f;

    private int currentAntIndex = 0; // Tracks which ant to process next
    public int antsPerStep = 100; // Number of ants to process each time

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < antCount; i++)
        {
            Ant ant = new RibAnt();
            ant.RandomizeDirection();
            ant.antPos = new Vector3(
                Mathf.FloorToInt(Random.Range(-200, 200)),
                Mathf.FloorToInt(Random.Range(120, 150)),
                Mathf.FloorToInt(Random.Range(-200, 200))
            );
            ants.Add(ant);
        }

        timer = timeBetweenSteps;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = timeBetweenSteps;

            // Process a batch of ants
            for (int i = 0; i < antsPerStep; i++)
            {
                if (currentAntIndex >= ants.Count)
                {
                    currentAntIndex = 0; // Reset to the start of the list
                    break;
                }

                Ant ant = ants[currentAntIndex];

                ant.MoveNext();

                currentAntIndex++;
            }
        }
    }
}
