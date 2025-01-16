using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{

    public List<Ant> ants = new List<Ant>();

    private int antCount = 1000;

    private float timer;
    private float timeBetweenSteps = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < antCount; i++)
        {
            Ant ant = new Ant();
            ant.RandomizeDirection();
            ant.antPos = new Vector3(
                Mathf.FloorToInt(Random.Range(-20, 20)),
                Mathf.FloorToInt(Random.Range(80, 120)),
                Mathf.FloorToInt(Random.Range(-20, 20))
            );
            ants.Add(ant);
            Debug.Log("Added an ant");
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

            foreach (Ant ant in ants)
            {
                Debug.Log("Move ant" + ant.antPos);

                Chunk chunk = ChunkContainingAnt(ant.antPos);
                // Check that the ant is in a valid chunk and the chunk is accepting block updates
                if (chunk != null && chunk.chunkState != Chunk.ChunkState.Processing)
                {
                    AddBlock(ant.antPos);
                    ant.MoveForward();
                }
            }
        }
    }

    private void AddBlock(Vector3 pos)
    {
        Chunk chunk = World.Instance.GetChunkAt(pos);

        if (chunk.GetBlock(pos) == Voxel.VoxelType.Air)
        {
            Debug.Log("Adding block at " + pos);
            chunk.SetBlock(pos, Voxel.VoxelType.Stone);
        }
    }


    private Chunk ChunkContainingAnt(Vector3 antPos)
    {
        return World.Instance.GetChunkAt(antPos);
    }

}
