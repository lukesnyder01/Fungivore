using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant
{
    // Global position of the ant
    public Vector3 antPos;

    public Vector3 antDir;

    private int directionIndex;

    // Cardinal directions winding clockwise
    private readonly Vector3[] directions = {
        new Vector3(0, 0, 1),  // North
        new Vector3(1, 0, 0),  // East
        new Vector3(0, 0, -1), // South
        new Vector3(-1, 0, 0)  // West
    };

    public void RandomizeDirection()
    {
        directionIndex = Random.Range(0, directions.Length);
        antDir = directions[directionIndex];
    }

    public void MoveForward()
    {
        antPos += antDir;
    }

    public void MoveUp()
    {
        antPos += Vector3.up;
    }

    public void MoveDown()
    {
        antPos += Vector3.down;
    }

    public void TurnRight()
    {
        directionIndex = (directionIndex + 1) % directions.Length;
        antDir = directions[directionIndex];
    }

    public void TurnLeft()
    {
        directionIndex = (directionIndex - 1) % directions.Length;
        antDir = directions[directionIndex];
    }



}
