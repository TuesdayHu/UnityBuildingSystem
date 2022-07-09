using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSavingManager : GameManagerBase
{
    public static GameSavingManager instance { get; private set; }

    public class BuildSaving
    {
        public List<GridManager.BlockListInfo> buildList;
    }

    private BuildSaving buildSaving = new BuildSaving();
    private GridManager GM;
    private BuildManager BM;
    private GameInputManager GIM;

    public string buildName = "testSave";

    private void CollectBuildSaving()
    {
        buildSaving.buildList = GM.blockList;
    }

    private void WriteJSON(object saving, string saveName)
    {
        string jsonString = JsonUtility.ToJson(saving);
        Debug.LogWarning("Saving " + buildName + " : " + jsonString);

        if (!Directory.Exists(Application.dataPath + "/SavingData/")) { Directory.CreateDirectory(Application.dataPath + "/SavingData/"); }
        File.WriteAllText(Application.dataPath + "/SavingData/" + saveName + ".json", jsonString);
    }

    private void RebuildBuild(BuildSaving buildSaving)
    {
        if(buildSaving != null)
        {
            BM.RebuildBuildSaving(buildSaving);
        }
    }

    private BuildSaving ReadJSON(string saveName)
    {
        if (!File.Exists(Application.dataPath + "/SavingData/" + saveName + ".json"))
        {
            Debug.LogError(Application.dataPath + "/SavingData/" + saveName + ".json" +  "  File doesn't exist.");
            return null;
        }
        string jsonString = File.ReadAllText(Application.dataPath + "/SavingData/" + saveName + ".json");
        return JsonUtility.FromJson<BuildSaving>(jsonString);
    }

    public void SaveVehicleBuild()
    {
        if (GIM.currentGameState == GameInputManager.GameState.Addblock)
        {
            CollectBuildSaving();
            WriteJSON(buildSaving, buildName);
        }
    }

    public void ReadVehicleBuild(string readName)
    {
        if (GIM.currentGameState == GameInputManager.GameState.Addblock)
        {
            RebuildBuild(ReadJSON(readName));
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("found multi instance " + this.name);
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GM = GridManager.instance;
        BM = BuildManager.instance;
        GIM = GameInputManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
