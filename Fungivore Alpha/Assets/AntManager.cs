using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    public List<Ant> ants = new List<Ant>();

    private int antCount = 10000;

    private float timer;
    private float timeBetweenSteps = 0.01f;

    private int currentAntIndex = 0; // Tracks which ant to process next
    private int antsPerFrame = 1000; // Number of ants to process per frame

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < antCount; i++)
        {
            Ant ant = new Ant();
            ant.RandomizeDirection();
            ant.antPos = new Vector3(
                Mathf.FloorToInt(Random.Range(-100, 100)),
                Mathf.FloorToInt(Random.Range(20, 160)),
                Mathf.FloorToInt(Random.Range(-100, 100))
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
            for (int i = 0; i < antsPerFrame; i++)
            {
                if (currentAntIndex >= ants.Count)
                {
                    currentAntIndex = 0; // Reset to the start of the list
                    break;
                }

                Ant ant = ants[currentAntIndex];

                Chunk chunk = ChunkContainingAnt(ant.antPos);
                // Check that the ant is in a valid chunk and the chunk is accepting block updates
                if (chunk != null && chunk.chunkState != Chunk.ChunkState.Processing)
                {
                    AddBlock(ant.antPos);
                    ant.RandomizeDirection();
                    ant.MoveForward();
                }

                currentAntIndex++;
            }
        }
    }

    private void AddBlock(Vector3 pos)
    {
        Chunk chunk = World.Instance.GetChunkAt(pos);

        if (chunk.GetBlock(pos) == Voxel.VoxelType.Air)
        {
            chunk.SetBlock(pos, Voxel.VoxelType.Stone);
        }
    }

    private Chunk ChunkContainingAnt(Vector3 antPos)
    {
        return World.Instance.GetChunkAt(antPos);
    }
}
