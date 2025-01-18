using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibAnt : Ant
{
    private List<AntMove> moves = new List<AntMove>
    {
        AntMove.TurnLeftAndMove,
        AntMove.MoveForward,
        AntMove.TurnLeftAndMove
    };

    public override void MoveNext()
    {
        // Pick a random move
        var currentMove = moves[Random.Range(0, moves.Count)];

        switch (currentMove)
        {
            case AntMove.TurnLeftAndMove:
                Vector3 leftDir = directions[(directionIndex + 1) % directions.Length];
                if (BlockIsEmpty(antPos + leftDir))
                {
                    TurnLeftAndMove();
                }
                break;
            case AntMove.MoveForward:
                if (BlockIsEmpty(antPos + antDir))
                {
                    MoveForward();
                }
                break;
            case AntMove.TurnRightAndMove:
                Vector3 rightDir = directions[(directionIndex - 1) % directions.Length];
                if (BlockIsEmpty(antPos + rightDir))
                {
                    TurnRightAndMove();
                }
                break;
        }



    }
}
