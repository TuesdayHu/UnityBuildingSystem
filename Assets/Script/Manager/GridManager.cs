using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.Serializable]
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

    [System.Serializable]
    public class GridPointInfo
    {
        public BlockBase blockInPlace;
        public bool occupied = false;
        public int blockListIndex;

        public GridPointInfo(BlockBase block, int listIndex)
        {
            blockInPlace = block;
            blockListIndex = listIndex;
            occupied = false;
        }
    }

    public float gridUnit { get; } = 1f;
    public int gridSize = 251;
    //param for grid

    public GridPointInfo[,,] gridArray { get; private set; }
    public List<BlockListInfo> blockList { get; private set; }
    //List and array for storing info
    public Vector3 gridOriginPosition { get; private set; } = Vector3.zero;
    public Quaternion gridOriginRotation { get; private set; } = Quaternion.identity;
    //param for center transform

    public void InitializeGridInfo()
    {
        gridArray = new GridPointInfo[gridSize, gridSize, gridSize];
        blockList = new List<BlockListInfo>();
    }

    public void SetGridTransform(Transform inputTransform)
    {
        gridOriginPosition = inputTransform.position;
        gridOriginRotation = inputTransform.rotation;
    }

    public void AddBlockInfo(List<Vector3Int> blockGridList, Vector3Int placeCenterPosition, Vector3 placeEulerRotation, BlockBase placeBlock)
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

            //Write the Data into gridArray and blockList
            iBlockInfo = new BlockListInfo(placeGridIndexList, placeBlock);
            currentblockListIndex = blockList.Count;
            blockList.Add(iBlockInfo);
            //Construct info for blocklist

            Debug.LogWarning("Current block list index is " + currentblockListIndex);

            iPointInfo = new GridPointInfo(placeBlock, currentblockListIndex);
            iPointInfo.occupied = true;

            foreach(Vector3Int placeGridIndex in placeGridIndexList)
            {
                gridArray[placeGridIndex.x, placeGridIndex.y, placeGridIndex.z] = iPointInfo;
            }
            //Adding info to gridpoints 
        }
    }

    public void DeleteBlockInfo()
    {
        
    }

    public bool CheckGirdOccupied(BlockBase checkBlock, Vector3 blockPositionInGrid, Vector3 blockRotationInGrid)
    {
        List<Vector3Int> checkList = checkBlock.blockGridOccpiedList;
        bool isOccupied = false;
        Vector3Int checkGridIndex = Vector3Int.zero;
        Debug.LogWarning("checkList count " + checkList.Count);
        foreach (Vector3Int checkGrid in checkList)
        {
            checkGridIndex = Vector3Int.RoundToInt(Quaternion.Euler(blockRotationInGrid) * checkGrid + blockPositionInGrid);
            //wrong Calculation
            Debug.LogWarning(checkGridIndex);
            if (gridArray[checkGridIndex.x, checkGridIndex.y, checkGridIndex.z].occupied) { isOccupied = true; break; }
        }
        return isOccupied;
    }


    //public Vector3 WorldPointGrabToNearGridWorldPoint(Vector3 oldPosition)
    //{
    //    Vector3 toGridPosition = WorldPositionToGrid(oldPosition);
    //    int newx = (int)(Mathf.RoundToInt(toGridPosition.x / gridUnit) * gridUnit);
    //    int newy = (int)(Mathf.RoundToInt(toGridPosition.y / gridUnit) * gridUnit);
    //    int newz = (int)(Mathf.RoundToInt(toGridPosition.z / gridUnit) * gridUnit);
    //    return GridPositionToWorld( new Vector3(newx, newy, newz));
    //}
    ////Move the grid back to origin according to the center object, then calculate the grid point and move back
    
    public Vector3 WorldPositionToGrid (Vector3 worldPosition)
    {
        return worldPosition - gridOriginPosition;
    }

    public Vector3 GridPositionToWorld (Vector3 gridPosition)
    {
        return gridPosition + gridOriginPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeGridInfo();

        //GameObject centerBlockBaseObject = centerBlockBase.transform.parent.gameObject;
        //originPoint = centerBlockBaseObject.transform.position;
        gridOriginPosition = this.transform.position;
        gridOriginRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = centerBlockBase.transform.parent.position;
        //update the grid center later
    }
}