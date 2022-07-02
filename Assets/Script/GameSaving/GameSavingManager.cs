using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSavingManager : MonoBehaviour
{
    public class BuildSaving
    {
        public List<GridManager.BlockListInfo> buildList;
    }

    private BuildSaving buildSaving;
    private GridManager GM;

    public string buildName = "testSave";

    private void CollectBuildSaving()
    {
        buildSaving.buildList = GM.blockList;
    }

    private void WriteJSON(object saving, string saveName)
    {
        string jsonString = JsonUtility.ToJson(saving);
        Debug.LogWarning("Saving " + buildName + " : " + jsonString);
    }

    public void SaveVehicleBuild()
    {
        Debug.LogWarning("print");
        CollectBuildSaving();
        WriteJSON(buildSaving.buildList, buildName);
    }

    private void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        GM = GridManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
