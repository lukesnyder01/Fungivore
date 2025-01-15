using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    private Ant ant;

    private float timer;
    private float timeBetweenSteps = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        timer = timeBetweenSteps;
        ant = new Ant();
        ant.antPos = new Vector3(0, 100, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = timeBetweenSteps;

            ant.RandomizeDirection();
            ant.MoveForward();
            AddBlock(ant.antPos);
        }
    }

    private void AddBlock(Vector3 pos)
    {
        Chunk chunk = World.Instance.GetChunkAt(pos);
        if (chunk != null)
        {
            chunk.SetBlock(pos, Voxel.VoxelType.Grass);
        }

    }
}
