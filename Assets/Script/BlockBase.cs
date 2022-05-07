using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    private List<Socket> socketList;
    private List<Vector3> socketPositionList = new List<Vector3>();
    private List<Quaternion> socketQuaternionList = new List<Quaternion>();

    public int GetClosestSocket(Vector3 inputPosition)
    {
        float tempDistance = Vector3.Distance(socketPositionList[0], inputPosition);
        float minDistance = tempDistance;
        int socketId = 0;
        if (socketList.Count > 1)
        {
            for (int i = 1; i < socketList.Count; i++)
            {
                //Debug.LogError("Current Testing " + i);
                if (Vector3.Distance(socketPositionList[i], inputPosition) > tempDistance)
                {
                    socketId = i;
                }
            }
        }
        return socketId;
    }

    public Vector3 GetSocketPosition(int socketId)
    {
        return socketPositionList[socketId];
    }

    public Quaternion GetSocketQuaternion(int socketId)
    {
        return socketQuaternionList[socketId];
    }

    // Start is called before the first frame update
    void Start()
    {
        socketList = GetComponentsInChildren<Socket>().ToList();
        Debug.Log("Socket found" + socketList.Count);
        foreach (Socket isocket in socketList)
        {
            socketPositionList.Add(isocket.transform.position);
            socketQuaternionList.Add(isocket.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
