using System.Collections.Generic;
using UnityEngine;

public static class UVHelper
{
    // Width of a side of the square texture atlas
    private static int atlasWidth = 8;

    // We take a voxel type and map it to the appropriate coordinates on the texture atlas
    // (column, row) starting from bottom left
    private static Dictionary<int, Vector2Int> voxelTypeToAtlasCoords = new Dictionary<int, Vector2Int>
    {
        {  Voxel.Type.Stone, new Vector2Int(3, 4) },
        {  Voxel.Type.Stone02, new Vector2Int(4, 5) },
    };

    // Return UV coordinates for a face based on the voxel type
    public static List<Vector2> GetFaceUVs(int voxelType)
    {
        // Look up the texture atlas coordinates for the voxel type
        if (!voxelTypeToAtlasCoords.TryGetValue(voxelType, out Vector2Int cellCoords))
        {
            // Default to the first cell if the voxel type is not found
            cellCoords = new Vector2Int(0, 0);
        }

        // Width of the side of a square atlas cell
        float cellWidth = 1f / atlasWidth;

        float uStart = cellCoords.x * cellWidth;
        float uEnd = (cellCoords.x + 1) * cellWidth;
        float vStart = cellCoords.y * cellWidth;
        float vEnd = (cellCoords.y + 1) * cellWidth;

        return new List<Vector2>
        {
            new Vector2(uStart, vStart),    // Bottom left corner
            new Vector2(uEnd, vStart),      // Bottom right corner
            new Vector2(uEnd, vEnd),        // Top right corner
            new Vector2(uStart, vEnd),      // Top left corner
        };
    }
}
