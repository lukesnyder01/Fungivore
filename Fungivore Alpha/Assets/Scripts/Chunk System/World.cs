using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://medium.com/@adamy1558/building-a-high-performance-voxel-engine-in-unity-a-step-by-step-guide-part-1-voxels-chunks-86275c079fb8
public class World : MonoBehaviour
{
    public int worldSize = 5; // Size of the world in number of chunks
    private int chunkSize = 16; // Assuming chunk size is 16x16x16

    private Dictionary<Vector3, Chunk> chunks;
    public static World Instance { get; private set; }

    public Material VoxelMaterial;

    private PlayerController playerController;
    private Vector3 playerPosition;

    public int loadRadius = 5;
    public int unloadRadius = 7;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want this to persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        chunks = new Dictionary<Vector3, Chunk>();
        GenerateWorld();
    }

    void Update()
    {
        playerPosition = playerController.GetPlayerPosition();
        UpdateChunks(playerPosition);
    }


    private void GenerateWorld()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    GameObject newChunkObject = new GameObject($"Chunk_{x}_{y}_{z}");
                    newChunkObject.transform.position = chunkPosition;
                    newChunkObject.transform.parent = this.transform;

                    Chunk newChunk = newChunkObject.AddComponent<Chunk>();
                    newChunk.Initialize(chunkSize);
                    chunks.Add(chunkPosition, newChunk);
                }
            }
        }
    }


    public Chunk GetChunkAt(Vector3 globalPosition)
    {
        // Calculate the chunk's starting position based on the global position
        Vector3Int chunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(globalPosition.x / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.y / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.z / chunkSize) * chunkSize
        );

        // Retrieve and return the chunk at the calculated position
        if (chunks.TryGetValue(chunkCoordinates, out Chunk chunk))
        {
            return chunk;
        }

        // Return null if no chunk exists at the position
        return null;
    }


    void UpdateChunks(Vector3 playerPosition)
    {
        // Determine the chunk coordinates for the player's position
        Vector3Int playerChunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize),
            Mathf.FloorToInt(playerPosition.z / chunkSize));

        // Load and unload chunks based on the player's position
        LoadChunksAround(playerChunkCoordinates);
        UnloadDistantChunks(playerChunkCoordinates);
    }

    void LoadChunksAround(Vector3Int centerChunkCoordinates)
    {
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int z = -loadRadius; z <= loadRadius; z++)
            {
                Vector3Int chunkCoordinates = new Vector3Int(centerChunkCoordinates.x + x, 0, centerChunkCoordinates.z + z);
                Vector3 chunkPosition = new Vector3(chunkCoordinates.x * chunkSize, 0, chunkCoordinates.z * chunkSize);
                if (!chunks.ContainsKey(chunkPosition))
                {
                    GameObject chunkObject = new GameObject($"Chunk_{chunkCoordinates.x}_{chunkCoordinates.z}");
                    chunkObject.transform.position = chunkPosition;
                    chunkObject.transform.parent = this.transform; // Optional, for organizational purposes

                    Chunk newChunk = chunkObject.AddComponent<Chunk>();
                    newChunk.Initialize(chunkSize); // Initialize the chunk with its size

                    chunks.Add(chunkPosition, newChunk); // Add the chunk to the dictionary
                }
            }
        }
    }


    void UnloadDistantChunks(Vector3Int centerChunkCoordinates)
    {
        List<Vector3> chunksToUnload = new List<Vector3>();
        foreach (var chunk in chunks)
        {
            Vector3Int chunkCoord = new Vector3Int(
                Mathf.FloorToInt(chunk.Key.x / chunkSize),
                Mathf.FloorToInt(chunk.Key.y / chunkSize),
                Mathf.FloorToInt(chunk.Key.z / chunkSize));

            if (Vector3Int.Distance(chunkCoord, centerChunkCoordinates) > unloadRadius)
            {
                chunksToUnload.Add(chunk.Key);
            }
        }

        foreach (var chunkPos in chunksToUnload)
        {
            Destroy(chunks[chunkPos].gameObject);
            chunks.Remove(chunkPos);
        }
    }


}