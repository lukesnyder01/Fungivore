using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAnt : Ant
{

    [Header("Building Settings")]

    public bool moveFlat;

    public bool moveDown;


    public override void AddNewBlock()
    {
        int selectedMove = 0;
        availablePositions.Clear();
        availableRotations.Clear();

        int forward = antDir;
        int right = (antDir + 1) % 4;
        int left = (antDir + 3) % 4;
        int back = (antDir + 2) % 4;


        if (moveDown)
        {
            if (SpaceIsEmpty(-Vector3.up))
            {
                availablePositions.Add(antPos + -Vector3.up * antMoveDistance);
                availableRotations.Add(forward);
            }
        }


        if (moveFlat)
        {
            if (SpaceIsEmpty(directionArray[forward]))
            {
                availablePositions.Add(antPos + directionArray[forward] * antMoveDistance);
                availableRotations.Add(forward);
            }

            if (SpaceIsEmpty(directionArray[back]))
            {
                availablePositions.Add(antPos + directionArray[back] * antMoveDistance);
                availableRotations.Add(back);
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
        }


        if (availablePositions.Count <= 3)
        {
            if (SpaceIsEmpty(Vector3.up))
            {
                availablePositions.Add(antPos + Vector3.up * antMoveDistance);
                availableRotations.Add(forward);
            }
        }


        if (availablePositions.Count != 0)
        {
            selectedMove = RandomUtility.Range(antPos, 0, availablePositions.Count);

            GameObject randomPrefab = spawnPrefabs[RandomUtility.Range(antPos, 0, spawnPrefabs.Length)];
            Vector3 randomRotation = directionArray[RandomUtility.Range(antPos, 0, directionArray.Length)];

            GameObject newBlock = Instantiate(randomPrefab, antPos, Quaternion.identity);
            newBlock.transform.forward = randomRotation;

            newBlock.transform.SetParent(this.transform);

            antPos = availablePositions[selectedMove];
            antDir = availableRotations[selectedMove];
        }
        else
        {
            EndAnt();
        }
    }

    public override bool SpaceIsEmpty(Vector3 direction)
    {
        // Calculate half dimensions of the box based on the side length of the ant
        Vector3 halfSize = new Vector3((antSideLength / 2) - 0.1f, (antSideLength / 2) - 0.1f, (antSideLength / 2) - 0.1f);

        // Perform a BoxCast in the specified direction
        return (!Physics.BoxCast(antPos, halfSize, direction, out hit, Quaternion.identity, antMoveDistance));
    }


}
