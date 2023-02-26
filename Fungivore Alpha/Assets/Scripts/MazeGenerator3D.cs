using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator3D : MonoBehaviour
{
    private float timeBetweenSteps = 0.001f;

    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public int cellSize;

    public GameObject cellPrefab;

    public int maxMazeSteps = 100;

    public Vector3Int startingCell = new Vector3Int(0, 0, 0);


    // Create the 3D array to store the state and directions for each cell
    int[,,] mazeGrid;


    // list of cells to 
    Queue<Vector3Int> cellsToCreate = new Queue<Vector3Int>();


    // Start is called before the first frame update
    void Start()
    {
        mazeGrid = new int[sizeX, sizeY, sizeZ];

        //overlap box each cell to see if it's obstructed, then mark it 0 for empty and 1 for obstructed
        InitializeCells();

        PickStartingCell();


        StartCoroutine(GenerateMaze());




        //spawn a block in each empty cell
        //StartCoroutine(FloodFillPrefabs());
    }


    void InitializeCells()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var cellPosX = transform.position.x + (x * cellSize);
                    var cellPosY = transform.position.y + (y * cellSize);
                    var cellPosZ = transform.position.z + (z * cellSize);

                    if (SpaceIsEmpty(cellPosX, cellPosY, cellPosZ))
                    {
                        mazeGrid[x, y, z] = 0;
                    }
                    else
                    {
                        mazeGrid[x, y, z] = 1;
                    }
                }
            }
        }
    }


    void PickStartingCell()
    {
        if (mazeGrid[startingCell.x, startingCell.y, startingCell.z] == 0)
        {
            cellsToCreate.Enqueue(startingCell);
        }
        else
        {
            Debug.Log("Starting cell is blocked");
        }

        //turn on the starting cell in the mazeGrid
        mazeGrid[startingCell.x, startingCell.y, startingCell.z] = 2;
    }


    IEnumerator GenerateMaze()
    {
        //add one maze cell per maze step
        for (int currentMazeStep = 1; currentMazeStep <= maxMazeSteps; currentMazeStep++)
        {

            if (cellsToCreate.Count > 0) 
            {
                //get the first cell in cellsToCheck
                var cellPos = cellsToCreate.Dequeue();

                //instantiate a new maze cell prefab
                Transform currentCell = NewPrefabAtPos(cellPos);

                var x = cellPos.x;
                var y = cellPos.y;
                var z = cellPos.z;



                //check the cell to the north
                if (z + 1 < sizeZ) //if we're not all the way to the edge
                {
                    if (mazeGrid[x, y, z + 1] == 0) //the cell is empty
                    {
                        mazeGrid[x, y, z + 1] = 2; //mark the cell as containing a maze cell

                        cellsToCreate.Enqueue(new Vector3Int(x, y, z + 1)); //add cell to the queue
                    }
                }


                //check the cell to the east
                if (x + 1 < sizeX) //if we're not all the way to the edge
                {
                    if (mazeGrid[x + 1, y, z] == 0) //the cell is empty
                    {
                        mazeGrid[x + 1, y, z] = 2; //mark the cell as containing a maze cell

                        cellsToCreate.Enqueue(new Vector3Int(x + 1, y, z)); //add cell the the queue
                    }
                }


                //check the cell above
                if (y + 1 < sizeY) //if we're not all the way to the edge
                {
                    if (mazeGrid[x, y + 1, z] == 0) //the cell is empty
                    {
                        mazeGrid[x, y + 1, z] = 2; //mark the cell as containing a maze cell

                        cellsToCreate.Enqueue(new Vector3Int(x, y + 1, z)); //add cell the the queue
                    }
                }





                /*
                // Check the cell to the north
                if (x > 0 && mazeGrid[x - 1, y, z] == 1)
                {
                    mazeGrid[x, y, z] |= maskNorth;
                    mazeGrid[x - 1, y, z] |= maskSouth;
                }
                */




                //look at the adjacent cells to see which ones are open
                //each open cell has a chance to get added to cellsToCheck and get turned on
                //if it's open and added to cellsToCheck, it should get marked as having an open wall facing the current cell
                //the current cell should also open its wall in the direction of the new cell


            }
            else 
            {
                Debug.Log("No cells left to check, ending maze");
                //end the maze
                break;

            }



            yield return new WaitForSeconds(timeBetweenSteps);
        }

        Debug.Log("Reached maximum maze size, ending maze");
        //end the maze

    }


    Transform NewPrefabAtPos(Vector3Int pos)
    {
        //get real world coordinates
        var cellPosX = transform.position.x + (pos.x * cellSize);
        var cellPosY = transform.position.y + (pos.y * cellSize);
        var cellPosZ = transform.position.z + (pos.z * cellSize);

        Transform newCell = Instantiate(cellPrefab, new Vector3(cellPosX, cellPosY, cellPosZ), Quaternion.identity).transform;

        return newCell;

        /*
        for (int i = 0; i < newCell.childCount; i++)
        {
            if (Random.value > 0.8)
            {
                newCell.GetChild(i).gameObject.SetActive(true);
            }
        }
        */
    }


    bool SpaceIsEmpty(float x, float y, float z)
    {
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(x, y, z), new Vector3(cellSize, cellSize, cellSize) / 2 , Quaternion.identity);
        if (hitColliders.Length < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    IEnumerator FloodFillPrefabs()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (mazeGrid[x, y, z] == 0) //spawn a block if the cell is marked 0 for empty
                    {
                        var cellPosX = transform.position.x + (x * cellSize);
                        var cellPosY = transform.position.y + (y * cellSize);
                        var cellPosZ = transform.position.z + (z * cellSize);

                        Transform newCell = Instantiate(cellPrefab, new Vector3(cellPosX, cellPosY, cellPosZ), Quaternion.identity).transform;

                        for (int i = 0; i < newCell.childCount; i++)
                        {
                            if (Random.value > 0.8)
                            {
                                newCell.GetChild(i).gameObject.SetActive(true);
                            }

                        }
                    }

                }

                yield return null;

            }
        }
    }



}
