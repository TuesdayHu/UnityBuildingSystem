using System.Collections.Generic;
using UnityEngine;

public class BuildRootManager : MonoBehaviour
{
    public static BuildRootManager instance { get; private set; }

    public class JointInfo
    {
        public BlockBase[] jointConnection;

        public JointInfo(BlockBase block1, BlockBase block2)
        {
            jointConnection = new BlockBase[2] { block1, block2 };
        }
    }

    private BuildManager BM;
    private VehicleRootManager VRM;
    private GridManager GM;

    private List<GameObject> buildBlockList = new List<GameObject>();
    private List<GridManager.BlockListInfo> gmBlockList;
    [SerializeField]
    public List<JointInfo> JointList = new List<JointInfo>();

    public void GenerateVehicleFromGridInfo()
    {
        gmBlockList = GM.blockList;
        JointList = SolveJointList(gmBlockList);

        foreach (var blockinfo in gmBlockList)
        {
            blockinfo.blockElement.transform.parent.SetParent(VRM.transform, false);
        }
        foreach (JointInfo jointInfo in JointList)
        {
            AddJointComponent(jointInfo.jointConnection);
        }
        foreach (var blockinfo in gmBlockList)
        {
            blockinfo.blockElement.SetBlockToMoveable() ;
        }

    }

    private List<JointInfo> SolveJointList(List<GridManager.BlockListInfo> inputBlockList)
    {
        List<GridManager.BlockListInfo> blockList = inputBlockList;
        List<JointInfo> outputJointList = new List<JointInfo>();
        JointInfo currentJointInfo;
        GridManager.GridPointInfo currentGridPointInfo;

        for (int i = 0; i < blockList.Count; i++)
        {
            List<Vector3Int> currentSocketConnectedGridList = blockList[i].blockElement.socketConnectedGridList;
            BlockBase currentBlockBase = blockList[i].blockElement;

            for (int j = 0; j < currentSocketConnectedGridList.Count; j++)
            {
                Vector3Int connectedIndex = Vector3Int.RoundToInt(blockList[i].gridRotation * currentSocketConnectedGridList[j] + blockList[i].gridPosition);
                currentGridPointInfo = GM.GetGridPointInfo(connectedIndex);

                if (currentGridPointInfo != null)
                {
                    Vector3 blockPosition = blockList[i].blockElement.socketList[j].transform.position;
                    if (currentGridPointInfo.occupied && currentGridPointInfo.blockInPlace.CheckHasSocketInPosition(blockPosition) )
                    {
                        if (currentGridPointInfo.blockListIndex > i) { currentJointInfo = new JointInfo(currentBlockBase, currentGridPointInfo.blockInPlace); }
                        else { currentJointInfo = new JointInfo(currentGridPointInfo.blockInPlace, currentBlockBase); }
                        
                        
                        bool arrayExist = false;
                        foreach (JointInfo iJointArray in outputJointList)
                        {
                            //System.Array.Exists(iJointArray, BlockBase i => i == currentJointInfo.JointConnection[0]);
                            if ((currentJointInfo.jointConnection[0] == iJointArray.jointConnection[0]) &&
                                (currentJointInfo.jointConnection[1] == iJointArray.jointConnection[1]))
                            {
                                arrayExist = true;
                                break;
                            }
                        }

                        if (!arrayExist)
                        {
                            outputJointList.Add(currentJointInfo);
                        }
                    }
                }
            }

        }

        return outputJointList;
    }

    private void AddJointComponent(BlockBase[] JointConnection)
    {
        if (JointConnection.Length == 2)
        {
            ConfigurableJoint newJoint = JointConnection[0].gameObject.AddComponent<ConfigurableJoint>();
            newJoint.connectedBody = JointConnection[1].GetComponent<Rigidbody>();
            newJoint.angularXMotion = ConfigurableJointMotion.Locked;
            newJoint.angularYMotion = ConfigurableJointMotion.Locked;
            newJoint.angularZMotion = ConfigurableJointMotion.Locked;
            newJoint.xMotion = ConfigurableJointMotion.Locked;
            newJoint.yMotion = ConfigurableJointMotion.Locked;
            newJoint.zMotion = ConfigurableJointMotion.Locked;
            newJoint.projectionMode = JointProjectionMode.PositionAndRotation;
            newJoint.projectionAngle = 0;
            newJoint.projectionDistance = 0;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        GM = GetComponent<GridManager>();
        BM = FindObjectOfType<BuildManager>();
        VRM = FindObjectOfType<VehicleRootManager>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
