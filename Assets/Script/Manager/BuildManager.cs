using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    private GridManager GM;
    private BuildRootManager BRM;
    private GameInputManager GIM;
    //param about other components

    public bool allowPlacing = false;
    //param about status

    public GameObject currentBlockPrefab;
    public GameObject currentBlockInstance;
    public Vector3 currentBlockInstancePosition = Vector3.zero;
    public Quaternion currentBlockInstanceRotation = Quaternion.identity;
    //param about block instance

    private Quaternion currentBlockInstanceGridRotation = Quaternion.identity;
    private Vector3 currentBlockInstanceGridYDirection = new Vector3(0,1,0);
    private Vector3 currentBlockInstanceGridZDirection = new Vector3(0,0,1);
    private Vector3Int currentBlockInstanceGridIndex = Vector3Int.zero;
    //block instance grid info

    public Material placeableMaterial;
    public Material unplaceableMaterial;
    private Material currentBlockMaterial;
    //param about material

    public float rayDistance = 100f;
    private RaycastHit rayResult;
    private Vector3 mousePointingPosition;
    private Vector3 mousePointingNormalDirection;
    private BlockBase hitObjectBlockBase;
    //param about ray and rayhit

    private void InstantiateCurrentBlock()
    {
        currentBlockInstance = Instantiate(currentBlockPrefab, currentBlockInstancePosition, currentBlockInstanceRotation);
        currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = false;
        currentBlockMaterial = currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material;
        currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = placeableMaterial;
    }
    //Instantiate a new Current Block
    public void DestoryBlockInstance()
    {
        if (currentBlockInstance != null) { Destroy(currentBlockInstance); }
    }
    //Destory the block instance held in hand now

    public void RefreshCurrentBlockInstance()
    {
        if (currentBlockInstance != null) { Destroy(currentBlockInstance); }

        currentBlockInstanceGridRotation = Quaternion.identity;
        currentBlockInstanceGridYDirection = new Vector3(0,1,0);
        currentBlockInstanceGridZDirection = new Vector3(0,0,1);
        currentBlockInstanceRotation = GM.transform.rotation * currentBlockInstanceGridRotation;
        InstantiateCurrentBlock();
    }
    //Switch the current block instance, or instantiate a new one if there is none.
    //build and destorying the block instance    

    public void ChangeBlockInstanceRotation()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        List<Vector3> axisListInWorld = new List<Vector3>() { GM.transform.right, -GM.transform.right, GM.transform.up, - GM.transform.up, GM.transform.forward, - GM.transform.forward};
        List<Vector3> axisListInGrid = new List<Vector3>() { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        float minAngle = 180f;
        float tempAngle = 180f;
        int closeAxisIndex = 0;

        for (int i = 0; i < axisListInWorld.Count; i++)
        {
            tempAngle = Vector3.Angle(cameraDirection, axisListInWorld[i]);
            if (tempAngle < minAngle)
            {
                minAngle = tempAngle;
                closeAxisIndex = i;
            }
        }
        currentBlockInstanceGridYDirection = Quaternion.AngleAxis(90, axisListInGrid[closeAxisIndex]) * currentBlockInstanceGridYDirection;
        currentBlockInstanceGridZDirection = Quaternion.AngleAxis(90, axisListInGrid[closeAxisIndex]) * currentBlockInstanceGridZDirection;

        currentBlockInstanceGridRotation = Quaternion.LookRotation(currentBlockInstanceGridZDirection, currentBlockInstanceGridYDirection);
        currentBlockInstanceRotation = GM.transform.rotation * currentBlockInstanceGridRotation;

    }

    private bool GetMousePointingPosition(out RaycastHit rayResult)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitOnBlockBase = false;
        bool ifHit = Physics.Raycast(mouseRay, out rayResult, rayDistance, -1, QueryTriggerInteraction.Ignore);
        if (ifHit)
        {
            mousePointingPosition = rayResult.point;
            mousePointingNormalDirection = rayResult.normal.normalized;

            GameObject hitObject = rayResult.collider.gameObject;
            if (hitObject.TryGetComponent<BlockBase>(out hitObjectBlockBase)) { hitOnBlockBase = true; }
        }
        return hitOnBlockBase;
    }
    //Make ray from camera towards mouse position, return world position noew; can get further information.

    public void UpdateCurrentBlockInstancePosition()//only update position now, later will add moveing position according to CurrentBlockInstancePlaceableCheck.
    {
        allowPlacing = true;
        //Debug.LogError("22222" + currentBlockInstance.GetComponentInChildren<BlockBase>().initializedFlag);
        if (GetMousePointingPosition(out rayResult) && currentBlockInstance.GetComponentInChildren<BlockBase>().initializedFlag)//get the ray result from mouse
        {
            int hitObjectSocket = hitObjectBlockBase.GetClosestSocket(mousePointingPosition);

            Vector3 defaultInstancePosition = hitObjectBlockBase.GetSocket(hitObjectSocket).transform.position + mousePointingNormalDirection * GM.gridUnit / 2;

            Vector3Int defaultGridIndex = GM.WorldPositionToGridIndex(defaultInstancePosition);//move the point back and rotate back the position, to get the index
            Vector3 normalDirectionToGrid = Vector3.Normalize((Quaternion.Inverse(GM.transform.rotation) * mousePointingNormalDirection));

            Vector3Int newGridIndexInArray = GM.OffsetBlockInstancePositionToPlaceable(
                currentBlockInstance.GetComponentInChildren<BlockBase>(),
                defaultGridIndex,
                currentBlockInstanceGridRotation,
                normalDirectionToGrid);

            Vector3 newWorldPosition =  Vector3.zero;
            if (GM.TestIndexWithinArryRange(newGridIndexInArray))
            {
                newWorldPosition = GM.GridIndexToWorldPosition(newGridIndexInArray);
                allowPlacing = true;
                Debug.LogWarning("aaaaaaaaaaaa" + newGridIndexInArray);
                Debug.LogWarning("bbbbbbbbbbbb" + newWorldPosition);
            }
            else { allowPlacing = false; }

            currentBlockInstanceGridIndex = newGridIndexInArray;
            currentBlockInstancePosition = newWorldPosition;
            // Check if pointing on a BlockBase and Calculate the position which the current block should be
        }
    }
    //If it's buidling mode, update the block position at each frame

    public void PlaceCurrentBlock()
    {
        GameObject placingObject = currentBlockInstance;
        BlockBase placingBlockBase = placingObject.GetComponentInChildren<BlockBase>();

        placingObject.transform.SetParent(BRM.transform, true);
        Debug.LogError("inputindex" + currentBlockInstanceGridIndex);
        GM.AddBlockInfo(placingBlockBase, currentBlockInstanceGridIndex, currentBlockInstanceGridRotation);
        placingBlockBase.GetComponent<Collider>().enabled = true;
        placingBlockBase.GetComponent<Renderer>().material = currentBlockMaterial;

        InstantiateCurrentBlock();
    }
    //place the block in hand into the grid position
    //Block placement 

    public void InitGridManager()
    {
        if (GM.blockList.Count == 0)
        {
            currentBlockInstancePosition = GM.gridOriginPosition;
            currentBlockInstanceRotation = GM.transform.rotation * Quaternion.identity;

            GameObject firstBlockInstance = Instantiate(currentBlockPrefab, GM.transform.position + Vector3Int.zero, currentBlockInstanceRotation);
            BlockBase firstBlockBase = firstBlockInstance.GetComponentInChildren<BlockBase>();

            firstBlockInstance.transform.SetParent(BRM.transform, true);
            firstBlockBase.GetComponent<Collider>().enabled = true;

            Vector3Int firstIndex = GM.FitIndexToNaturalNum(Vector3Int.zero);
            GM.AddBlockInfo(firstBlockBase, firstIndex, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        GM = FindObjectOfType<GridManager>().GetComponent<GridManager>();
        BRM = FindObjectOfType<BuildRootManager>().GetComponent<BuildRootManager>();
        GIM = FindObjectOfType<GameInputManager>().GetComponent<GameInputManager>();
        //UpdateCurrentBlockInstance(currentBlock);
    }

    // Update is called once per frame
    void Update()
    {
        if (GIM.currentGameState == GameInputManager.GameState.Addblock)
        {
            UpdateCurrentBlockInstancePosition();
            if (allowPlacing)
            {
                currentBlockInstance.transform.position = currentBlockInstancePosition;
                currentBlockInstance.transform.rotation = currentBlockInstanceRotation;
                currentBlockInstance.SetActive(true);
                //Debug.LogWarning("Set to true");
            }
            else { currentBlockInstance.SetActive(false); Debug.LogWarning("Set to false"); }
        }
        //If it's buidling mode, update the block position at each frame
    }
}
