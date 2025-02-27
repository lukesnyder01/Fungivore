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
    public static async Task SaveChunkAsync(ChunkData chunk)
    {
        string filePath = GetChunkFilePath(chunk.globalChunkPos);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        byte[] data = SerializeChunk(chunk);

        await File.WriteAllBytesAsync(filePath, data);
    }


    // Load a chunk's voxel data from disk
    public static async Task LoadChunkAsync(ChunkData chunk)
    {
        string filePath = GetChunkFilePath(chunk.globalChunkPos);

        if (File.Exists(filePath))
        {
            // Read file data asynchronously
            byte[] data = await File.ReadAllBytesAsync(filePath);

            // Deserialize into chunk
            DeserializeChunk(chunk, data);
        }
        else
        {
            Debug.Log("No chunk file found at " + filePath);
        }
    }


    // Serialize the chunk's voxel data into a byte array
    private static byte[] SerializeChunk(ChunkData chunk)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // Write the chunk size
            writer.Write(chunk.chunkSize);

            // Write each voxel's type directly
            for (int x = 0; x < chunk.chunkSize; x++)
            {
                for (int y = 0; y < chunk.chunkSize; y++)
                {
                    for (int z = 0; z < chunk.chunkSize; z++)
                    {
                        Voxel voxel = chunk.GetVoxelLocal(x, y, z);
                        writer.Write(voxel.type); // Directly write voxel type (byte)
                    }
                }
            }

            return stream.ToArray();
        }
    }


    private static void DeserializeChunk(ChunkData chunk, byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            // Read the chunk size
            int chunkSize = reader.ReadInt32();

            int solidBlockCount = 0;

            if (chunkSize != chunk.chunkSize)
            {
                Debug.LogError($"Chunk size mismatch: expected {chunk.chunkSize}, got {chunkSize}");
                return;
            }

            // Read each voxel's type directly
            for (int x = 0; x < chunk.chunkSize; x++)
            {
                for (int y = 0; y < chunk.chunkSize; y++)
                {
                    for (int z = 0; z < chunk.chunkSize; z++)
                    {
                        byte voxelType = reader.ReadByte(); // Read voxel type (byte)

                        if (voxelType == 1)
                        {
                            solidBlockCount++;
                        }

                        Voxel voxel = new Voxel(voxelType, voxelType != 0);
                        chunk.SetVoxelAt(x, y, z, voxel);
                    }
                }
            }

            Debug.Log("Loaded chunk at " + chunk.globalChunkPos + "with " + solidBlockCount + " blocks");
        }
    }






    // Determine file path based on chunk position
    public static string GetChunkFilePath(Vector3 globalChunkPos)
    {
        return Path.Combine(saveDirectory, $"{(int)globalChunkPos.x}_{(int)globalChunkPos.y}_{(int)globalChunkPos.z}.chunk");
    }
}
