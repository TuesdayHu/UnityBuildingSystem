using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    [SerializeField]private List<Socket> socketList;//the list of each socket
    [SerializeField]private List<Vector3> socketPositionList = new List<Vector3>();//the position of each socket
    private List<Quaternion> socketQuaternionList = new List<Quaternion>();//the rotation of each socket
    //param for sockets
    public bool initializedFlag { get; private set; } = false;
    //tell if is initialized

    private BuildManager BM;
    //Find some key components

    public List<Vector3Int> blockGridOccpiedList = new List<Vector3Int>();

    //Param for size and overlap box

    public Vector3 GetSocketPosition(int socketId) { return socketPositionList[socketId]; }
    //get related position according to the index

    public Quaternion GetSocketQuaternion(int socketId) { return socketQuaternionList[socketId]; }
    //get related quaternion according to the index

    public Socket GetSocket(int socketId) { return socketList[socketId]; }

    public void InitBlockBaseSocketList()
    {
        socketPositionList.Clear();
        socketQuaternionList.Clear();
        socketList = GetComponentsInChildren<Socket>().ToList();
        foreach (Socket isocket in socketList)
        {
            socketPositionList.Add(isocket.transform.position - transform.position);
            socketQuaternionList.Add(isocket.transform.rotation * Quaternion.Inverse(transform.rotation));
        }
    }
    //Initialize the Block information

    private void CalculateSize()
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

        Vector3Int blockGridSize = new Vector3Int((int)(maxx - minx), (int)(maxy - miny), (int)(maxz - minz));

        for (int i = 0; i<blockGridSize.x; i++)
        {
            for(int j = 0; j<blockGridSize.y; j++)
            {
                for (int k = 0; k < blockGridSize.z; k++)
                {
                    blockGridOccpiedList.Add(new Vector3Int(i, j, k));
                }

            }
        }

        //overlapBoxCenterOffset = new Vector3((maxx - minx) / 2 - transform.position.x, (maxy - miny) / 2 - transform.position.y, (maxz - minz) / 2 - transform.position.z);//Need To be Fixed Later maybe using grid
        //overlapBoxHalfExtents = new Vector3((maxx-minx)/2 - overlapBoxOffset, (maxy-miny)/2 - overlapBoxOffset, (maxz-minz)/2 - overlapBoxOffset);
    }

    public int GetClosestSocket(Vector3 inputPosition)
    {
        float minDistance = Vector3.Distance(socketPositionList[0], inputPosition);
        float currentDistance = minDistance;
        int socketId = 0;
        if (socketList.Count > 1)
        {
            for (int i = 1; i < socketList.Count; i++)
            {
                currentDistance = Vector3.Distance(socketPositionList[i], inputPosition);
                //Debug.LogError("Current Testing " + i);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    socketId = i;
                }
            }
        }
        //Debug.LogError("exist" + socketId);
        return socketId;
    }
    //Find the closest socket index to the rayhit position

    //public int GetFacingSocket(GameObject facingTowardObject, int facingSocketId)
    //{
    //    Vector3 selfToFacingObject = facingTowardObject.transform.position - facingTowardObject.GetComponent<BlockBase>().GetSocketPosition(facingSocketId);

    //    //Might cause reporting one frame error if this function is called one frame before socketFacingVector list generated.
    //    float tempAngle = Mathf.Abs(Vector3.Angle(selfToFacingObject, socketFacingVector[0]));
    //    float currentAngle = tempAngle;
    //    int socketId = 0;
    //    if (socketFacingVector.Count > 1)
    //    {
    //        for (int i = 1; i < socketFacingVector.Count; i++)
    //        {
    //            currentAngle = Mathf.Abs(Vector3.Angle(selfToFacingObject, socketFacingVector[i]));
    //            if (currentAngle < tempAngle)
    //            {
    //                tempAngle = currentAngle;
    //                socketId = i;
    //            }
    //        }
    //    }
    //    return (socketId);
    //}
    //Find in this block, which socket is facing towards the mouse pointing existing object
    //Can be used as offset for placing the block in hand in case of bumping into the existing block

    //public bool DetectAllowPlacingWithoutCollision()
    //{
    //    Collider[] collideLists = Physics.OverlapBox(transform.position + overlapBoxCenterOffset, overlapBoxHalfExtents, transform.rotation, -1, QueryTriggerInteraction.Ignore);
    //    if (collideLists.Length > 0) { return false; }
    //    else { return true; }
    //}

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BuildManager>().GetComponent<BuildManager>();
        //currentCollider = BM.currentBlockInstance.GetComponent<Collider>();

        InitBlockBaseSocketList();
        CalculateSize();

        initializedFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
