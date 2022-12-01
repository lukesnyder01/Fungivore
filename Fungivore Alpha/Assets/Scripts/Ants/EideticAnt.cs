using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EideticAnt : Ant
{
    public List<int> moveList = new List<int>();
    public bool randomizeList;

    public int totalLoops = 2;
    public int currentLoop = 0;

    public float halfExtentSizeMod = 0f;


    public override void InitializeParameters()
    {
        
        antPos = transform.position + new Vector3(0, verticalSpawnOffset, 0);

        player = GameObject.Find("Player");

        antMoveDistance = antSideLength;

        //halfExtents = new Vector3(antMoveDistance / 2 - 0.2f, antMoveDistance / 2 - 0.2f, antMoveDistance / 2 - 0.2f);
        halfExtents = new Vector3(antMoveDistance / 2 + halfExtentSizeMod, antMoveDistance / 2 + halfExtentSizeMod, antMoveDistance / 2 + halfExtentSizeMod);

        minPlayerDistance = antSideLength + 1;

        Random.InitState(transform.position.GetHashCode() % 100000);
        //totalSteps = Random.Range(minSteps, maxSteps);

        if (randomizeList)
        {
            totalSteps = Random.Range(minSteps, maxSteps);

            for (int i = 0; i < totalSteps + 1; i++)
            {
                moveList.Add(Random.Range(0, 5));
                //Debug.Log(moveList[i]);
            }
        }
        else
        {
            totalSteps = moveList.Count;
        }
    }


    public override void Grow()
    {
        if (currentStep < totalSteps)
        {
            currentStep++;

            playerDistance = (antPos - player.transform.position).sqrMagnitude;

            if (playerDistance > (minPlayerDistance * minPlayerDistance))
            {
                AddNewBlock();
            }
        }
        else if (currentLoop < totalLoops)
        {
            currentLoop++;
            currentStep = 0;

            StartCoroutine(IterateGrowth());
        }
        else
        {
            if (multipleRespawns && noMovesLeft == false)
            {
                Respawn();
                EndAnt();
            }

            EndAnt();
        }
    }


    public override void AddNewBlock()
    {

        int straight = antDir;
        int right = (antDir + 1) % 4;
        int left = (antDir + 3) % 4;

        Vector3 newPosition = transform.position;
        int newDirection = 0;

        if (moveList[currentStep - 1] == 0 && SpaceIsEmpty(directionArray[straight]))
        {
            newPosition = antPos + directionArray[straight] * antMoveDistance;
            newDirection = straight;
        }

        if (moveList[currentStep - 1] == 1 && SpaceIsEmpty(directionArray[right]))
        {
            newPosition = antPos + directionArray[right] * antMoveDistance;
            newDirection = right;
        }

        if (moveList[currentStep - 1] == 2 && SpaceIsEmpty(directionArray[left]))
        {
            newPosition = antPos + directionArray[left] * antMoveDistance;
            newDirection = left;
        }

        if (moveList[currentStep - 1] == 3 && SpaceIsEmpty(Vector3.up))
        {
            newPosition = antPos + Vector3.up * antMoveDistance;
            newDirection = straight;
        }

        if (moveList[currentStep - 1] == 4 && SpaceIsEmpty(-Vector3.up))
        {
            newPosition = antPos + -Vector3.up * antMoveDistance;
            newDirection = straight;
        }

        if (newPosition == transform.position)
        {
            currentStep++;
            return;
        }
        else 
        {
            var newBlock = ObjectPooler.current.GetPooledObject(1);
            newBlock.transform.position = antPos;
            newBlock.SetActive(true);
            newBlock.transform.SetParent(this.transform);

            antPos = newPosition;
            antDir = newDirection;
        }


    }

}
