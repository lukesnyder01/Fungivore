using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibAnt : Ant
{
    [Header("Rib Specific Settings")]
    public bool spawnClimbers;
    public GameObject climber;

    public bool spawnDroppers;
    public GameObject dropper;

    public float sphereCastRadius = 0.6f;
    public float sphereCastDistance = 1.2f;

    public override bool SpaceIsEmpty(Vector3 direction)
    {
        return (!Physics.SphereCast(antPos, sphereCastRadius, direction, out hit, sphereCastDistance));
    }


    public override void AddNewBlock()
    {
        int selectedMove = 0;
        availablePositions.Clear();
        availableRotations.Clear();

        int forward = antDir;
        int right = (antDir + 1) % 4;
        int left = (antDir + 3) % 4;
        int back = (antDir + 2) % 4;





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

        if (availablePositions.Count <= 2)
        {
            if (SpaceIsEmpty(Vector3.up))
            {
                availablePositions.Add(antPos + Vector3.up * antMoveDistance);
                availableRotations.Add(forward);
            }
        }

        if (availablePositions.Count <= 2)
        {
            if (SpaceIsEmpty(-Vector3.up))
            {
                availablePositions.Add(antPos + -Vector3.up * antMoveDistance);
                availableRotations.Add(forward);
            }
        }


        if (availablePositions.Count > 0)
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            selectedMove = Random.Range(0, availablePositions.Count);


            var newBlock = ObjectPooler.current.GetPooledObject(1);
            newBlock.transform.position = antPos;
            newBlock.SetActive(true);


            /*
            GameObject randomPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            Vector3 randomRotation = directionArray[Random.Range(0, directionArray.Length)];
            GameObject newBlock = Instantiate(randomPrefab, antPos, Quaternion.identity);
            newBlock.transform.forward = randomRotation;
            */

            newBlock.transform.SetParent(this.transform);

            antPos = availablePositions[selectedMove];
            antDir = availableRotations[selectedMove];
        }
        else
        {
            noMovesLeft = true;
            return;
        }

        if (spawnClimbers)
        {
            AddClimber();
        }


        if (spawnDroppers)
        {
            AddDropper();
        }
    }


    public void AddClimber() 
    {
        Random.InitState(antPos.GetHashCode() % 100000);

        if (Random.Range(0, 1000) <= 10)
        {
            if (SpaceIsEmpty(Vector3.up))
            {
                var newClimber = Instantiate(climber, (antPos + Vector3.up * antMoveDistance), Quaternion.identity);
                newClimber.transform.parent = gameObject.transform;
            }
        }
    }


    public void AddDropper()
    {
        Random.InitState(antPos.GetHashCode() % 100000);

        if (Random.Range(0, 1000) <= 50)
        {
            if (SpaceIsEmpty(-Vector3.up))
            {
                var newDropper = Instantiate(dropper, (antPos + -Vector3.up * antMoveDistance), Quaternion.identity);
                newDropper.transform.parent = gameObject.transform;
            }
        }
    }

}
