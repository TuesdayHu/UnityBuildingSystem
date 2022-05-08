using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public KeyCode playModeSwitch = KeyCode.M;

    public bool playingFlag = false;

    //param about block instance
    public GameObject currentBlock;
    public GameObject currentBlockInstance;


    public float rayDistance = 100f;
    private RaycastHit rayResult;
    private Vector3 mousePosition;
    private Quaternion mouseDirection;
    private Vector3 socketPosition;//?
    private Quaternion socketDirection;
    private GameObject hitObject;
    private BlockBase hitObjectBlockBase;

    private bool GetMousePointingPosition( out RaycastHit rayResult, out Vector3 pointingResult, out Quaternion rotationResult)
    {
        //Debug.LogWarning("Pressed");
        Vector3 cameraPosition = Camera.main.transform.position;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool ifHit = Physics.Raycast(mouseRay, out rayResult, rayDistance);
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
            Debug.LogError(currentBlockBase);
            int currentSocket = currentBlockBase.GetFacingSocket(hitObjectBlockBase.gameObject, hitObjectSocket);
            defaultPosition = hitObjectBlockBase.GetSocketPosition(hitObjectSocket) - currentBlockBase.GetSocketFacingVector(currentSocket);
            defaultRotation = hitObjectBlockBase.GetSocketQuaternion(hitObjectSocket);

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


    public void UpdateCurrentBlockInstance(GameObject currentBlock)
    {
        //Debug.LogError("currentBlockInstance" + currentBlockInstance);
        if (currentBlockInstance == null)
        {
            currentBlockInstance = Instantiate(currentBlock, mousePosition, mouseDirection);
            currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = false;
            Debug.Log("new Instance");
        }
        else
        {
            Destroy(currentBlockInstance);
            currentBlockInstance = Instantiate(currentBlock, mousePosition, mouseDirection);
            currentBlockInstance.GetComponentInChildren<BlockBase>().GetComponent<Collider>().enabled = false;
            Debug.Log("Regenerate Instance");
        }
        //Debug.LogError("UpdateFinished" + currentBlockInstance);
    }
    //Switch the current block instance, or instantiate a new one if there is none.

    public void DestoryBlockInstance()
    {
        if (currentBlockInstance != null) {Destroy(currentBlockInstance); }
    }
    //Destory the block instance held in hand now

    // Start is called before the first frame update
    void Start()
    {
        playingFlag = false;

        //Eneter Build mode when start, will change this later.
        UpdateCurrentBlockInstance(currentBlock);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(playModeSwitch))
        {
            mousePosition = Vector3.zero;
            mouseDirection = Quaternion.identity;

            playingFlag = !playingFlag;
            Debug.Log("Playing is " + playingFlag);
            if (playingFlag)
            {DestoryBlockInstance();}
            else
            {UpdateCurrentBlockInstance(currentBlock); }
        }
        //switch between play and build mode


        //check when building if the selected prefab is instantiated

        if (!playingFlag)
        {
            if (GetMousePointingPosition(out rayResult, out mousePosition, out mouseDirection))//get the ray result from mouse
            {
                hitObject = rayResult.collider.gameObject;
                if (hitObject.TryGetComponent<BlockBase>(out hitObjectBlockBase))// if the hit object is a blockbase object
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

        //if (Input.GetKey(KeyCode.Mouse0) && playingFlag)
        //{
        //    if (GetMousePointingPosition(out mousePosition, out mouseDirection))
        //    {
        //        currentBlockInstance.transform.position = mousePosition;
        //        currentBlockInstance.transform.rotation = mouseDirection;
        //    }
        //}
        //place prefab object

    }
}
