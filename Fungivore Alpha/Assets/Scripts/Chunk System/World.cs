using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
//using System.Diagnostics;


// Initial system built based on https://medium.com/@adamy1558/building-a-high-performance-voxel-engine-in-unity-a-step-by-step-guide-part-1-voxels-chunks-86275c079fb8

public class World : MonoBehaviour
{
    public int totalVoxelCount = 0;

    public readonly int chunkSize = 16;

    private Dictionary<Vector3, ChunkData> globalChunkData;
    private Dictionary<Vector3, ChunkData> activeChunks;



    public static World Instance { get; private set; }

    public Material VoxelMaterial;

    private PlayerController playerController;
    private Vector3 playerPosition;

    public int loadRadius = 5;
    public int unloadRadius = 7;


    private Vector3Int lastPlayerChunkCoordinates;
    private int chunksMovedCount = 0;
    public int chunkUpdateThreshold = 1; // How many chunks the player has to move before updating
    private bool JustStarted = true;


    private Queue<Vector3> chunkLoadQueue = new Queue<Vector3>();
    public int chunksPerFrame = 2; // Number of chunks to load per frame
    public int loadInterval = 1; // Load chunks every 4 frames
    private int frameCounter = 0;


    private Queue<Vector3> chunkUnloadQueue = new Queue<Vector3>();
    private int unloadFrameCounter = 0;
    private int unloadInterval = 2;
    private int chunksPerFrameUnloading = 10;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        activeChunks = new Dictionary<Vector3, ChunkData>();


        globalChunkData = new Dictionary<Vector3, ChunkData>();
        // Load global chunk data from world file on disk


        ChunkPoolManager.Instance.PopulateInitialPool();
    }


    void Update()
    {
        playerPosition = playerController.GetPlayerPosition();
        UpdateChunks(playerPosition);
        ProcessChunkLoadingQueue();
        ProcessChunkUnloadingQueue();
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


    // Returns the chunk that the given coordinates are inside
    public ChunkData GetChunkAt(Vector3 globalPosition)
    {
        // Calculate the chunk's starting position based on the global position
        Vector3Int chunkCoordinates = new Vector3Int(
            Mathf.FloorToInt(globalPosition.x / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.y / chunkSize) * chunkSize,
            Mathf.FloorToInt(globalPosition.z / chunkSize) * chunkSize
        );

        // Retrieve and return the chunk at the calculated position
        if (activeChunks.TryGetValue(chunkCoordinates, out ChunkData chunk))
        {
            return chunk;
        }

        // Return null if no chunk exists at the position
        return null;
    }

    public void SetBlockGlobal(Vector3 globalBlockPos, byte blockType)
    {
        ChunkData selectedChunk = World.Instance.GetChunkAt(globalBlockPos);
        if (selectedChunk != null)
        {
            World.Instance.totalVoxelCount++;
            Vector3 localChunkPosition = globalBlockPos - selectedChunk.globalChunkPos;
            selectedChunk.SetBlockLocal(localChunkPosition, blockType);
        }
        else
        {
            Debug.Log("Couldn't find a chunk at " + globalBlockPos);
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
                        if (!activeChunks.ContainsKey(chunkPosition))
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

    public void AddChunkToQueue(ChunkData chunk)
    {
        chunkLoadQueue.Enqueue(chunk.globalChunkPos);
        //Debug.Log(chunkLoadQueue.Count + "Chunks in queue");
    }


    private void ProcessChunkLoadingQueue()
    {
        frameCounter++;

        if (frameCounter % loadInterval == 0)
        {
            for (int i = 0; i < chunksPerFrame && chunkLoadQueue.Count > 0; i++)
            {
                Vector3 chunkPosition = chunkLoadQueue.Dequeue();

                // If there isn't already a chunk object at this position, we get one from the pool an initialize it
                if (!activeChunks.ContainsKey(chunkPosition))
                {
                    // Get a new chunk GameObject from the pool
                    GameObject newChunkObject = ChunkPoolManager.Instance.GetChunk(); // Get a chunk from the pool
                    newChunkObject.transform.position = chunkPosition; // Move the chunk to the right position
                    newChunkObject.transform.parent = this.transform; // Nests chunk GameObjects under World

                    ChunkData newChunkData;

                    // If the chunk data exists in the global chunk list
                    // get that chunk data
                    if (globalChunkData.ContainsKey(chunkPosition))
                    {
                        newChunkData = globalChunkData[chunkPosition];
                        newChunkData.Initialize(newChunkObject);
                    }
                    else 
                    {
                        // If the global chunk list doesn't have a chunk defined there
                        // create a new ChunkData and pass it newChunkObject and add to the global chunk dictionary
                        newChunkData = new ChunkData();
                        newChunkData.globalChunkPos = chunkPosition;
                        newChunkData.Initialize(newChunkObject);
                        globalChunkData.Add(chunkPosition, newChunkData);
                    }


                    activeChunks.Add(chunkPosition, newChunkData);
                    newChunkObject.gameObject.SetActive(true);

                    

                }
                else // If there's already a chunk at this position it's in the queue because we want to regenerate it because of block updates
                {
                    activeChunks[chunkPosition].RegenerateChunk();
                }

                // If the chunk had blocks added to it while it was processing, it'll get marked dirty.
                // So, we want to add it right back to the queue
                /*
                if (activeChunks[chunkPosition].dirty == true)
                {
                    AddChunkToQueue(activeChunks[chunkPosition]);
                    //activeChunks[chunkPosition].chunkState = ChunkData.ChunkState.Queued;
                    activeChunks[chunkPosition].dirty = false;

                    Debug.Log("Chunk " + chunkPosition + " added back to queue and no longer marked as dirty");
                }
                */

            }

        }
    }


    void UnloadDistantChunks(Vector3Int centerChunkCoordinates)
    {
        List<Vector3> chunksToUnload = new List<Vector3>();

        foreach (var chunk in activeChunks)
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

                    // Null-conditional operator is super cool
                    GameObject chunkToUnload = GetChunkAt(chunkPosition)?.chunkObject;

                    if (chunkToUnload != null)
                    {
                        activeChunks[chunkPosition].ClearChunkMesh();
                        ChunkPoolManager.Instance.ReturnChunk(chunkToUnload);
                        globalChunkData[chunkPosition] = activeChunks[chunkPosition];
                        activeChunks.Remove(chunkPosition); // Remove the chunk from the active chunks dictionary
                    }
                }
            }
        }
    }




}