using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibAnt : Ant
{
    private List<AntMove> openMoves = new List<AntMove>();

    private int movesRemaining = 10000;


    public override void MoveNext()
    {
        if (movesRemaining >= 0)
        {
            //Debug.Log("Moves remaining:" + movesRemaining);

            movesRemaining--;

            // Reset the open moves for the ant
            openMoves.Clear();

            CheckForOpenMoves();

            if (openMoves.Count != 0)
            {
                // Pick a random open move

                var currentMove = openMoves[Random.Range(0, openMoves.Count)];

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

                World.Instance.totalVoxelCount++;
                currentChunk = World.Instance.GetChunkAt(antPos);
                if (currentChunk == null)
                {
                    Debug.Log("Couldn't find a chunk at " + antPos);
                }
                else
                {
                    currentChunk.SetBlock(antPos, Voxel.Type.Stone);
                }



                
            }
            else
            {
                if (BlockIsEmpty(antPos - Vector3.up))
                {
                    MoveDown();
                }
            }
        }
    }


    private void CheckForOpenMoves()
    {
        // Check if left move is valid
        var tempDirectionIndex = (directionIndex + 3) % directions.Length;
        Vector3 leftDir = directions[tempDirectionIndex];
        if (BlockIsEmpty(antPos + leftDir))
        {
            openMoves.Add(AntMove.TurnLeftAndMove);
        }

        // Check if forward move is valid
        if (BlockIsEmpty(antPos + antDir))
        {
            openMoves.Add(AntMove.MoveForward);
        }

        tempDirectionIndex = (directionIndex + 1) % directions.Length;
        // Check if right move is valid
        Vector3 rightDir = directions[tempDirectionIndex];

        if (BlockIsEmpty(antPos + rightDir))
        {
            openMoves.Add(AntMove.TurnRightAndMove);
        }
    }


}
