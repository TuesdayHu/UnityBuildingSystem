using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public class BlockListInfo
    {
        public List<Vector3Int> gridIndexList;
        BlockBase blockElement;

        public BlockListInfo(List<Vector3Int> gridPositionList, BlockBase block)
        {
            gridIndexList = gridPositionList;
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
    [SerializeField]private int gridSize = 251;
    private Vector3Int gridOffsetVector;
    //param for grid

    public GridPointInfo[,,] gridArray { get; private set; }
    public List<BlockListInfo> blockList { get; private set; }
    //List and array for storing info
    public Vector3 gridOriginPosition { get; private set; } = Vector3.zero;
    public Quaternion gridOriginRotation { get; private set; } = Quaternion.identity;
    //param for center transform
    
    public Vector3Int ReturnGridOriginIndex() { return gridOffsetVector; }

    public void InitializeGridInfo()
    {
        gridArray = new GridPointInfo[gridSize, gridSize, gridSize];
        blockList = new List<BlockListInfo>();
        int gridOffset = Mathf.RoundToInt((gridSize - 1) / 2);
        gridOffsetVector = new Vector3Int(gridOffset, gridSize, gridSize);
    }

    public void SetGridTransform(Transform inputTransform)
    {
        gridOriginPosition = inputTransform.position;
        gridOriginRotation = inputTransform.rotation;
    }

    public void AddBlockInfo(BlockBase placeBlock,  Vector3Int placeCenterPosition, Vector3 placeEulerRotation)
    {
        bool intCheck = true;
        List<Vector3Int> placeGridIndexList = new List<Vector3Int>();
        List<Vector3Int> blockGridList = placeBlock.blockGridOccpiedList;
        foreach (Vector3 iPosition in blockGridList)
        {
            Vector3 newPlacePositionInGrid = Quaternion.Euler(placeEulerRotation) * iPosition;
            placeGridIndexList.Add(new Vector3Int((int)newPlacePositionInGrid.x + placeCenterPosition.x, (int)newPlacePositionInGrid.y + placeCenterPosition.y, (int)newPlacePositionInGrid.z + placeCenterPosition.z));
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
            if (gridArray[checkGridIndex.x, checkGridIndex.y, checkGridIndex.z] != null)
            {
                if (gridArray[checkGridIndex.x, checkGridIndex.y, checkGridIndex.z].occupied) { isOccupied = true; break; }
            }
        }
        return isOccupied;
    }

    public bool FitIndexToGridIndex(ref Vector3Int gridIndex)
    {
        bool withinRange = false;
        Vector3Int newGridIndex = gridIndex + gridOffsetVector;
        if ((newGridIndex.x >= 0) && (newGridIndex.x <= (gridSize - 1))
            && (newGridIndex.y >= 0) && (newGridIndex.y <= (gridSize - 1))
            && (newGridIndex.z >= 0) && (newGridIndex.z <= (gridSize - 1)))
        {
            withinRange = true;
        }
        gridIndex = newGridIndex;
        return withinRange;
    }

    public Vector3 WorldPositionToGrid (Vector3 worldPosition)
    {
        return worldPosition - gridOriginPosition;
    }

    public Vector3 WorldEulerRotationToGrid (Vector3 worldEulerRotation)
    {
        return worldEulerRotation - gridOriginRotation.eulerAngles;
    }

    public Vector3 GridPositionToWorld (Vector3 gridPosition)
    {
        return gridPosition + gridOriginPosition;
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
