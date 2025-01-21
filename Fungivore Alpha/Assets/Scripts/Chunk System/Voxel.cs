using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from https://medium.com/@adamy1558/building-a-high-performance-voxel-engine-in-unity-a-step-by-step-guide-part-1-voxels-chunks-86275c079fb8

[System.Serializable]
public struct Voxel
{
    //public Vector3 position;
    public bool isActive;

    public VoxelType type; // Using the VoxelType enum
    public enum VoxelType
    {
        Air,    // Represents empty space
        Stone,
    }

    public Voxel(VoxelType type, bool isActive = true)
    {
        //this.position = position;
        this.type = type;
        this.isActive = isActive;
    }
}