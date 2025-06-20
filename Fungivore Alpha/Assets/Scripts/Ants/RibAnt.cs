using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibAnt : Ant
{
    private List<AntMove> openMoves = new List<AntMove>();


    // Set the ant's starting moves in its constructor
    public RibAnt()
    {
        movesRemaining = 10000;
    }


    public override void MoveNext()
    {
        // if the ant is in an unloaded chunk, don't do anything
        if (World.Instance.GetChunkAt(antPos) == null)
        {
            return;
        }


        movesRemaining--;

        // Reset the open moves for the ant
        openMoves.Clear();

        CheckForOpenMoves();

        if (openMoves.Count != 0)
        {
            // Pick a random open move

            var currentMove = openMoves[Random.Range(0, openMoves.Count)];

            World.Instance.SetBlockGlobal(antPos, Voxel.Type.Stone02);

            switch (currentMove)
            {
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
        else // If there are no flat moves, try moving downward
        {
            if (BlockIsEmpty(antPos - Vector3.up))
            {
                MoveDown();
            }
            else // If down isn't available, the ant is stuck and should be removed
            {
                movesRemaining = 0;
            }
        }
    }


    private void CheckForOpenMoves()
    {
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
