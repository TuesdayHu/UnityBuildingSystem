using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public class BlockListInfo
    {
        public List<Vector3Int> gridPointList;
        BlockBase blockElement;

        public BlockListInfo(List<Vector3Int> gridPositionList, BlockBase block)
        {
            gridPointList = gridPositionList;
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
    public GridPointInfo[,,] gridArray;
    [SerializeField]public List<BlockListInfo> blockList;
    //List and array for storing info

    public BlockBase centerBlockBase;
    private Vector3 originPoint;
    //param for center

    public void InitializeGrid()
    {
        gridArray = new GridPointInfo[gridSize, gridSize, gridSize];
        blockList = new List<BlockListInfo>();
    }

    public void AddBlock(Vector3Int placeCenterPosition, List<Vector3Int> blockGridList, Vector3 placeEulerRotation, BlockBase placeBlock)
    {
        bool intCheck = true;
        List<Vector3Int> placeGridIndexList = new List<Vector3Int>();

        foreach (Vector3 iPosition in blockGridList)
        {
            if (((iPosition.x - (int)iPosition.x) != 0) ||
            ((iPosition.y - (int)iPosition.y) != 0) ||
            ((iPosition.z - (int)iPosition.z) != 0))
            {
                intCheck = false;
            }
            else
            {
                Vector3 newPlacePositionInGrid = Quaternion.Euler(placeEulerRotation) * iPosition;
                Debug.LogWarning("111" + newPlacePositionInGrid);
                placeGridIndexList.Add(new Vector3Int((int)newPlacePositionInGrid.x + placeCenterPosition.x, (int)newPlacePositionInGrid.y + placeCenterPosition.y, (int)newPlacePositionInGrid.z + placeCenterPosition.z));
            }
            //Calculate the grid point list occupying for this object
        }

        if(intCheck)
        {
            int currentblockListIndex;
            BlockListInfo iBlockInfo;
            GridPointInfo iPointInfo;
            //param used for current block

            Debug.LogWarning("Is int vector3");

            //Write the Data into gridArray and blockList
            iBlockInfo = new BlockListInfo(placeGridIndexList, placeBlock);
            currentblockListIndex = blockList.Count;
            blockList.Add(iBlockInfo);
            //Construct info for blocklist

            Debug.LogError("Current block list index is " + currentblockListIndex);

            iPointInfo = new GridPointInfo(placeBlock, currentblockListIndex);
            iPointInfo.occupied = true;

            foreach(Vector3Int placeGridIndex in placeGridIndexList)
            {
                gridArray[placeGridIndex.x, placeGridIndex.y, placeGridIndex.z] = iPointInfo;
            }
            //Adding info to gridpoints 

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
