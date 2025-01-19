using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EideticAnt : Ant
{
    public List<AntMove> moves;
    public int numberOfMoves = 20;
    public int movesPerTurn = 10;

    private int currentMoveIndex;


    // Start is called before the first frame update
    void Start()
    {
        // fill the moves list with random moves
    }


    public override void MoveNext()
    {
        // get next move on move list
        // check if move is valid
        // if so, make move and advance to the next move on the list
        // repeat for some number of moves
    }

}
