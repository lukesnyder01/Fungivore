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

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        // Serialize voxel data
        byte[] data = SerializeChunk(chunk);

        // Write to disk asynchronously
        await File.WriteAllBytesAsync(filePath, data);
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
        {
            BinaryFormatter formatter = new BinaryFormatter();

            // Create a simple object to store voxel data
            var voxelData = new Voxel[chunk.chunkSize, chunk.chunkSize, chunk.chunkSize];
            for (int x = 0; x < chunk.chunkSize; x++)
            {
                for (int y = 0; y < chunk.chunkSize; y++)
                {
                    for (int z = 0; z < chunk.chunkSize; z++)
                    {
                        voxelData[x, y, z] = chunk.GetVoxelLocal(x, y, z);
                    }
                }
            }

            formatter.Serialize(stream, voxelData);
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
                        Debug.Log(voxelData[x, y, z].type);
                        chunk.SetVoxelAt(x, y, z, voxelData[x, y, z]);
                    }
                }
            }
        }
    }




    // Determine file path based on chunk position
    private static string GetChunkFilePath(Vector3 globalChunkPos)
    {
        return Path.Combine(saveDirectory, $"{(int)globalChunkPos.x}_{(int)globalChunkPos.y}_{(int)globalChunkPos.z}.chunk");
    }
}
