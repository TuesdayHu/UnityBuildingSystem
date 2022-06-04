using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRootManager : MonoBehaviour
{
    private BuildManager BM;
    private VehicleRootManager VRM;
    private GridManager GM;

    private List<GameObject> buildBlockList = new List<GameObject>();
    private List<GridManager.BlockListInfo> gmBlockList;

    public void MoveBuildToVehicle()
    {
        //foreach (var block in gmBlockList)
        //{
        //}

        gmBlockList = GM.blockList;
        BlockBase[] buildBlockBase;
        buildBlockBase = GM.GetComponentsInChildren<BlockBase>();
        foreach (var block in buildBlockBase)
        {
            buildBlockList.Add(block.transform.parent.gameObject);
        }
        foreach (GameObject block in buildBlockList)
        {
            block.transform.SetParent(VRM.transform, false);
            block.GetComponentInChildren<BlockBase>().gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
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
