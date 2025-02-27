using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    public static ChunkPoolManager Instance { get; private set; }

    private Queue<ChunkData> chunkPool = new Queue<ChunkData>();
    public int initialPoolSize = 10; // Number of chunks to add to the pool at start

    void Awake()
    {
        Instance = this;
    }

    public void PopulateInitialPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            ChunkData newChunk = InstantiateNewChunk();
            chunkPool.Enqueue(newChunk);
        }
    }

    public ChunkData GetChunk()
    {
        ChunkData chunk;
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

    public void ReturnChunk(ChunkData chunk)
    {
        chunk.ResetChunk();
        chunk.gameObject.SetActive(false);
        chunkPool.Enqueue(chunk);
    }

    // Instantiate chunk objects for the pool, and give them appropriate components
    private ChunkData InstantiateNewChunk()
    {
        GameObject newChunkObject = new GameObject("Chunk");
        ChunkData newChunk = newChunkObject.AddComponent<ChunkData>();

        newChunkObject.AddComponent<MeshFilter>();
        newChunkObject.AddComponent<MeshRenderer>();
        newChunkObject.AddComponent<MeshCollider>();

        newChunkObject.layer = LayerMask.NameToLayer("Solid Block");
        newChunkObject.tag = "Solid Block";

        return newChunk;
    }


}