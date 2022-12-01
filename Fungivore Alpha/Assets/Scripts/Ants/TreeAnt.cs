using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAnt : Ant
{
    public GameObject ribSpawner;

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
        }


        if (availablePositions.Count != 0)
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            selectedMove = Random.Range(0, availablePositions.Count);

            GameObject randomPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            Vector3 randomRotation = directionArray[Random.Range(0, directionArray.Length)];


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

        AddRibSpawner();
    }

    public void AddRibSpawner()
    {
        Random.InitState(antPos.GetHashCode() % 100000);

        if (Random.Range(0, 1000) <= 400)
        {
            if (SpaceIsEmpty(Vector3.forward))
            {
                Instantiate(ribSpawner, (antPos + Vector3.forward * antMoveDistance), Quaternion.identity);
            }
        }
    }


}
