using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant
{
    // Global position of the ant
    public Vector3 antPos;

    public Vector3 antDir;

    public int directionIndex;

    public Chunk currentChunk;

    // Cardinal directions winding clockwise
    public readonly Vector3[] directions = {
        new Vector3(0, 0, 1),  // Positive Z North
        new Vector3(1, 0, 0),  // Positive X East
        new Vector3(0, 0, -1), // Negative Z South
        new Vector3(-1, 0, 0), // Negative X West
    };

    public enum AntMove
    { 
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        TurnLeftAndMove,
        TurnRightAndMove
    }




    public virtual void MoveNext()
    { 

    }


    public bool BlockIsEmpty(Vector3 position)
    {
        currentChunk = World.Instance.GetChunkAt(position);
        if (currentChunk != null && currentChunk.GetBlockGlobal(position) == Voxel.VoxelType.Air)
        {
            // Also check a overlap box here for entities
            return true;
        }
        else
        {
            return false;
        }
    }




    public void RandomizeDirection()
    {
        directionIndex = Random.Range(0, directions.Length);
        antDir = directions[directionIndex];
    }

    public void MoveForward()
    {
        antPos += antDir;
    }

    public void MoveBackwards()
    {
        antPos -= antDir;
    }



    public void MoveUp()
    {
        antPos += Vector3.up;
    }

    public void MoveDown()
    {
        antPos += Vector3.down;
    }

    public void TurnRightAndMove()
    {
        directionIndex = (directionIndex + 1) % directions.Length;
        antDir = directions[directionIndex];
    }

    public void TurnLeftAndMove()
    {
        directionIndex = (directionIndex + 3) % directions.Length;
        antDir = directions[directionIndex];
    }



}
