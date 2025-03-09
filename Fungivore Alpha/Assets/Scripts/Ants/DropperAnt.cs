using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperAnt : Ant
{
    private List<AntMove> openMoves = new List<AntMove>();


    // Set the ant's starting moves in its constructor
    public DropperAnt()
    {
        movesRemaining = 20;
    }


    public override void MoveNext()
    {
        movesRemaining--;

        // Reset the open moves for the ant
        openMoves.Clear();

        CheckForOpenMoves();

        World.Instance.SetBlockGlobal(antPos, Voxel.Type.Stone02);

        if (openMoves.Count != 0)
        {
            // Pick a random open move
            var currentMove = openMoves[Random.Range(0, openMoves.Count)];

            switch (currentMove)
            {
                case AntMove.MoveDown:
                    MoveDown();
                    break;
                case AntMove.TurnLeftAndMove:
                    TurnLeftAndMove();
                    break;
                case AntMove.MoveForward:
                    MoveForward();
                    break;
                case AntMove.TurnRightAndMove:
                    TurnRightAndMove();
                    break;
            }
        }
        else
        {
            movesRemaining = 0;
        }
    }


    private void CheckForOpenMoves()
    {
        // Check if down move is valid
        // Since this is a dropper, we want to heavily bias downward movement
        // so we add it multiple times
        if (BlockIsEmpty(antPos - Vector3.up))
        {
            openMoves.Add(AntMove.MoveDown);
            openMoves.Add(AntMove.MoveDown);
            openMoves.Add(AntMove.MoveDown);
        }

        // Check if forward move is valid
        if (BlockIsEmpty(antPos + antDir))
        {
            openMoves.Add(AntMove.MoveForward);
        }

        // Check if left move is valid
        var tempDirectionIndex = (directionIndex + 3) % directions.Length;
        Vector3 leftDir = directions[tempDirectionIndex];
        if (BlockIsEmpty(antPos + leftDir))
        {
            openMoves.Add(AntMove.TurnLeftAndMove);
        }

        // Check if right move is valid
        tempDirectionIndex = (directionIndex + 1) % directions.Length;
        Vector3 rightDir = directions[tempDirectionIndex];

        if (BlockIsEmpty(antPos + rightDir))
        {
            openMoves.Add(AntMove.TurnRightAndMove);
        }
    }


}
