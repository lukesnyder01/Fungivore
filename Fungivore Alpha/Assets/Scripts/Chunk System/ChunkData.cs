using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


// from https://medium.com/@adamy1558/building-a-high-performance-voxel-engine-in-unity-a-step-by-step-guide-part-1-voxels-chunks-86275c079fb8

public class ChunkData
{
    public Voxel[,,] voxels;
    private int chunkSize;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public GameObject chunkObject;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public LayerMask chunkLayer;

    private int randomNoiseDensity = 0;

    public Vector3 globalChunkPos;

    public ChunkState chunkState;
    public bool ChunkSavedToDisk = false;

    public enum ChunkState
    { 
        Idle, // Can accept block updates
        Queued, // Has pending block updates and has been added to the queue
        Processing, // Inaccessible until work on background thread is complete
    }


    public void Initialize(GameObject pooledChunk)
    {
        chunkSize = World.Instance.chunkSize;

        chunkState = ChunkState.Idle;

        chunkObject = pooledChunk;
        meshFilter = pooledChunk.GetComponent<MeshFilter>();
        meshRenderer = pooledChunk.GetComponent<MeshRenderer>();
        meshCollider = pooledChunk.GetComponent<MeshCollider>();

        voxels = new Voxel[chunkSize, chunkSize, chunkSize];

        InitializeVoxels();
    }

    private async void InitializeVoxels()
    {
        string filePath = ChunkSerializer.GetChunkFilePath(this.globalChunkPos);
        if (File.Exists(filePath))
        {
            await Task.Run(() => LoadVoxelsAndGenerate());
            GenerateMesh();
        }
        else
        {
            RandomizeVoxels();
            ChunkSavedToDisk = true;
            GenerateMesh();
        }
    }


    private async Task LoadVoxelsAndGenerate()
    {
        await ChunkSerializer.LoadChunkAsync(this);
    }


    private void RandomizeVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    // Use world coordinates for noise sampling
                    Vector3 worldPos = globalChunkPos + new Vector3(x, y, z);
                    byte type = DetermineVoxelType(worldPos.x, worldPos.y, worldPos.z);
                    voxels[x, y, z] = new Voxel(type, type != Voxel.Type.Air);

                }
            }
        }
    }


    public void SetBlockLocal(Vector3 localBlockPos, byte type)
    {
        if (chunkState != ChunkState.Processing)
        {
            voxels[(int)localBlockPos.x, (int)localBlockPos.y, (int)localBlockPos.z] = new Voxel(type, true);

            if (chunkState == ChunkState.Idle)
            {
                World.Instance.AddChunkToQueue(this);
                chunkState = ChunkState.Queued;
            }
        }
        else
        {
            Debug.Log("Can't add chunk " + globalChunkPos + " to queue because it's processing");
        }
    }


    public void RegenerateChunk()
    {
        if (chunkState != ChunkState.Processing)
        {           
            chunkState = ChunkState.Processing;
            GenerateMesh();
        }
        else
        {
            Debug.Log("Tried to regenerate a chunk while it was processing");
        }

    }


    public byte GetBlockGlobal(Vector3 globalBlockPos)
    {
        Vector3 localBlockPos = globalBlockPos - globalChunkPos;
        return voxels[(int)localBlockPos.x, (int)localBlockPos.y, (int)localBlockPos.z].type;
    }


    public Voxel GetVoxelLocal(int x, int y, int z)
    {
        return voxels[x, y, z];
    }


    public void SetVoxelAt(int x, int y, int z, Voxel voxelData)
    {
        voxels[x, y, z] = new Voxel(voxelData.type, voxelData.isActive);
    }

    

    public async void GenerateMesh()
    {
        chunkState = ChunkState.Processing;
        ClearMeshArrays();
        await Task.Run(() => IterateVoxels());
        ApplyMeshData();

        // Save to disk after generating the mesh
        await Task.Run(() => ChunkSerializer.SaveChunkAsync(this));

        //Debug.Log("Saved chunk at " + globalChunkPos);

        chunkState = ChunkState.Idle;
    }


    private void ApplyMeshData()
    {
        if (vertices.Count > 0)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.RecalculateNormals(); // Important for lighting
            mesh.RecalculateTangents(); // Fixes x axis aligned faces not displaying bump maps correctly

            meshFilter.mesh = mesh;
            meshCollider.enabled = false;
            meshCollider.sharedMesh = mesh;
            meshCollider.enabled = true;

            // Apply a material or texture if needed
            meshRenderer.material = World.Instance.VoxelMaterial;
        }
    }


    private byte DetermineVoxelType(float x, float y, float z)
    {
        float noiseValue = Random.Range(0, 1000);

        if (y == 0)
        {
            if (ThreadSafeRandom.Next(0, 100) < 50)
            {
                return Voxel.Type.Stone;
            }
            else
            {
                return Voxel.Type.Stone02;
            }
        }

        if (noiseValue < randomNoiseDensity && y > 0)
        {
            World.Instance.totalVoxelCount++;
            if (ThreadSafeRandom.Next(0, 100) < 50)
            {
                return Voxel.Type.Stone;
            }
            else
            {
                return Voxel.Type.Stone02;
            }
        }

        else
            return Voxel.Type.Air;
    }


    private async Task IterateVoxels()
    {
        // Use Task.Run to move IterateVoxels to a background thread
        await Task.Run(() =>
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        ProcessVoxel(x, y, z);
                    }
                }
            }
        });
    }


    private void ProcessVoxel(int x, int y, int z)
    {
        // Check if the voxels array is initialized and the indices are within bounds
        if (voxels == null || x < 0 || x >= voxels.GetLength(0) ||
            y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        }
        Voxel voxel = voxels[x, y, z];
        if (voxel.isActive)
        {
            // Check each face of the voxel for visibility
            bool[] facesVisible = new bool[6];

            // Check visibility for each face
            facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
            facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
            facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
            facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                    AddFaceData(x, y, z, i, voxel.type); // Method to add mesh data for the visible face
            }
        }
    }


    private bool IsFaceVisible(int x, int y, int z)
    {
        // Convert local chunk coordinates to global coordinates
        Vector3 globalPos = globalChunkPos + new Vector3(x, y, z);

        // Check if the neighboring voxel is inactive or out of bounds in the current chunk
        // and also if it's inactive or out of bounds in the world (neighboring chunks)
        return IsVoxelHiddenInChunk(x, y, z) && IsVoxelHiddenInWorld(globalPos);
    }


    private bool IsVoxelHiddenInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize)
            return true; // Face is at the boundary of the chunk
        return !voxels[x, y, z].isActive;
    }


    private bool IsVoxelHiddenInWorld(Vector3 globalPos)
    {
        // Check if there is a chunk at the global position
        ChunkData neighborChunk = World.Instance.GetChunkAt(globalPos);
        if (neighborChunk == null)
        {
            // No chunk at this position, so the voxel face should be hidden
            return true;
        }
        
        // Convert the global position to the local position within the neighboring chunk
        //Vector3 localPos = neighborChunk.transform.InverseTransformPoint(globalPos);

        // The voxel's local position in the neighbor chunk
        // is the voxel's global pos - the neighbor chunk's global pos
        Vector3 localPos = globalPos - neighborChunk.globalChunkPos;

        // If the voxel at this local position is inactive, the face should be visible (not hidden)
        return !neighborChunk.IsVoxelActiveAt(localPos);
    }


    public bool IsVoxelActiveAt(Vector3 localPosition)
    {
        // Round the local position to get the nearest voxel index
        int x = Mathf.RoundToInt(localPosition.x);
        int y = Mathf.RoundToInt(localPosition.y);
        int z = Mathf.RoundToInt(localPosition.z);

        // Check if the indices are within the bounds of the voxel array
        if (x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
        {
            // Return the active state of the voxel at these indices
            return voxels[x, y, z].isActive;
        }

        // If out of bounds, consider the voxel inactive
        return false;
    }


    private void AddFaceData(int x, int y, int z, int faceIndex, int voxelType)
    {
        // Based on faceIndex, determine vertices and triangles
        // Add vertices and triangles for the visible face
        // Calculate and add corresponding UVs

        if (faceIndex == 0) // Top Face
        {
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.AddRange(UVHelper.GetFaceUVs(voxelType));
        }

        if (faceIndex == 1) // Bottom Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x, y, z + 1));
            uvs.AddRange(UVHelper.GetFaceUVs(voxelType));
        }

        if (faceIndex == 2) // Left Face
        {
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));
            uvs.AddRange(UVHelper.GetFaceUVs(voxelType));
        }

        if (faceIndex == 3) // Right Face
        {
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            uvs.AddRange(UVHelper.GetFaceUVs(voxelType));
        }

        if (faceIndex == 4) // Front Face
        {
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            uvs.AddRange(UVHelper.GetFaceUVs(voxelType));
        }

        if (faceIndex == 5) // Back Face
        {
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            uvs.AddRange(UVHelper.GetFaceUVs(voxelType));
        }
        AddTriangleIndices();
    }


    private void AddTriangleIndices()
    {
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }


    public void ResetChunkData()
    {
        // Clear voxel data
        voxels = new Voxel[chunkSize, chunkSize, chunkSize];

        ClearMeshArrays();
    }

    public void ClearChunkMesh()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            meshFilter.sharedMesh.Clear();

            meshCollider.sharedMesh = null;

        }
    }

    public void ClearMeshArrays()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

}