using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public KeyCode playModeSwitch = KeyCode.M;
    public KeyCode buildModeSwitch = KeyCode.B;
    public KeyCode rotateBlock = KeyCode.R;
    //param about input

    private GridManager GM;
    //param about other components

    public bool playingFlag { get; private set; } = false;
    public bool buildingFlag { get; private set; } = false;
    //playing true building true => not exist yet 
    //playing false building true => building
    //playing true building false => playing
    //playing false building false => spectating

    public bool allowPlacing = false;
    //param about status

    public GameObject currentBlockPrefab;
    public GameObject currentBlockInstance;
    public Vector3 currentBlockInstancePosition = Vector3.zero;
    public Vector3 currentBlockInstanceEulerRotation = Vector3.zero;
    //param about block instance

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
        currentBlockInstance = Instantiate(currentBlockPrefab, currentBlockInstancePosition, Quaternion.Euler(currentBlockInstanceEulerRotation));
        currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = false;
        currentBlockMaterial = currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material;
        currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = placeableMaterial;
    }
    //Instantiate a new Current Block
    private void DestoryBlockInstance()
    {
        if (currentBlockInstance != null) { Destroy(currentBlockInstance); }
    }
    //Destory the block instance held in hand now

    public void RefreshCurrentBlockInstance()
    {
        if (currentBlockInstance != null) { Destroy(currentBlockInstance); }
        currentBlockInstanceEulerRotation = Vector3.zero;
        InstantiateCurrentBlock();
    }
    //Switch the current block instance, or instantiate a new one if there is none.
    //build and destorying the block instance    

    private void ChangeBlockInstanceRotation()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        List<Vector3> axisList = new List<Vector3>() { GM.transform.up, - GM.transform.up, GM.transform.forward, - GM.transform.forward, GM.transform.right, - GM.transform.right };
        float minAngle = 180f;
        float tempAngle = 180f;
        Vector3 closeAxis = new Vector3(1, 0, 0);

        foreach (Vector3 iAxis in axisList)
        {
            tempAngle = Vector3.Angle(cameraDirection, iAxis);
            if (tempAngle < minAngle)
            {
                minAngle = tempAngle;
                closeAxis = iAxis;
            }
        }
        currentBlockInstanceEulerRotation += closeAxis * 90;

        currentBlockInstanceEulerRotation.x = currentBlockInstanceEulerRotation.x % 360;
        currentBlockInstanceEulerRotation.y = currentBlockInstanceEulerRotation.y % 360;
        currentBlockInstanceEulerRotation.z = currentBlockInstanceEulerRotation.z % 360;
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
    //will need to add more function for manully deciding blocks rotation/positionOffset

    private bool UpdateCurrentBlockInstancePosition()//only update position now, later will add moveing position according to CurrentBlockInstancePlaceableCheck.
    {
        bool placeable = false;

        if (GetMousePointingPosition(out rayResult) && currentBlockInstance.GetComponentInChildren<BlockBase>().initializedFlag)//get the ray result from mouse
        {
            int hitObjectSocket = hitObjectBlockBase.GetClosestSocket(mousePointingPosition);

            Vector3 defaultInstancePosition = hitObjectBlockBase.GetSocket(hitObjectSocket).transform.position + mousePointingNormalDirection * GM.gridUnit / 2;// offset 0.5*grid size to get the world position of default position

            Vector3Int defaultGridIndex = Vector3Int.RoundToInt(Quaternion.Inverse(GM.transform.rotation) * GM.WorldPositionToGrid(defaultInstancePosition));//move the point back and rotate back the position, to get the index
            Vector3 normalDirectionToGrid = Vector3.Normalize((Quaternion.Inverse(GM.transform.rotation) * mousePointingNormalDirection));
            Debug.LogWarning(defaultGridIndex + "00000000");
            GM.FitIndexToGridIndex(ref defaultGridIndex);
            Debug.LogWarning(defaultGridIndex + "11111111111111");
            int offsetDistance = OffsetBlockInstancePositionToPlaceable(defaultGridIndex, normalDirectionToGrid);
            Vector3Int newGridIndex = Vector3Int.RoundToInt(defaultGridIndex + normalDirectionToGrid.normalized * offsetDistance);

            Debug.LogWarning(defaultGridIndex);
            if (GM.FitIndexToGridIndex(ref defaultGridIndex))
            {

            }

            currentBlockInstancePosition = GM.GridPositionToWorld(currentBlockInstanceGridIndex);
            Debug.LogWarning(hitObjectBlockBase.GetSocket(hitObjectSocket).transform.position);
            // Check if pointing on a BlockBase and Calculate the position which the current block should be
        }
        return placeable;
    }
    //If it's buidling mode, update the block position at each frame

    private int OffsetBlockInstancePositionToPlaceable(Vector3Int oldGridPosition, Vector3 gridOffsetDirection)
    {
        bool isOccupied = GM.CheckGirdOccupied(currentBlockInstance.GetComponentInChildren<BlockBase>(), oldGridPosition, currentBlockInstanceEulerRotation);
        int offsetDistance = 0;
        Vector3Int newGridPosition = oldGridPosition;
        while (isOccupied)
        {
            offsetDistance += 1; 
            newGridPosition = Vector3Int.CeilToInt((Quaternion.Inverse(GM.gridOriginRotation) * gridOffsetDirection.normalized) * offsetDistance) + oldGridPosition;
            isOccupied = GM.CheckGirdOccupied(currentBlockInstance.GetComponentInChildren<BlockBase>(), newGridPosition, currentBlockInstanceEulerRotation);
        }
        return offsetDistance;
    }


    private void PlaceCurrentBlock()
    {
        GameObject placingObject = currentBlockInstance;
        BlockBase placingBlockBase = placingObject.GetComponentInChildren<BlockBase>();

        GM.AddBlockInfo(placingBlockBase, currentBlockInstanceGridIndex, currentBlockInstanceEulerRotation);
        placingBlockBase.GetComponent<Collider>().enabled = true;
        placingBlockBase.GetComponent<Renderer>().material = currentBlockMaterial;

        InstantiateCurrentBlock();
    }
    //place the block in hand into the grid position
    //Block placement 

    private void InitGridManager()
    {
        if (GM.blockList.Count == 0)
        {
            GameObject firstBlockInstance = Instantiate(currentBlockPrefab, GM.transform.position + Vector3Int.zero, GM.transform.rotation * Quaternion.Euler(Vector3.zero));
            firstBlockInstance.transform.SetParent(GM.transform, true);
            BlockBase firstBlockBase = firstBlockInstance.GetComponentInChildren<BlockBase>();
            firstBlockBase.GetComponent<Collider>().enabled = true;

            Vector3Int firstIndex = Vector3Int.zero;
            GM.FitIndexToGridIndex(ref firstIndex);
            GM.AddBlockInfo(firstBlockBase, firstIndex, Vector3.zero);
        }
    }

    private void SwitchPlayMode()
    {
        playingFlag = !playingFlag;
        buildingFlag = false;
        Debug.Log("Playing is " + playingFlag);
        DestoryBlockInstance();
    }
    //switch between play and static mode

    private void SwitchBuildMode()
    {
        buildingFlag = !buildingFlag;
        playingFlag = false;

        if (!buildingFlag)
        { DestoryBlockInstance(); }//Leaving Build Mode
        else 
        {
            RefreshCurrentBlockInstance();
            InitGridManager();
        }
        //Enter Build Mode
    }
    //siwtch between build and non-build mode

    // Start is called before the first frame update
    void Awake()
    {
        GM =FindObjectOfType<GridManager>().GetComponent<GridManager>();
        //UpdateCurrentBlockInstance(currentBlock);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(playModeSwitch))
        {
            SwitchPlayMode();
        }
        //switch between play and static mode

        if (Input.GetKeyDown(buildModeSwitch))
        {
            SwitchBuildMode();
        }
        //siwtch between build and non-build mode

        if (!playingFlag && buildingFlag)
        {
            bool placeable = UpdateCurrentBlockInstancePosition();
            if (placeable)
            {
                currentBlockInstance.transform.position = currentBlockInstancePosition;
                currentBlockInstance.transform.rotation = Quaternion.Euler(currentBlockInstanceEulerRotation);
            }

            //If it's buidling mode, update the block position at each frame

            if (Input.GetKeyDown(rotateBlock))
            {
                ChangeBlockInstanceRotation();
            }
            //rotate block

            if (Input.GetMouseButtonDown(0) && allowPlacing)
            {
                PlaceCurrentBlock();
            }
            else if (Input.GetMouseButtonDown(0) && !allowPlacing)
            {
                Debug.Log("Ileagal placment of current block");
            }
            //Key input
        }
    }
}
