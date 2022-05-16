using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public KeyCode playModeSwitch { get; } = KeyCode.M;
    public KeyCode buildModeSwitch { get; } = KeyCode.B;

    public KeyCode rotateBlock { get; } = KeyCode.R;
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

    public GameObject currentBlock;
    public GameObject currentBlockInstance;
    public Vector3 currentBlockInstancePosition = Vector3.zero;
    public Vector3 currentBlockInstanceEulerRotation = Vector3.zero;
    public GameObject defaultBlockObject;
    //param about block instance

    public Material placeableMaterial;
    public Material unplaceableMaterial;

    private Material currentBlockMaterial;
    //param about material

    public float rayDistance = 100f;
    private RaycastHit rayResult;
    private Vector3 mousePosition;
    private Quaternion mouseDirection;
    private BlockBase hitObjectBlockBase;
    //param about block transform calculation

    private void InstantiateCurrentBlock()
    {
        currentBlockInstance = Instantiate(currentBlock, mousePosition, mouseDirection);
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

    public void UpdateCurrentBlockInstance(GameObject currentBlock)
    {
        if (currentBlockInstance != null) { Destroy(currentBlockInstance); }
        InstantiateCurrentBlock();
    }
    //Switch the current block instance, or instantiate a new one if there is none.
    //build and destorying the block instance    

    private bool GetMousePointingPosition(out RaycastHit rayResult, out Vector3 pointingResult, out Quaternion rotationResult)
    {
        //Debug.LogWarning("Pressed");
        Vector3 cameraPosition = Camera.main.transform.position;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool ifHit = Physics.Raycast(mouseRay, out rayResult, rayDistance, -1, QueryTriggerInteraction.Ignore);//-1
        if (ifHit)
        {
            pointingResult = rayResult.point;
            rotationResult = Quaternion.LookRotation(rayResult.normal);
            //Debug.LogError("Hit" + pointingResult);
        }
        else
        {
            pointingResult = Vector3.zero;//Maybe Later don't need to give a (0,0,0)to mouse when not hit?
            rotationResult = Quaternion.identity;
        }
        return ifHit;
    }
    //Make ray from camera towards mouse position, return world position noew; can get further information.
    //will need to add more function for manully deciding blocks rotation/positionOffset

    private void ChangeBlockInstanceRotation()
    {
        Vector3 cameraDirection = Camera.main.transform.forward;
        List<Vector3> axisList = new List<Vector3>() { Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
        float minAngle = 180f;
        float tempAngle = 180f;
        Vector3 closeAxis = new Vector3(1,0,0);

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

        Debug.LogError("current rotation = " + currentBlockInstanceEulerRotation);
    }

    private void GetBlockDefaultPlacingPosition(out Vector3 defaultPosition)
    {
        BlockBase currentBlockBase = currentBlockInstance.GetComponentInChildren<BlockBase>();

        int hitObjectSocket = hitObjectBlockBase.GetClosestSocket(mousePosition);
        int currentSocket = currentBlockBase.GetFacingSocket(hitObjectBlockBase.gameObject, hitObjectSocket);

        defaultPosition = GM.GrabToNearGridPoint(hitObjectBlockBase.GetSocketPosition(hitObjectSocket) + rayResult.normal * GM.gridUnit / 2);//hitObjectBlockBase.GetSocketPosition(hitObjectSocket) - currentBlockBase.GetSocketFacingVector(currentSocket);
    }
    //Calculate the default current block position without other offset, so that the block in hand can stay out of the existing block.

    private void UpdateCurrentBlockInstanceTransform()//only update position now, later will add moveing position according to CurrentBlockInstancePlaceableCheck.
    {
        if (GetMousePointingPosition(out rayResult, out mousePosition, out mouseDirection))//get the ray result from mouse
        {
            GameObject hitObject = rayResult.collider.gameObject;
            if (hitObject.TryGetComponent<BlockBase>(out hitObjectBlockBase)&& currentBlockInstance.GetComponentInChildren<BlockBase>().initializedFlag)// if the hit object is a blockbase object
            {
                GetBlockDefaultPlacingPosition(out currentBlockInstancePosition);// Calculate the position which the current block should be
            }
        }
    }
    //If it's buidling mode, update the block position at each frame

    public void CurrentBlockInstancePlaceableCheck()
    {
        allowPlacing = currentBlockInstance.GetComponentInChildren<BlockBase>().DetectAllowPlacingWithoutCollision();
        if (allowPlacing) { currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = placeableMaterial; }
        else { currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = unplaceableMaterial; }
    }
    //check if the block is allowed to be placed/if the grid is occupied

    private void PlaceCurrentBlock()
    {
        GameObject placingObject = currentBlockInstance;
        BlockBase placingBlockBase = placingObject.GetComponentInChildren<BlockBase>();
        placingBlockBase.GetComponent<Collider>().enabled = true;
        placingBlockBase.GetComponent<Renderer>().material = currentBlockMaterial;
        placingBlockBase.RefreshBlockBaseSocketList();
        Vector3Int placeCenterPositionInGrid = new Vector3Int(
            (int)(currentBlockInstancePosition.x - GM.transform.position.x),
            (int)(currentBlockInstancePosition.y - GM.transform.position.y),
            (int)(currentBlockInstancePosition.z - GM.transform.position.z));
        GM.AddBlock(placeCenterPositionInGrid, placingBlockBase.blockGridOccpiedList, currentBlockInstanceEulerRotation, placingBlockBase);

        InstantiateCurrentBlock();
    }
    //place the block in hand into the grid position
    //Block placement 

    private void InitGridManager()
    {
        if (GM.blockList.Count == 0)
        {
            GameObject firstBlockInstance = Instantiate(defaultBlockObject, Vector3.zero, Quaternion.identity);
            BlockBase firstBlockBase = firstBlockInstance.GetComponentInChildren<BlockBase>();
            firstBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = true;
            GM.AddBlock(Vector3Int.zero, firstBlockBase.blockGridOccpiedList, Vector3.zero, firstBlockBase);
        }
    }

    private void SwitchPlayMode()
    {
        mousePosition = Vector3.zero;
        mouseDirection = Quaternion.identity;

        playingFlag = !playingFlag;
        buildingFlag = false;
        Debug.Log("Playing is " + playingFlag);
        DestoryBlockInstance();
    }
    //switch between play and static mode

    private void SwitchBuildMode()
    {
        mousePosition = Vector3.zero;
        mouseDirection = Quaternion.identity;

        buildingFlag = !buildingFlag;
        playingFlag = false;
        //Debug.Log("Building is " + buildingFlag);
        if (!buildingFlag)
        { DestoryBlockInstance(); }//Leaving Build Mode
        else 
        {
            UpdateCurrentBlockInstance(currentBlock);
            InitGridManager();
        }
        //Enter Build Mode
    }
    //siwtch between build and non-build mode

    // Start is called before the first frame update
    void Awake()
    {
        playingFlag = false;
        buildingFlag = false;

        GM =FindObjectOfType<GridManager>().GetComponent<GridManager>();

        //Enter Build mode when start, will change this later.
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
            UpdateCurrentBlockInstanceTransform();
            CurrentBlockInstancePlaceableCheck();

            currentBlockInstance.transform.position = currentBlockInstancePosition;
            currentBlockInstance.transform.rotation = Quaternion.Euler(currentBlockInstanceEulerRotation);
            //If it's buidling mode, update the block position at each frame

            if (Input.GetKeyDown(rotateBlock))
            {
                ChangeBlockInstanceRotation();
            }

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
