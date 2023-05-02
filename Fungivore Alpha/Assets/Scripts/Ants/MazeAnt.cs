using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAnt : Ant
{
    
    private List<Vector3> terminalPosition = new List<Vector3>();
    private List<int> terminalDirection = new List<int>();

    private int currentTerminal = 0;
    private int mazeSize;

    [Header("Maze Specific Settings")]

    public bool canOpenDeadEnds = true;
    public int percentOpenDeadEnds = 50;

    public int maxTerminals = 10;

    private List<string> availableMoves = new List<string>();

    [Header("Maze References")]

    public GameObject[] mazeBranchAll;
    public GameObject[] mazeBranchLeft;
    public GameObject[] mazeBranchMid;
    public GameObject[] mazeBranchRight;
    public GameObject[] mazeDeadEnd;
    public GameObject[] mazeStraight;
    public GameObject[] mazeTurnLeft;
    public GameObject[] mazeTurnRight;


    public override void Awake()
    {
        InitializeParameters();

        terminalDirection.Add(antDir);
        terminalPosition.Add(antPos);
    }


    public override void AddNewBlock()
    {
        availableMoves.Clear();

        int selectedMove = 0;

        if (currentStep == totalSteps)
        {
            Respawn();

            CloseMazeEnds();
        }


        if (currentTerminal > terminalPosition.Count - 1)
        {
            currentTerminal = 0;
        }
        

        antPos = terminalPosition[currentTerminal];
        antDir = terminalDirection[currentTerminal];


        if (AntIsHittingSomething())
        {
            terminalPosition.RemoveAt(currentTerminal);
            terminalDirection.RemoveAt(currentTerminal);

            if (terminalPosition.Count == 0)
            {
                EndAnt();
            }

            return;
        }


        int forward = antDir;
        int right = (antDir + 1) % 4;
        int left = (antDir + 3) % 4;


        bool canMoveForward = SpaceIsEmpty(directionArray[forward]);
        bool canMoveRight = SpaceIsEmpty(directionArray[right]);
        bool canMoveLeft = SpaceIsEmpty(directionArray[left]);


        bool canBranchRight = (canMoveForward && canMoveRight);
        bool canBranchLeft = (canMoveForward && canMoveLeft);
        bool canBranchMid = (canMoveLeft && canMoveRight);
        bool canBranchAll = (canMoveForward && canMoveRight && canMoveLeft);


        if (canMoveForward) { availableMoves.Add("moveStraight"); }
        if (canMoveRight) { availableMoves.Add("turnRight"); }
        if (canMoveLeft) { availableMoves.Add("turnLeft"); }


        if (terminalPosition.Count < maxTerminals)
        {
            if (canBranchRight) { availableMoves.Add("branchRight"); }
            if (canBranchLeft) { availableMoves.Add("branchLeft"); }
            if (canBranchMid) { availableMoves.Add("branchMid"); }
            if (canBranchAll) { availableMoves.Add("branchAll"); }
        }


        if (availableMoves.Count == 0)
        {

            if (canOpenDeadEnds && Random.Range(0, 100) < percentOpenDeadEnds)
            {
                Random.InitState(antPos.GetHashCode() % 100000);
                GameObject randomPrefab = mazeBranchAll[Random.Range(0, mazeBranchAll.Length)];
                GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

                room.transform.forward = directionArray[antDir];
                room.transform.SetParent(this.transform);

                terminalPosition.RemoveAt(currentTerminal);
                terminalDirection.RemoveAt(currentTerminal);
            }
            else
            {
                Random.InitState(antPos.GetHashCode() % 100000);
                GameObject randomPrefab = mazeDeadEnd[Random.Range(0, mazeDeadEnd.Length)];
                GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

                room.transform.forward = directionArray[antDir];
                room.transform.SetParent(this.transform);

                terminalPosition.RemoveAt(currentTerminal);
                terminalDirection.RemoveAt(currentTerminal);
            }

            if (terminalPosition.Count == 0)
            {
                EndAnt();
            }

            return;
        }


        Random.InitState(antPos.GetHashCode() % 100000);
        selectedMove = Random.Range(0, availableMoves.Count);


        if (availableMoves[selectedMove] == "moveStraight")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeStraight[Random.Range(0, mazeStraight.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[forward] * antMoveDistance);
            terminalDirection[currentTerminal] = antDir;
        }

        if (availableMoves[selectedMove] == "turnRight")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeTurnRight[Random.Range(0, mazeTurnRight.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[right] * antMoveDistance);
            terminalDirection[currentTerminal] = right;
        }

        if (availableMoves[selectedMove] == "turnLeft")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeTurnLeft[Random.Range(0, mazeTurnLeft.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[left] * antMoveDistance);
            terminalDirection[currentTerminal] = left;
        }

        if (availableMoves[selectedMove] == "branchRight")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeBranchRight[Random.Range(0, mazeBranchRight.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[right] * antMoveDistance);
            terminalDirection[currentTerminal] = right;

            terminalPosition.Add(antPos + (directionArray[forward] * antMoveDistance));
            terminalDirection.Add(antDir);
        }

        if (availableMoves[selectedMove] == "branchLeft")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeBranchLeft[Random.Range(0, mazeBranchLeft.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[left] * antMoveDistance);
            terminalDirection[currentTerminal] = left;

            terminalPosition.Add(antPos + (directionArray[forward] * antMoveDistance));
            terminalDirection.Add(antDir);
        }

        if (availableMoves[selectedMove] == "branchMid")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeBranchMid[Random.Range(0, mazeBranchMid.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[left] * antMoveDistance);
            terminalDirection[currentTerminal] = left;

            terminalPosition.Add(antPos + (directionArray[right] * antMoveDistance));
            terminalDirection.Add(right);
        }

        if (availableMoves[selectedMove] == "branchAll")
        {
            Random.InitState(antPos.GetHashCode() % 100000);
            GameObject randomPrefab = mazeBranchAll[Random.Range(0, mazeBranchAll.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[forward] * antMoveDistance);
            terminalDirection[currentTerminal] = antDir;

            terminalPosition.Add(antPos + (directionArray[right] * antMoveDistance));
            terminalDirection.Add(right);

            terminalPosition.Add(antPos + (directionArray[left] * antMoveDistance));
            terminalDirection.Add(left);
        }

        currentTerminal++;

    }


    public bool AntIsHittingSomething()
    {
        var halfExtents = new Vector3(antSideLength / 2, antSideLength / 2, antSideLength / 2);

        Collider[] hitColliders = Physics.OverlapBox(antPos, halfExtents, Quaternion.identity);

        if (hitColliders.Length != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void CloseMazeEnds()
    {
        for (int i = 0; i < terminalPosition.Count; i++)
        {
            antPos = terminalPosition[i];
            antDir = terminalDirection[i];

            Random.InitState(antPos.GetHashCode() % 100000);
            if (!AntIsHittingSomething() && Random.Range(0, 100) < 100)
            {
                if (canOpenDeadEnds && Random.Range(0, 100) < percentOpenDeadEnds)
                {
                    Random.InitState(antPos.GetHashCode() % 100000);
                    GameObject randomPrefab = mazeBranchAll[Random.Range(0, mazeBranchAll.Length)];
                    GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

                    room.transform.forward = directionArray[antDir];
                    room.transform.SetParent(this.transform);
                }
                else
                {
                    Random.InitState(antPos.GetHashCode() % 100000);
                    GameObject randomPrefab = mazeDeadEnd[Random.Range(0, mazeDeadEnd.Length)];
                    GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

                    room.transform.forward = directionArray[antDir];
                    room.transform.SetParent(this.transform);
                }
            }
        }
    }


    public override void Respawn()
    {
        if (respawnCount > 0)
        {
            for (int i = 0; i < terminalPosition.Count; i++)
            {
                antPos = terminalPosition[i];
                antDir = terminalDirection[i];

                if (!AntIsHittingSomething() && respawnCount > 0)
                {
                    respawnCount--;

                    GameObject newMaze = Instantiate(respawnPrefab, antPos, Quaternion.identity);
                    newMaze.transform.forward = directionArray[antDir];

                    newMaze.GetComponent<MazeRespawner>().prefabDir = antDir;
                    newMaze.GetComponent<MazeRespawner>().prefabRespawnCount = respawnCount;

                    newMaze.transform.parent = this.transform.root;

                    terminalPosition.RemoveAt(0);
                    terminalDirection.RemoveAt(0);
                }
            }
        }
    }


}
