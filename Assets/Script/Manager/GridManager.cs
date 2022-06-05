using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public class BlockListInfo
    {
        public List<Vector3Int> gridIndexList;
        public Vector3Int gridPosition;
        public Quaternion gridRotation;
        public BlockBase blockElement;


        public BlockListInfo(List<Vector3Int> inputGridIndexList, Vector3Int inputGridPosition, Quaternion inputGridRotation, BlockBase block)
        {
            gridIndexList = inputGridIndexList;
            gridPosition = inputGridPosition;
            gridRotation = inputGridRotation;
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

    public float gridUnit { get; } = 0.3f;
    [SerializeField]private int gridSize = 251;
    private Vector3Int gridOffsetVector;
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
        int gridOffset = Mathf.RoundToInt((gridSize - 1) / 2);
        gridOffsetVector = new Vector3Int(gridOffset, gridOffset, gridOffset);
    }

    public void SetGridTransform(Transform inputTransform)
    {
        gridOriginPosition = inputTransform.position;
        gridOriginRotation = inputTransform.rotation;
    }

    public void AddBlockInfo(BlockBase placeBlock,  Vector3Int placeCenterPosition, Quaternion placeRotation)
    {
        Debug.LogError("Block " + placeBlock.name + "  Center " + placeCenterPosition + " Rotation " + placeRotation.eulerAngles);

        List<Vector3Int> placeGridIndexList = new List<Vector3Int>();
        List<Vector3Int> blockGridList = placeBlock.blockGridOccupiedList;

        foreach (Vector3 iPosition in blockGridList)
        {
            Vector3 newPlacePositionInGrid = placeRotation * iPosition;
            placeGridIndexList.Add(Vector3Int.RoundToInt(newPlacePositionInGrid + placeCenterPosition));
            //Calculate the grid point list occupying for this object
        }

        int currentblockListIndex;
        BlockListInfo iBlockInfo;
        GridPointInfo iPointInfo;
        //param used for current block

        //Write the Data into gridArray and blockList
        iBlockInfo = new BlockListInfo(placeGridIndexList, placeCenterPosition, placeRotation, placeBlock);
        currentblockListIndex = blockList.Count;
        blockList.Add(iBlockInfo);
        //Construct info for blocklist

        iPointInfo = new GridPointInfo(placeBlock, currentblockListIndex);
        iPointInfo.occupied = true;

        foreach(Vector3Int placeGridIndex in placeGridIndexList)
        {
            gridArray[placeGridIndex.x, placeGridIndex.y, placeGridIndex.z] = iPointInfo;
            Debug.Log("Write into Index" + placeGridIndex);
        }
        Debug.Log("Current block list index is " + currentblockListIndex);
        //Adding info to gridpoints 
    }

    public void DeleteBlockInfo()
    {
        
    }

    public void PrintGridArray()
    {

    }

    public void PrintBlockList()
    {

    }

    public bool CheckGirdOccupied(BlockBase checkBlock, Vector3 blockPositionInGrid, Quaternion blockRotationInGrid)
    {
        List<Vector3Int> checkList = checkBlock.blockGridOccupiedList;
        bool isOccupied = false;
        Vector3Int checkGridIndex;

        foreach (Vector3Int checkGrid in checkList)
        {
            checkGridIndex = Vector3Int.RoundToInt(blockRotationInGrid * checkGrid + blockPositionInGrid);
            //wrong Calculation

            if (gridArray[checkGridIndex.x, checkGridIndex.y, checkGridIndex.z] != null)
            {
                if (gridArray[checkGridIndex.x, checkGridIndex.y, checkGridIndex.z].occupied) { isOccupied = true; break; }
            }
        }
        return isOccupied;
    }

    public Vector3Int FitIndexToNaturalNum(Vector3Int gridIndex)
    {
        return gridIndex + gridOffsetVector;
    }

    public bool TestIndexWithinArryRange(Vector3Int gridIndex)
    {
        bool withinRange = false;

        if ((gridIndex.x >= 0) && (gridIndex.x <= (gridSize - 1))
            && (gridIndex.y >= 0) && (gridIndex.y <= (gridSize - 1))
            && (gridIndex.z >= 0) && (gridIndex.z <= (gridSize - 1)))
        {
            withinRange = true;
        }

        return withinRange;
    }

    public Vector3Int OffsetBlockInstancePositionToPlaceable(BlockBase currentBlockBase, Vector3Int oldGridPosition, Quaternion currentBlockRotation, Vector3 gridOffsetDirection)
    {
        bool isOccupied = CheckGirdOccupied(currentBlockBase.GetComponentInChildren<BlockBase>(), oldGridPosition, currentBlockRotation);
        int offsetDistance = 0;
        Vector3Int newGridPosition = oldGridPosition;

        while (isOccupied)
        {
            offsetDistance += 1;
            newGridPosition = Vector3Int.CeilToInt((Quaternion.Inverse(gridOriginRotation) * gridOffsetDirection.normalized) * offsetDistance) + oldGridPosition;
            isOccupied = CheckGirdOccupied(currentBlockBase.GetComponentInChildren<BlockBase>(), newGridPosition, currentBlockRotation);
        }
        Debug.LogError("offset " + offsetDistance);
        return newGridPosition;
    }

    public void DestroyAllBlockInGrid()
    {
        foreach (BlockListInfo blockListInfo in blockList)
        {
            Destroy(blockListInfo.blockElement.transform.parent);
        }
        blockList.Clear();
        Array.Clear(gridArray,0, gridSize);
    }

    public Vector3Int WorldPositionToGridIndex(Vector3 worldPosition)
    {
        return Vector3Int.RoundToInt(Quaternion.Inverse(transform.rotation) * (worldPosition - gridOriginPosition) / gridUnit) + gridOffsetVector;
    }

    public Vector3 WorldVectorToGrid(Vector3 worldVector)
    {
        return Vector3.Normalize(Quaternion.Inverse(transform.rotation) * worldVector);
    }

    public Vector3 GridIndexToWorldPosition(Vector3Int gridIndex)
    {
        return transform.rotation * (gridIndex - gridOffsetVector)* gridUnit + gridOriginPosition;
    }

    public Vector3 GridVectorToWorld(Vector3 gridVector)
    {
        return Vector3.Normalize(transform.rotation * gridVector);
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeGridInfo();
        gridOriginPosition = this.transform.position;
        gridOriginRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
