using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    [SerializeField] private List<Socket> socketList;//the list of each socket
    [SerializeField] private List<Vector3> socketPositionInGrid = new List<Vector3>();//the position of each socket
    [SerializeField] public List<Vector3Int> socketConnectedGridList = new List<Vector3Int>();
    private List<Quaternion> socketQuaternionList = new List<Quaternion>();//the rotation of each socket
    //param for sockets
    public bool initializedFlag { get; protected set; } = false;
    //tell if is initialized

    [SerializeField] public float weight { get; private set; }
    [SerializeField] public BlockBaseCategory blockBaseCategory;

    //param about blockbase gameplay

    protected static BuildManager BM;
    protected static GridManager GM;
    protected static GameInputManager GIM;
    //Find some key components

    public List<Vector3Int> blockGridOccupiedList = new List<Vector3Int>();
    //Param for size and overlap box

    public virtual void BlockAction(bool positiveAction) { }

    public enum BlockBaseCategory
    {
        Normal,
        Engine,
        Wheel,
        Wing
    }


    public Socket GetSocket(int socketId) { return socketList[socketId]; }

    public Vector3 GetSocketPositionFromOrigin(int socketId) { return socketPositionInGrid[socketId]; }
    //get related position according to the index

    public Quaternion GetSocketQuaternion(int socketId) { return socketQuaternionList[socketId]; }
    //get related quaternion according to the index

    public void InitBlockBaseSocketList()
    {
        socketPositionInGrid.Clear();
        socketQuaternionList.Clear();
        socketConnectedGridList.Clear();
        socketList = GetComponentsInChildren<Socket>().ToList();
        foreach (Socket isocket in socketList)
        {
            socketPositionInGrid.Add(Quaternion.Inverse(transform.rotation) * (isocket.transform.position - transform.position) / GM.gridUnit);
            socketQuaternionList.Add(isocket.transform.rotation * Quaternion.Inverse(transform.rotation));
            if (isocket.allowAttach)
            {
                socketConnectedGridList.Add(Vector3Int.RoundToInt(
                    Quaternion.Inverse(transform.rotation) * (isocket.transform.position + (GM.gridUnit * 0.5f * Vector3.Normalize(isocket.transform.up)) - transform.position) / GM.gridUnit));
            }
        }
    }
    //Initialize the Block information

    protected void InitBlockBase()
    {
        BM = FindObjectOfType<BuildManager>().GetComponent<BuildManager>();
        GM = FindObjectOfType<GridManager>().GetComponent<GridManager>();
        GIM = FindObjectOfType<GameInputManager>();
        InitBlockBaseSocketList();
        CalculateSize();
        initializedFlag = true;
    }

    protected void CalculateSize()
    {
        float minx = socketPositionInGrid[0].x;
        float miny = socketPositionInGrid[0].y;
        float minz = socketPositionInGrid[0].z;

        float maxx = socketPositionInGrid[0].x;
        float maxy = socketPositionInGrid[0].y;
        float maxz = socketPositionInGrid[0].z;

        if (socketPositionInGrid.Count > 1)
        {
            for (int i = 1; i < socketPositionInGrid.Count; i++)
            {
                minx = socketPositionInGrid[i].x < minx ? socketPositionInGrid[i].x : minx;
                miny = socketPositionInGrid[i].y < miny ? socketPositionInGrid[i].y : miny;
                minz = socketPositionInGrid[i].z < minz ? socketPositionInGrid[i].z : minz;

                maxx = socketPositionInGrid[i].x > maxx ? socketPositionInGrid[i].x : maxx;
                maxy = socketPositionInGrid[i].y > maxy ? socketPositionInGrid[i].y : maxy;
                maxz = socketPositionInGrid[i].z > maxz ? socketPositionInGrid[i].z : maxz;
            }
        }

        Vector3Int blockGridSize = new Vector3Int(Mathf.RoundToInt(maxx - minx), Mathf.RoundToInt(maxy - miny), Mathf.RoundToInt(maxz - minz));
        Debug.Log("Size" + blockGridSize);
        Debug.Log("digit ++++" + maxx + "     " + minx);
        Debug.Log("digit ++++" + maxy + "     " + miny);
        Debug.Log("digit ++++" + maxz + "     " + minz);

        for (int i = 0; i < blockGridSize.x; i++)
        {
            for (int j = 0; j < blockGridSize.y; j++)
            {
                for (int k = 0; k < blockGridSize.z; k++)
                {
                    blockGridOccupiedList.Add(new Vector3Int(i, j, k));
                }
            }
        }

        Debug.Log("count " + blockGridOccupiedList.Count);
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
        InitBlockBase();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
