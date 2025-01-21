using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    public static ChunkPoolManager Instance { get; private set; }

    private Queue<Chunk> chunkPool = new Queue<Chunk>();
    public int initialPoolSize = 10; // Number of chunks to add to the pool at start

    void Awake()
    {
        Instance = this;
    }

    public void PopulateInitialPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Chunk newChunk = InstantiateNewChunk();
            chunkPool.Enqueue(newChunk);
        }
    }

    public Chunk GetChunk()
    {
        Chunk chunk;
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

    public void ReturnChunk(Chunk chunk)
    {
        chunk.ResetChunk();
        chunk.gameObject.SetActive(false);
        chunkPool.Enqueue(chunk);
    }

    // Instantiate chunk objects for the pool, and give them appropriate components
    private Chunk InstantiateNewChunk()
    {
        GameObject newChunkObject = new GameObject("Chunk");
        Chunk newChunk = newChunkObject.AddComponent<Chunk>();

        newChunkObject.AddComponent<MeshFilter>();
        newChunkObject.AddComponent<MeshRenderer>();
        newChunkObject.AddComponent<MeshCollider>();

        newChunkObject.layer = LayerMask.NameToLayer("Solid Block");
        newChunkObject.tag = "Solid Block";

        return newChunk;
    }


}