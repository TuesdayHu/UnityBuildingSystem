using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    [SerializeField]private List<Socket> socketList;//the list of each socket
    [SerializeField]private List<Vector3> socketPositionFromOriginList = new List<Vector3>();//the position of each socket
    private List<Quaternion> socketQuaternionList = new List<Quaternion>();//the rotation of each socket
    //param for sockets
    public bool initializedFlag { get; private set; } = false;
    //tell if is initialized

    private BuildManager BM;
    //Find some key components

    public List<Vector3Int> blockGridOccpiedList = new List<Vector3Int>();
    //Param for size and overlap box

    public Socket GetSocket(int socketId) { return socketList[socketId]; }

    public Vector3 GetSocketPositionFromOrigin(int socketId) { return socketPositionFromOriginList[socketId]; }
    //get related position according to the index

    public Quaternion GetSocketQuaternion(int socketId) { return socketQuaternionList[socketId]; }
    //get related quaternion according to the index

    public void InitBlockBaseSocketList()
    {
        socketPositionFromOriginList.Clear();
        socketQuaternionList.Clear();
        socketList = GetComponentsInChildren<Socket>().ToList();
        foreach (Socket isocket in socketList)
        {
            socketPositionFromOriginList.Add(isocket.transform.position - transform.position);
            socketQuaternionList.Add(isocket.transform.rotation * Quaternion.Inverse(transform.rotation));
        }
    }
    //Initialize the Block information

    private void CalculateSize()
    {
        float minx = socketPositionFromOriginList[0].x;
        float miny = socketPositionFromOriginList[0].y;
        float minz = socketPositionFromOriginList[0].z;

        float maxx = socketPositionFromOriginList[0].x;
        float maxy = socketPositionFromOriginList[0].y;
        float maxz = socketPositionFromOriginList[0].z;

        if (socketPositionFromOriginList.Count > 1)
        {
            for (int i = 1; i < socketPositionFromOriginList.Count; i++)
            {
                minx = socketPositionFromOriginList[i].x < minx ? socketPositionFromOriginList[i].x : minx;
                miny = socketPositionFromOriginList[i].y < miny ? socketPositionFromOriginList[i].y : miny;
                minz = socketPositionFromOriginList[i].z < minz ? socketPositionFromOriginList[i].z : minz;

                maxx = socketPositionFromOriginList[i].x > maxx ? socketPositionFromOriginList[i].x : maxx;
                maxy = socketPositionFromOriginList[i].y > maxy ? socketPositionFromOriginList[i].y : maxy;
                maxz = socketPositionFromOriginList[i].z > maxz ? socketPositionFromOriginList[i].z : maxz;
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
        Debug.LogError("blockGridOccpiedList" + blockGridOccpiedList.Count);
    }

    public int GetClosestSocket(Vector3 inputPosition)
    {
        float minDistance = Vector3.Distance(socketList[0].transform.position, inputPosition);
        float currentDistance = minDistance;
        int socketId = 0;
        if (socketList.Count > 1)
        {
            for (int i = 1; i < socketList.Count; i++)
            {
                currentDistance = Vector3.Distance(socketList[i].transform.position, inputPosition);
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

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BuildManager>().GetComponent<BuildManager>();

        InitBlockBaseSocketList();
        CalculateSize();
        initializedFlag = true;
        Debug.LogError("Init block33333333333333333");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
