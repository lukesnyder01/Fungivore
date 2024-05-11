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

            if (canOpenDeadEnds && RandomUtility.Range(antPos, 0, 100) < percentOpenDeadEnds)
            {
                GameObject randomPrefab = mazeBranchAll[RandomUtility.Range(antPos, 0, mazeBranchAll.Length)];
                GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

                room.transform.forward = directionArray[antDir];
                room.transform.SetParent(this.transform);

                terminalPosition.RemoveAt(currentTerminal);
                terminalDirection.RemoveAt(currentTerminal);
            }
            else
            {
                GameObject randomPrefab = mazeDeadEnd[RandomUtility.Range(antPos, 0, mazeDeadEnd.Length)];
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

        selectedMove = RandomUtility.Range(antPos, 0, availableMoves.Count);

        if (availableMoves[selectedMove] == "moveStraight")
        {
            GameObject randomPrefab = mazeStraight[RandomUtility.Range(antPos, 0, mazeStraight.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[forward] * antMoveDistance);
            terminalDirection[currentTerminal] = antDir;
        }

        if (availableMoves[selectedMove] == "turnRight")
        {
            GameObject randomPrefab = mazeTurnRight[RandomUtility.Range(antPos, 0, mazeTurnRight.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[right] * antMoveDistance);
            terminalDirection[currentTerminal] = right;
        }

        if (availableMoves[selectedMove] == "turnLeft")
        {
            GameObject randomPrefab = mazeTurnLeft[RandomUtility.Range(antPos, 0, mazeTurnLeft.Length)];
            GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

            room.transform.forward = directionArray[antDir];
            room.transform.SetParent(this.transform);

            terminalPosition[currentTerminal] = antPos + (directionArray[left] * antMoveDistance);
            terminalDirection[currentTerminal] = left;
        }

        if (availableMoves[selectedMove] == "branchRight")
        {
            GameObject randomPrefab = mazeBranchRight[RandomUtility.Range(antPos, 0, mazeBranchRight.Length)];
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
            GameObject randomPrefab = mazeBranchLeft[RandomUtility.Range(antPos, 0, mazeBranchLeft.Length)];
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
            GameObject randomPrefab = mazeBranchMid[RandomUtility.Range(antPos, 0, mazeBranchMid.Length)];
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
            GameObject randomPrefab = mazeBranchAll[RandomUtility.Range(antPos, 0, mazeBranchAll.Length)];
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

    public override bool SpaceIsEmpty(Vector3 direction)
    {
        // Calculate half dimensions of the box based on the side length of the ant
        Vector3 halfSize = new Vector3((antSideLength / 2) - 0.1f, (antSideLength / 2) - 0.1f, (antSideLength / 2) - 0.1f);

        // Perform a BoxCast in the specified direction
        return (!Physics.BoxCast(antPos, halfSize, direction, out hit, Quaternion.identity, antMoveDistance));
    }



    public bool AntIsHittingSomething()
    {
        var halfExtents = new Vector3(antSideLength / 2.1f, antSideLength / 2.1f, antSideLength / 2.1f);

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

            if (!AntIsHittingSomething() && RandomUtility.Range(antPos, 0, 100) < 100)
            {
                if (canOpenDeadEnds && RandomUtility.Range(antPos, 0, 100) < percentOpenDeadEnds)
                {
                    GameObject randomPrefab = mazeBranchAll[RandomUtility.Range(antPos, 0, mazeBranchAll.Length)];
                    GameObject room = Instantiate(randomPrefab, antPos, Quaternion.identity);

                    room.transform.forward = directionArray[antDir];
                    room.transform.SetParent(this.transform);
                }
                else
                {
                    GameObject randomPrefab = mazeDeadEnd[RandomUtility.Range(antPos, 0, mazeDeadEnd.Length)];
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
