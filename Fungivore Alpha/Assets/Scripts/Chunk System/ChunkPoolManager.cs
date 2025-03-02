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
        GameObject chunkObject;

        if (chunkPool.Count > 0)
        {
            chunkObject = chunkPool.Dequeue();
        }
        else
        {
            chunkObject = InstantiateNewChunk();
        }
        return chunkObject;
    }

    public void ReturnChunk(GameObject chunkObject)
    {
        chunkObject.gameObject.SetActive(false);
        chunkPool.Enqueue(chunkObject);
    }




}