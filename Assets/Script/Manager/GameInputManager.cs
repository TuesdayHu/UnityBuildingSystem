using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public bool playingFlag { get; private set; } = false;
    public bool buildingFlag { get; private set; } = false;
    //playing true building true => not exist yet 
    //playing false building true => building
    //playing true building false => playing
    //playing false building false => spectating
    public int gameStatus { get; private set; } = 0;


    private BuildManager BM;
    private GridManager GM;
    private BlockPrefabListManager BPLM;

    public KeyCode playModeSwitch = KeyCode.M;
    public KeyCode buildModeSwitch = KeyCode.B;
    public KeyCode rotateBlock = KeyCode.R;
    public KeyCode printList = KeyCode.L;
    public KeyCode printArray = KeyCode.P;

    //param about input

    // Start is called before the first frame update
    void Awake()
    {
        BM = FindObjectOfType<BuildManager>().GetComponent<BuildManager>();
        GM = FindObjectOfType<GridManager>().GetComponent<GridManager>();
        BPLM = FindObjectOfType<BlockPrefabListManager>().GetComponent<BlockPrefabListManager>();
    }

    private void SwitchPlayMode()
    {
        playingFlag = !playingFlag;
        buildingFlag = false;
        Debug.Log("Playing is " + playingFlag);
        BM.DestoryBlockInstance();
    }
    //switch between play and static mode

    private void SwitchBuildMode()
    {
        buildingFlag = !buildingFlag;
        playingFlag = false;

        if (!buildingFlag)
        { BM.DestoryBlockInstance(); }//Leaving Build Mode
        else
        {
            BM.RefreshCurrentBlockInstance();
            BM.InitGridManager();
        }
        //Enter Build Mode
    }
    //siwtch between build and non-build mode

    // Update is called once per frame
    void Update()
    {
        if (!playingFlag && buildingFlag && Input.anyKeyDown)
        {
            int currentIndex;
            if (int.TryParse(Input.inputString, out currentIndex))
            {
                BM.currentBlockPrefab = BPLM.blockPrefabList[currentIndex];
                BM.RefreshCurrentBlockInstance();
            }
        }
        //Switch the BuildManager scelection to any Prefab on the blockPrefabList according to the num input 1-9-0

        if (Input.GetKeyDown(playModeSwitch))
        {
            SwitchPlayMode();
        }
        //switch between play and static mode

        if (Input.GetKeyDown(buildModeSwitch))
        {
            SwitchBuildMode();
        }
        //siwtch between build and non-build mode

        if (!playingFlag && buildingFlag)
        {
            if (Input.GetKeyDown(rotateBlock))
            {
                BM.ChangeBlockInstanceRotation();
            }
            //rotate block

            if (Input.GetMouseButtonDown(0) && BM.allowPlacing)
            {
                BM.PlaceCurrentBlock();
            }
            else if (Input.GetMouseButtonDown(0) && !BM.allowPlacing)
            {
                Debug.Log("Ileagal placment of current block");
            }
        }

        if (Input.GetKeyDown(printArray))
        {
            GM.PrintGridArray();
        }

        if (Input.GetKeyDown(printList))
        {
            GM.PrintBlockList();
        }

    }
}
