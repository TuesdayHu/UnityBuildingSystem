using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    [SerializeField]private List<Socket> socketList;
    private List<Vector3> socketPositionList = new List<Vector3>();
    private List<Quaternion> socketQuaternionList = new List<Quaternion>();
    private List<Vector3> socketFacingVector = new List<Vector3>();

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
        Debug.LogError(facingTowardObject.name + "-----" + facingTowardObject.transform.position);
        //Debug.LogError(transform.position + "===" + this.transform.position);
        Vector3 selfToFacingObject = facingTowardObject.transform.position - facingTowardObject.GetComponent<BlockBase>().GetSocketPosition(facingSocketId);
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

    // Start is called before the first frame update
    void Start()
    {
        socketList = GetComponentsInChildren<Socket>().ToList();
        Debug.Log("Socket found" + socketList.Count);
        foreach (Socket isocket in socketList)
        {
            socketPositionList.Add(isocket.transform.position);
            socketQuaternionList.Add(isocket.transform.rotation);
            socketFacingVector.Add(isocket.transform.position - transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
