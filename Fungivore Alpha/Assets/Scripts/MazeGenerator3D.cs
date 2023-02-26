using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator3D : MonoBehaviour
{
    private float timeBetweenSteps = 0.0f;

    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public int cellSize;

    public float chanceToMoveInOpenDirection = 0.5f;
    public float chanceToCloseExterior = 0.5f;

    public GameObject cellPrefab;

    public int maxMazeSteps = 100;

    public Vector3Int startingCell = new Vector3Int(0, 0, 0);


    //class to store the state and bitmask of each cell in the maze
    public class MazeCell
    {
        public int cellState; // 0 for open, 1 for blocked, 2 for active with a maze cell

        public int bitmask; // 6 digit bitmask to define the open sides of each active maze cell
    }


    // Create the 3D array to store the state and directions for each cell
    MazeCell[,,] mazeGrid;


    // list of cells to 
    List<Vector3Int> cellsToCreate = new List<Vector3Int>();


    // Define bit masks for each direction
    int maskNorth = 1 << 0;
    int maskEast = 1 << 1;
    int maskSouth = 1 << 2;
    int maskWest = 1 << 3;
    int maskUp = 1 << 4;
    int maskDown = 1 << 5;




    // Start is called before the first frame update
    void Start()
    {



        //overlap box each cell to see if it's obstructed, then mark it 0 for empty and 1 for obstructed
        InitializeCells();

        PickStartingCell();


        StartCoroutine(GenerateMaze());




        //spawn a block in each empty cell
        //StartCoroutine(FloodFillPrefabs());
    }


    void InitializeCells()
    {
        mazeGrid = new MazeCell[sizeX, sizeY, sizeZ]; //initialize the whole grid

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    mazeGrid[x, y, z] = new MazeCell(); //initialize each cell on the grid

                    var cellPosX = transform.position.x + (x * cellSize);
                    var cellPosY = transform.position.y + (y * cellSize);
                    var cellPosZ = transform.position.z + (z * cellSize);

                    if (SpaceIsEmpty(cellPosX, cellPosY, cellPosZ))
                    {
                       mazeGrid[x, y, z].cellState = 0;
                    }
                    else
                    {
                       mazeGrid[x, y, z].cellState = 1;
                    }
                }
            }
        }
    }


    void PickStartingCell()
    {
        if (mazeGrid[startingCell.x, startingCell.y, startingCell.z].cellState == 0)
        {
            cellsToCreate.Add(startingCell);
        }
        else
        {
            Debug.Log("Starting cell is blocked");
        }

        //turn on the starting cell in the mazeGrid
        mazeGrid[startingCell.x, startingCell.y, startingCell.z].cellState = 2;
    }


    IEnumerator GenerateMaze()
    {
        //add one maze cell per maze step
        for (int currentMazeStep = 1; currentMazeStep <= maxMazeSteps; currentMazeStep++)
        {

            if (cellsToCreate.Count > 0) 
            {
                //get a random cell to check
                var selectedCell = Random.Range(0, cellsToCreate.Count);
                var cellPos = cellsToCreate[selectedCell];
                cellsToCreate.RemoveAt(selectedCell);

                //instantiate a new maze cell prefab, we'll set the walls later
                Transform currentCellPrefab = NewPrefabAtPos(cellPos);


                var x = cellPos.x;
                var y = cellPos.y;
                var z = cellPos.z;


                if (z < sizeZ - 1) //check to the north
                {
                    CheckDirection(new Vector3Int(x, y, z + 1), maskNorth, maskSouth, cellPos);
                }


                if (x < sizeX - 1) //check to the east
                {
                    CheckDirection(new Vector3Int(x + 1, y, z), maskEast, maskWest, cellPos);
                }


                if (z > 1) //check to the south
                {
                    CheckDirection(new Vector3Int(x, y, z - 1), maskSouth, maskNorth, cellPos);
                }


                if (x > 1) //check to the west
                {
                    CheckDirection(new Vector3Int(x - 1, y, z), maskWest, maskEast, cellPos);
                }


                if (y < sizeY - 1) //check above
                {
                    CheckDirection(new Vector3Int(x, y + 1, z), maskUp, maskDown, cellPos);
                }


                if (y > 1) //check below
                {
                    CheckDirection(new Vector3Int(x, y - 1, z), maskDown, maskUp, cellPos);
                }


                //sets the walls from the bitmask
                for (int i = 0; i < 6; i++)
                {
                    if ((mazeGrid[x, y, z].bitmask & (1 << i)) != 0)
                    {
                        currentCellPrefab.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        currentCellPrefab.GetChild(i).gameObject.SetActive(false);
                    }
                }

            }
            else 
            {
                Debug.Log("No cells left to check, ending maze");
                //end the maze
                break;
            }



            //yield return new WaitForSeconds(timeBetweenSteps);
            yield return null;

        }

        //end the maze

    }

    void CheckDirection(Vector3Int cellCoords, int myMask, int adjacentMask, Vector3Int currentCellPos)
    {
        var cellToCheck = mazeGrid[cellCoords.x, cellCoords.y, cellCoords.z];


        if (cellToCheck.cellState == 0) //if the cell is empty
        {
            // chance to add cell to open direction
            if (Random.value < chanceToMoveInOpenDirection)
            {
                cellToCheck.cellState = 2; //mark the cell as containing a maze cell
                cellsToCreate.Add(cellCoords); //add cell to the queue
                mazeGrid[currentCellPos.x, currentCellPos.y, currentCellPos.z].bitmask |= myMask;
                cellToCheck.bitmask |= adjacentMask;
            }
            else if (Random.value < chanceToCloseExterior)
            {
                mazeGrid[currentCellPos.x, currentCellPos.y, currentCellPos.z].bitmask &= ~myMask;
            }


        }
        else if (cellToCheck.cellState == 1) //if the cell is blocked
        {
            if (Random.value < chanceToCloseExterior)
            {
                mazeGrid[currentCellPos.x, currentCellPos.y, currentCellPos.z].bitmask &= ~myMask;
            }
        }
        else if (cellToCheck.cellState == 2) //if the cell is occupied by a maze cell
        {

        }
    }




    Transform NewPrefabAtPos(Vector3Int pos)
    {
        //get real world coordinates
        var cellPosX = transform.position.x + (pos.x * cellSize);
        var cellPosY = transform.position.y + (pos.y * cellSize);
        var cellPosZ = transform.position.z + (pos.z * cellSize);

        Transform newCell = Instantiate(cellPrefab, new Vector3(cellPosX, cellPosY, cellPosZ), Quaternion.identity).transform;

        return newCell;
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
                    if (mazeGrid[x, y, z].cellState == 0) //spawn a block if the cell is marked 0 for empty
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
