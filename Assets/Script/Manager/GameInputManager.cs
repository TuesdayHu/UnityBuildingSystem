using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        if (!BM.playingFlag && BM.buildingFlag && Input.anyKeyDown)
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
            BM.SwitchPlayMode();
        }
        //switch between play and static mode

        if (Input.GetKeyDown(buildModeSwitch))
        {
            BM.SwitchBuildMode();
        }
        //siwtch between build and non-build mode

        if (!BM.playingFlag && BM.buildingFlag)
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
