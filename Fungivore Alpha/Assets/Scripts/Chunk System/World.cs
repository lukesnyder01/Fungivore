using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://medium.com/@adamy1558/building-a-high-performance-voxel-engine-in-unity-a-step-by-step-guide-part-1-voxels-chunks-86275c079fb8

public class World : MonoBehaviour
{
    public int totalVoxelCount = 0;

    private int chunkSize = 16;

    private Dictionary<Vector3, Chunk> chunks;
    public static World Instance { get; private set; }

    public Material VoxelMaterial;

    private PlayerController playerController;
    private Vector3 playerPosition;

    public int loadRadius = 5;
    public int unloadRadius = 7;


    private Vector3Int lastPlayerChunkCoordinates;
    private int chunksMovedCount = 0;
    public int chunkUpdateThreshold = 1; // Update every 5 chunks
    private bool JustStarted = true;


    private Queue<Vector3> chunkLoadQueue = new Queue<Vector3>();
    public int chunksPerFrame = 2; // Number of chunks to load per frame
    public int loadInterval = 1; // Load chunks every 4 frames
    private int frameCounter = 0;


    private Queue<Vector3> chunkUnloadQueue = new Queue<Vector3>();
    private int unloadFrameCounter = 0;
    private int unloadInterval = 5;
    private int chunksPerFrameUnloading = 4;


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
        lastPlayerChunkCoordinates = Vector3Int.zero;
        chunks = new Dictionary<Vector3, Chunk>();
        ChunkPoolManager.Instance.PopulateInitialPool();
    }


    void Update()
    {
        Debug.Log(World.Instance.totalVoxelCount);

        playerPosition = playerController.GetPlayerPosition();
        UpdateChunks(playerPosition);
        ProcessChunkLoadingQueue();
        ProcessChunkUnloadingQueue();
    }



    // Returns the chunk that the given coordinates are inside
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
        Vector3Int playerChunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize),
            Mathf.FloorToInt(playerPosition.z / chunkSize));

        // Check if player has moved to a new chunk
        if (!playerChunkCoordinates.Equals(lastPlayerChunkCoordinates))
        {
            if (chunksMovedCount >= chunkUpdateThreshold || JustStarted)
            {
                LoadChunksAround(playerChunkCoordinates);
                UnloadDistantChunks(playerChunkCoordinates);
                JustStarted = false;
                chunksMovedCount = 0;
            }

            lastPlayerChunkCoordinates = playerChunkCoordinates;
            chunksMovedCount++;
        }
    }


    void LoadChunksAround(Vector3Int centerChunkCoordinates)
    {
        // Temporary list to store chunks with their squared distances
        List<(Vector3 chunkPosition, int distanceSquared)> chunksToLoad = new List<(Vector3, int)>();

        for (int y = -loadRadius; y <= loadRadius; y++)
        {
            int maxHorizontalRadius = Mathf.FloorToInt(Mathf.Sqrt(loadRadius * loadRadius - y * y));

            for (int x = -maxHorizontalRadius; x <= maxHorizontalRadius; x++)
            {
                for (int z = -maxHorizontalRadius; z <= maxHorizontalRadius; z++)
                {
                    // Calculate the squared distance
                    int distanceSquared = x * x + y * y + z * z;

                    if (distanceSquared <= loadRadius * loadRadius)
                    {
                        Vector3Int chunkCoordinates = new Vector3Int(centerChunkCoordinates.x + x, centerChunkCoordinates.y + y, centerChunkCoordinates.z + z);
                        Vector3 chunkPosition = new Vector3(chunkCoordinates.x * chunkSize, chunkCoordinates.y * chunkSize, chunkCoordinates.z * chunkSize);

                        // Check to make sure there isn't already a chunk in that position
                        if (!chunks.ContainsKey(chunkPosition))
                        {
                            // Add chunk and its distance to the list
                            chunksToLoad.Add((chunkPosition, distanceSquared));
                        }
                    }
                }
            }
        }
        
        // Sort chunks by distance so we load the closest first
        chunksToLoad.Sort((a, b) => a.distanceSquared.CompareTo(b.distanceSquared));

        // Enqueue sorted chunks for loading
        foreach (var chunk in chunksToLoad)
        {
            chunkLoadQueue.Enqueue(chunk.chunkPosition);
        }

    }

    public void AddChunkToQueue(Chunk chunk)
    {
        chunkLoadQueue.Enqueue(chunk.globalChunkPos);
        Debug.Log(chunkLoadQueue.Count + "Chunks in queue");
    }


    void ProcessChunkLoadingQueue()
    {
        frameCounter++;

        if (frameCounter % loadInterval == 0)
        {
            for (int i = 0; i < chunksPerFrame && chunkLoadQueue.Count > 0; i++)
            {
                Vector3 chunkPosition = chunkLoadQueue.Dequeue();

                // If there isn't already a chunk at this position, we get one from the pool an initialize it
                if (!chunks.ContainsKey(chunkPosition))
                {
                    Chunk chunkObject = ChunkPoolManager.Instance.GetChunk(); // Get a chunk from the pool
                    chunkObject.transform.position = chunkPosition;
                    chunkObject.transform.parent = this.transform; // Nests chunk GameObjects under World
                    chunkObject.Initialize(chunkSize); // Initialize the chunk
                    chunks.Add(chunkPosition, chunkObject); // Add the chunk to the dictionary
                    chunkObject.gameObject.SetActive(true);
                }
                else // If there's already a chunk at this position it's in the queue because we want to regenerate it because of block updates
                {
                    Chunk chunk = chunks[chunkPosition];
                    chunk.RegenerateChunk();
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
                chunkUnloadQueue.Enqueue(chunk.Key);
            }
        }
    }


    void ProcessChunkUnloadingQueue()
    {
        // Check if there are chunks in the unload queue
        if (chunkUnloadQueue.Count > 0)
        {
            unloadFrameCounter++;
            if (unloadFrameCounter % unloadInterval == 0)
            {
                int chunksToProcess = Mathf.Min(chunksPerFrameUnloading, chunkUnloadQueue.Count);
                for (int i = 0; i < chunksToProcess; i++)
                {
                    Vector3 chunkPosition = chunkUnloadQueue.Dequeue();
                    Chunk chunkToUnload = GetChunkAt(chunkPosition);
                    if (chunkToUnload != null)
                    {
                        ChunkPoolManager.Instance.ReturnChunk(chunkToUnload);
                        chunks.Remove(chunkPosition); // Remove the chunk from the active chunks dictionary
                    }
                }
            }
        }
    }




}