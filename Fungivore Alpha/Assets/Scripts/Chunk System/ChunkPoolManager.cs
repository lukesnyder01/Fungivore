using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    public static ChunkPoolManager Instance { get; private set; }

    private Queue<GameObject> chunkPool = new Queue<GameObject>();

    private int initialPoolSize = 4000; // Number of chunks to add to the pool at start

    void Awake()
    {
        Instance = this;
    }

    public void PopulateInitialPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject newChunk = InstantiateNewChunk();
            chunkPool.Enqueue(newChunk);
        }
    }

        // Instantiate chunk objects for the pool, and give them appropriate components
    private GameObject InstantiateNewChunk()
    {
        GameObject newChunk = new GameObject("Chunk");

        newChunk.AddComponent<MeshFilter>();
        newChunk.AddComponent<MeshRenderer>();
        newChunk.AddComponent<MeshCollider>();

        newChunk.layer = LayerMask.NameToLayer("Solid Block");
        newChunk.tag = "Solid Block";

        return newChunk;
    }


    public GameObject GetChunk()
    {
        GameObject chunk;

        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
        }
        else
        {
            chunk = InstantiateNewChunk();
        }
        return chunk;
    }

    public void ReturnChunk(GameObject chunk)
    {
        //chunk.ResetChunk();
        chunk.gameObject.SetActive(false);
        chunkPool.Enqueue(chunk);
    }




}