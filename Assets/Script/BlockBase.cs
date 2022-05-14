using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    [SerializeField]private List<Socket> socketList;//the list of each socket
    private List<Vector3> socketPositionList = new List<Vector3>();//the position of each socket
    private List<Quaternion> socketQuaternionList = new List<Quaternion>();//the rotation of each socket
    private List<Vector3> socketFacingVector = new List<Vector3>();//vector list from blockbase center to each socket

    public bool initializedFlag { get; private set; } = false;

    private BuildManager BM;

    private Vector3 overlapBoxhalfExtents;
    [SerializeField] private float overlapBoxOffset = 0.05f;

    //private Collider currentCollider;

    public int GetClosestSocket(Vector3 inputPosition)
    {
        float tempDistance = Vector3.Distance(socketPositionList[0], inputPosition);
        float currentDistance = tempDistance;
        int socketId = 0;
        if (socketList.Count > 1)
        {
            for (int i = 1; i < socketList.Count; i++)
            {
                currentDistance = Vector3.Distance(socketPositionList[i], inputPosition);
                //Debug.LogError("Current Testing " + i);
                if (currentDistance < tempDistance)
                {
                    tempDistance = currentDistance;
                    socketId = i;
                }
            }
        }
        //Debug.LogError("exist" + socketId);
        return socketId;
    }
    //Find the closest socket index to the rayhit position

    public int GetFacingSocket(GameObject facingTowardObject, int facingSocketId)
    {
        //Debug.LogError(facingTowardObject.name + "-----" + facingTowardObject.transform.position);
        //Debug.LogError(transform.position + "===" + this.transform.position);
        Vector3 selfToFacingObject = facingTowardObject.transform.position - facingTowardObject.GetComponent<BlockBase>().GetSocketPosition(facingSocketId);

        //Might cause reporting one frame error if this function is called one frame before socketFacingVector list generated.
        float tempAngle = Mathf.Abs(Vector3.Angle(selfToFacingObject, socketFacingVector[0]));
        float currentAngle = tempAngle;
        int socketId = 0;
        if (socketFacingVector.Count > 1)
        {
            for (int i = 1; i < socketFacingVector.Count; i++)
            {
                currentAngle = Mathf.Abs(Vector3.Angle(selfToFacingObject, socketFacingVector[i]));
                //Debug.LogWarning(currentAngle);
                if (currentAngle < tempAngle)
                {
                    tempAngle = currentAngle;
                    socketId = i;
                }
            }
        }
        //Debug.LogError("current " + socketId);
        return (socketId);
    }
    //Find in this block, which socket is facing towards the mouse pointing existing object
    //Can be used as offset for placing the block in hand in case of bumping into the existing block

    public Vector3 GetSocketPosition(int socketId)
    {
        return socketPositionList[socketId];
    }
    //get related position according to the index

    public Quaternion GetSocketQuaternion(int socketId)
    {
        return socketQuaternionList[socketId];
    }
    //get related quaternion according to the index

    public Vector3 GetSocketFacingVector(int socketId)
    {
        return socketFacingVector[socketId];
    }
    //get the socket direction from the block center according to the index

    public void RefreshBlockBaseSocketList()
    {
        socketPositionList.Clear();
        socketQuaternionList.Clear();
        socketFacingVector.Clear();

        socketList = GetComponentsInChildren<Socket>().ToList();
            //Debug.Log("Socket found" + socketList.Count);
        foreach (Socket isocket in socketList)
        {
            socketPositionList.Add(isocket.transform.position);
            socketQuaternionList.Add(isocket.transform.rotation);
            socketFacingVector.Add(isocket.transform.position - transform.position);
        }
        //Debug.LogWarning("Initialized");
        initializedFlag = true;
    }
    //Initialize the Block information

    private void CalculateOverlapBoxSize()
    {
        float minx = socketPositionList[0].x;
        float miny = socketPositionList[0].y;
        float minz = socketPositionList[0].z;

        float maxx = socketPositionList[0].x;
        float maxy = socketPositionList[0].y;
        float maxz = socketPositionList[0].z;

        if (socketPositionList.Count > 1)
        {
            for (int i = 1; i < socketPositionList.Count; i++)
            {
                minx = socketPositionList[i].x < minx ? socketPositionList[i].x : minx;
                miny = socketPositionList[i].y < miny ? socketPositionList[i].y : miny;
                minz = socketPositionList[i].z < minz ? socketPositionList[i].z : minz;

                maxx = socketPositionList[i].x > maxx ? socketPositionList[i].x : maxx;
                maxy = socketPositionList[i].y > maxy ? socketPositionList[i].y : maxy;
                maxz = socketPositionList[i].z > maxz ? socketPositionList[i].z : maxz;
            }
        }

        overlapBoxhalfExtents = new Vector3((maxx-minx)/2 - overlapBoxOffset, (maxy-miny)/2 - overlapBoxOffset, (maxz-minz)/2 - overlapBoxOffset);
    }

    public bool DetectAllowPlacingWithoutCollision()
    {
        Collider[] collideLists = Physics.OverlapBox(transform.position, overlapBoxhalfExtents, transform.rotation, -1, QueryTriggerInteraction.Ignore);
        if (collideLists.Length > 0) { return false; }
        else { return true; }
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshBlockBaseSocketList();
        BM = FindObjectOfType<BuildManager>().GetComponent<BuildManager>();
        //currentCollider = BM.currentBlockInstance.GetComponent<Collider>();

        CalculateOverlapBoxSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
