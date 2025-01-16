using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    private Ant ant;

    private float timer;
    private float timeBetweenSteps = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        timer = timeBetweenSteps;
        ant = new Ant();
        ant.antDir = new Vector3(0, 0, 1);
        ant.antPos = new Vector3(0, 160, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = timeBetweenSteps;

            // Check if ant is in a chunk that's loaded

            if (AntIsInChunk(ant.antPos))
            {
                AddBlock(ant.antPos);
                ant.MoveForward();
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

    

    private bool AntIsInChunk(Vector3 antPos)
    {
        Chunk chunk = World.Instance.GetChunkAt(antPos);
        return chunk != null;
    }

}
