using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

// The chunk serializer takes chunk data and writes it to disk
// We store multiple chunks in a single region file


public static class ChunkSerializer
{
    private static string saveDirectory = "Chunks";


    // Save a chunk's voxel data to disk
    public static async Task SaveChunkAsync(Chunk chunk)
    {
        string filePath = GetChunkFilePath(chunk.globalChunkPos);

        Debug.Log($"[Save] Starting save for chunk {chunk.globalChunkPos}");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        Debug.Log($"[Save] Directory creation took {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        byte[] data = SerializeChunk(chunk);
        Debug.Log($"[Save] Serialization took {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        await File.WriteAllBytesAsync(filePath, data);
        Debug.Log($"[Save] File writing took {sw.ElapsedMilliseconds}ms");
    }


    // Load a chunk's voxel data from disk
    public static async Task<bool> LoadChunkAsync(Chunk chunk)
    {
        string filePath = GetChunkFilePath(chunk.globalChunkPos);

        if (File.Exists(filePath))
        {
            // Read file data asynchronously
            byte[] data = await File.ReadAllBytesAsync(filePath);

            // Deserialize into chunk
            DeserializeChunk(chunk, data);

            return true;
        }
        return false; // File not found
    }


    // Serialize the chunk's voxel data into a byte array
    private static byte[] SerializeChunk(Chunk chunk)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int currentRun = 0;
            byte currentType = 0;

            // Write chunk size
            writer.Write(chunk.chunkSize);

            // Iterate through all voxels
            for (int x = 0; x < chunk.chunkSize; x++)
                for (int y = 0; y < chunk.chunkSize; y++)
                    for (int z = 0; z < chunk.chunkSize; z++)
                    {
                        var voxel = chunk.GetVoxelLocal(x, y, z);
                        byte voxelType = voxel.type;  // Assuming Type is a byte

                        if (currentRun == 0)
                        {
                            currentType = voxelType;
                            currentRun = 1;
                        }
                        else if (voxelType == currentType && currentRun < 255)
                        {
                            currentRun++;
                        }
                        else
                        {
                            // Write the run
                            writer.Write(currentType);
                            writer.Write((byte)currentRun);
                            currentType = voxelType;
                            currentRun = 1;
                        }
                    }

            // Write final run
            if (currentRun > 0)
            {
                writer.Write(currentType);
                writer.Write((byte)currentRun);
            }

            return stream.ToArray();
        }
    }


    // Deserialize a byte array into the chunk's voxel data
    private static void DeserializeChunk(Chunk chunk, byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Voxel[,,] voxelData = (Voxel[,,])formatter.Deserialize(stream);

            for (int x = 0; x < chunk.chunkSize; x++)
            {
                for (int y = 0; y < chunk.chunkSize; y++)
                {
                    for (int z = 0; z < chunk.chunkSize; z++)
                    {
                        chunk.SetVoxelAt(x, y, z, voxelData[x, y, z]);
                    }
                }
            }
            Debug.Log("Loaded chunk at " + chunk.globalChunkPos);
        }
    }




    // Determine file path based on chunk position
    public static string GetChunkFilePath(Vector3 globalChunkPos)
    {
        return Path.Combine(saveDirectory, $"{(int)globalChunkPos.x}_{(int)globalChunkPos.y}_{(int)globalChunkPos.z}.chunk");
    }
}
