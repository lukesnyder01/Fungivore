using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimberAnt : Ant
{

    public override void AddNewBlock()
    {
        int selectedMove = 0;
        availablePositions.Clear();
        availableRotations.Clear();

        int straight = antDir;
        int right = (antDir + 1) % 4;
        int left = (antDir + 3) % 4;


        if (SpaceIsEmpty(Vector3.up))
        {
            availablePositions.Add(antPos + Vector3.up * antMoveDistance);
            availableRotations.Add(straight);

            availablePositions.Add(antPos + Vector3.up * antMoveDistance);
            availableRotations.Add(straight);

            availablePositions.Add(antPos + Vector3.up * antMoveDistance);
            availableRotations.Add(straight);
        }

        if (SpaceIsEmpty(directionArray[straight]))
        {
            availablePositions.Add(antPos + directionArray[straight] * antMoveDistance);
            availableRotations.Add(straight);
        }

        if (SpaceIsEmpty(directionArray[right]))
        {
            availablePositions.Add(antPos + directionArray[right] * antMoveDistance);
            availableRotations.Add(right);
        }

        if (SpaceIsEmpty(directionArray[left]))
        {
            availablePositions.Add(antPos + directionArray[left] * antMoveDistance);
            availableRotations.Add(left);
        }


        if (availablePositions.Count != 0)
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            selectedMove = Random.Range(0, availablePositions.Count);

            var newBlock = ObjectPooler.current.GetPooledObject(1);
            newBlock.transform.position = antPos;
            newBlock.SetActive(true);
            newBlock.transform.SetParent(this.transform);

            antPos = availablePositions[selectedMove];
            antDir = availableRotations[selectedMove];
        }
        else
        {
            EndAnt();
        }
    }

}
