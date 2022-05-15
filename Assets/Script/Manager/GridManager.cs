using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public class BlockListInfo
    {
        public int gridX;// need to be List<Vector3> later
        public int gridY;
        public int gridZ;
        BlockBase blockElement;

        public BlockListInfo(Vector3Int gridPosition, BlockBase block)
        {
            gridX = gridPosition.x;
            gridY = gridPosition.y;
            gridZ = gridPosition.z;
            blockElement = block;
        }
    }


    public class GridPointInfo
    {
        public BlockBase blockInPlace;
        public bool occupied = false;
        public int blockListIndex;

        public GridPointInfo(BlockBase block, int listIndex)
        {
            blockInPlace = block;
            blockListIndex = listIndex;
        }
    }

    [HideInInspector] public float gridUnit { get; } = 1f;
    public int gridSize = 251;
    //param for grid
    //[HideInInspector] 
    public GridPointInfo[,,] gridArray;
    //[HideInInspector] 
    public List<BlockListInfo> blockList;
    //List and array for storing info

    public BlockBase centerBlockBase;
    private Vector3 originPoint;
    //param for center

    public void InitializeGrid()
    {
        gridArray = new GridPointInfo[gridSize, gridSize, gridSize];
        blockList = new List<BlockListInfo>();
    }

    public void AddBlock(Vector3 placePosition, BlockBase placeBlock)
    {
        if (((placePosition.x - (int)placePosition.x) == 0) && 
            ((placePosition.y - (int)placePosition.y) == 0) && 
            ((placePosition.z - (int)placePosition.z) == 0))
        {
            Vector3Int placeIndex;
            int currentblockListIndex;
            BlockListInfo iBlockInfo;
            GridPointInfo iPointInfo;
            //param used for current block

            Debug.LogWarning("Is int vector3");
            placeIndex = new Vector3Int((int)placePosition.x, (int)placePosition.y, (int)placePosition.z);

            //Write the Data into gridArray and blockList
            iBlockInfo = new BlockListInfo(placeIndex, placeBlock);
            currentblockListIndex = blockList.Count;
            blockList.Add(iBlockInfo);

            Debug.LogError("Current block list index is " + currentblockListIndex);

            iPointInfo = new GridPointInfo(placeBlock, currentblockListIndex);
            iPointInfo.occupied = true;

            gridArray[placeIndex.x, placeIndex.y, placeIndex.z] = iPointInfo;
            //Only adding info to one gridpoint now 

        }
    }

    public void DeleteBlock()
    {
        
    }


    public Vector3 GrabToNearGridPoint(Vector3 oldPosition)
    {
        Vector3 toGridPosition = WorldPositionToGrid(oldPosition);
        float newx = Mathf.RoundToInt(toGridPosition.x / gridUnit) * gridUnit + originPoint.x;
        float newy = Mathf.RoundToInt(toGridPosition.y / gridUnit) * gridUnit + originPoint.y;
        float newz = Mathf.RoundToInt(toGridPosition.z / gridUnit) * gridUnit + originPoint.z;

        return new Vector3(newx, newy, newz);
    }
    //Move the grid back to origin according to the center object, then calculate the grid point and move back
    
    public Vector3 WorldPositionToGrid (Vector3 worldPosition)
    {
        return worldPosition - originPoint;
    }

    public Vector3 GridPositionToWorld (Vector3 gridPosition)
    {
        return gridPosition + originPoint;
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();

        GameObject centerBlockBaseObject = centerBlockBase.transform.parent.gameObject;
        originPoint = centerBlockBaseObject.transform.position;

        this.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = centerBlockBase.transform.parent.position;
    }
}
