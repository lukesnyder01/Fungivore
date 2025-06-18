using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    public static AntManager Instance { get; private set; }

    public List<Ant> ants = new List<Ant>();

    public int startingAntCount = 10;

    private float timer;
    public float timeBetweenSteps = 0.05f;

    private int currentAntIndex = 0; // Tracks which ant to process next
    public int antsPerStep = 100; // Number of ants to process each time

    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < startingAntCount; i++)
        {
            Ant ant = new RibAnt();
            ant.RandomizeDirection();
            ant.antPos = new Vector3(
                Mathf.FloorToInt(Random.Range(-300, 300)),
                Mathf.FloorToInt(Random.Range(1, 150)),
                Mathf.FloorToInt(Random.Range(-300, 300))
            );
            ants.Add(ant);
        }

        timer = timeBetweenSteps;
    }


    public void AddRibAnt(Vector3 position)
    {
        Ant ant = new RibAnt();
        ant.RandomizeDirection();
        ant.antPos = position;
        ants.Add(ant);
    }

    public void AddDropperAnt(Vector3 position)
    {
        Ant ant = new DropperAnt();
        ant.RandomizeDirection();
        ant.antPos = position;
        ants.Add(ant);
    }

    public void AddClimberAnt(Vector3 position)
    {
        Ant ant = new ClimberAnt();
        ant.RandomizeDirection();
        ant.antPos = position;
        ants.Add(ant);
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

                // Check to see if the ant has any remaining moves
                // and remove from the ant list if not
                if (ant.movesRemaining <= 0)
                {
                    ants.RemoveAt(currentAntIndex);
                    Debug.Log(ants.Count + " ants in AntManager");
                }
                else
                {
                    ant.MoveNext();
                }

                currentAntIndex++;
            }
        }
    }
}
