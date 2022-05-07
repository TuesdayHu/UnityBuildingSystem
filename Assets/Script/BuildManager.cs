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
    private Vector3 socketPosition;
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
        }
        else
        {
            pointingResult = Vector3.zero;//Maybe Later don't need to give a (0,0,0)to mouse when not hit?
            rotationResult = Quaternion.LookRotation(Vector3.forward);
        }
        return ifHit;
    }
    //Make ray from camera towards mouse position, return world position noew; can get further information.
    //will need to add more function for manully deciding blocks rotation/positionOffset

    public void UpdateCurrentBlockInstance(GameObject currentBlock)
    {
        //Debug.LogError("currentBlockInstance" + currentBlockInstance);
        if (currentBlockInstance == null)
        {
            Debug.Log("new Instance");
            currentBlockInstance = Instantiate(currentBlock, mousePosition, mouseDirection);
        }
        else
        {
            Destroy(currentBlockInstance);
            currentBlockInstance = Instantiate(currentBlock, mousePosition, mouseDirection);
            Debug.LogError("Regenerate Instance");
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
            if (GetMousePointingPosition(out rayResult, out mousePosition, out mouseDirection))
            {
                hitObject = rayResult.collider.gameObject;
                if (hitObject.TryGetComponent<BlockBase>(out hitObjectBlockBase))
                {
                    Debug.Log("Playing is!!!! " + playingFlag);
                    int i = hitObjectBlockBase.GetClosestSocket(mousePosition);
                    currentBlockInstance.transform.position = hitObjectBlockBase.GetSocketPosition(i);      
                    currentBlockInstance.transform.rotation = hitObjectBlockBase.GetSocketQuaternion(i);
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
