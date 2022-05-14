using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public KeyCode playModeSwitch { get; } = KeyCode.M;
    public KeyCode buildModeSwitch { get; } = KeyCode.B;
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


    //param about block instance
    public GameObject currentBlock;
    public GameObject currentBlockInstance;

    public Material placeableMaterial;
    public Material unplaceableMaterial;

    private Material currentBlockMaterial;
    //param about material

    public float rayDistance = 100f;
    private RaycastHit rayResult;
    private Vector3 mousePosition;
    private Quaternion mouseDirection;
    private Vector3 socketPosition;//?
    private Quaternion socketDirection;
    private GameObject hitObject;
    private BlockBase hitObjectBlockBase;
    //param about block transform calculation

    private bool GetMousePointingPosition( out RaycastHit rayResult, out Vector3 pointingResult, out Quaternion rotationResult)
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

    public bool GetBlockDefaultPlacingPosition(BlockBase hitObjectBlockBase, out Vector3 defaultPosition, out Quaternion defaultRotation)
    {
        BlockBase currentBlockBase = currentBlockInstance.GetComponentInChildren<BlockBase>();
        if (hitObjectBlockBase != null)
        {
            int hitObjectSocket = hitObjectBlockBase.GetClosestSocket(mousePosition);
            int currentSocket = currentBlockBase.GetFacingSocket(hitObjectBlockBase.gameObject, hitObjectSocket);

            defaultPosition = GM.GrabToNearGridPoint(hitObjectBlockBase.GetSocketPosition(hitObjectSocket) + rayResult.normal * GM.gridUnit / 2 );//hitObjectBlockBase.GetSocketPosition(hitObjectSocket) - currentBlockBase.GetSocketFacingVector(currentSocket);
            defaultRotation = Quaternion.identity;//hitObjectBlockBase.GetSocketQuaternion(hitObjectSocket);

            return true;
        }
        else
        {
            defaultPosition = Vector3.zero;
            defaultRotation = Quaternion.identity;

            return false; 
        }

    }
    //Calculate the default current block position without other offset, so that the block in hand can stay out of the existing block.

    public void CurrentBlockInstancePlaceableCheck()
    {
        allowPlacing = currentBlockInstance.GetComponentInChildren<BlockBase>().DetectAllowPlacingWithoutCollision();
        if (allowPlacing) { currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = placeableMaterial; }
        else { currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = unplaceableMaterial; }
    }

    private void InstantiateCurrentBlock()
    {
        currentBlockInstance = Instantiate(currentBlock, mousePosition, mouseDirection);
        currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = false;
        currentBlockMaterial = currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material;
        currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = placeableMaterial;
    }
    //Instantiate a new Current Block

    public void UpdateCurrentBlockInstance(GameObject currentBlock)
    {
        if (currentBlockInstance != null) { Destroy(currentBlockInstance); Debug.Log("Regenerate Instance"); }
        InstantiateCurrentBlock();
    }
    //Switch the current block instance, or instantiate a new one if there is none.

    private void UpdateCurrentBlockInstanceTransform()
    {
        if (GetMousePointingPosition(out rayResult, out mousePosition, out mouseDirection))//get the ray result from mouse
        {
            hitObject = rayResult.collider.gameObject;
            //Debug.LogWarning(hitObject.name + "sssssssssss" );
            if (hitObject.TryGetComponent<BlockBase>(out hitObjectBlockBase)&& currentBlockInstance.GetComponentInChildren<BlockBase>().initializedFlag)// if the hit object is a blockbase object
            {
                Vector3 defaultPosition = Vector3.zero;
                Quaternion defaultRotation = Quaternion.identity;

                if (GetBlockDefaultPlacingPosition(hitObjectBlockBase, out defaultPosition, out defaultRotation))// Calculate the position which the current block should be
                {
                    currentBlockInstance.transform.position = defaultPosition;
                    currentBlockInstance.transform.rotation = defaultRotation;
                    //put the current block to the right place
                }
            }
        }
    }
    //If it's buidling mode, update the block position at each frame

    private void PlaceCurrentBlock()
    {
        GameObject placingObject = currentBlockInstance;
        placingObject.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = true;
        placingObject.GetComponentInChildren<BlockBase>().GetComponent<Renderer>().material = currentBlockMaterial;
        placingObject.GetComponentInChildren<BlockBase>().RefreshBlockBaseSocketList();
        InstantiateCurrentBlock();
    }


    public void DestoryBlockInstance()
    {
        if (currentBlockInstance != null) {Destroy(currentBlockInstance); }
    }
    //Destory the block instance held in hand now

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
        Debug.Log("Building is " + buildingFlag);
        if (!buildingFlag)
        { DestoryBlockInstance(); }
        else
        { UpdateCurrentBlockInstance(currentBlock); }
    }
    //siwtch between build and non-build mode

    // Start is called before the first frame update
    void Start()
    {
        playingFlag = false;
        buildingFlag = false;

        GM = GameObject.Find("GridManager").GetComponent<GridManager>();

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
            //If it's buidling mode, update the block position at each frame

            if (Input.GetMouseButtonDown(0) && allowPlacing)
            {
                PlaceCurrentBlock();
            }
            else if (Input.GetMouseButtonDown(0) && !allowPlacing)
            {
                Debug.Log("Ileagal placment of current block");
            }
        }


    }


}
