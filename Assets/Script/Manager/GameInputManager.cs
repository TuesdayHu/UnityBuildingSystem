using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public enum GameState
    {
        Enter,
        Play,
        Observe,
        Addblock
    }
    public GameState currentGameState { get; private set; }

    private BuildManager BM;
    private BuildRootManager BRM;
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
        BRM =  FindObjectOfType<BuildRootManager>().GetComponent<BuildRootManager>();
    }

    void Start()
    {
        currentGameState = GameState.Play;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGameState)
        {
            default:
                break;

            case GameState.Enter:
                if (Input.GetKeyDown(playModeSwitch))
                {
                    BM.DestoryBlockInstance();
                }
                break;

            case GameState.Play:
                if (Input.GetKeyDown(playModeSwitch)) { currentGameState = GameState.Observe; break; }
                break;

            case GameState.Observe:
                if (Input.GetKeyDown(playModeSwitch))
                { 
                    currentGameState = GameState.Play;
                    BRM.MoveBuildToVehicle();
                    break; 
                }
                if (Input.GetKeyDown(buildModeSwitch)) 
                { 
                    currentGameState = GameState.Addblock;
                    BM.RefreshCurrentBlockInstance();
                    BM.InitGridManager(); 
                    break;
                }
                break;

            case GameState.Addblock:
                if (int.TryParse(Input.inputString, out int currentIndex))
                {
                    BM.currentBlockPrefab = BPLM.blockPrefabList[currentIndex];
                    BM.RefreshCurrentBlockInstance();
                    break;
                }
                if (Input.GetKeyDown(playModeSwitch)) {currentGameState = GameState.Play; BM.DestoryBlockInstance(); BRM.MoveBuildToVehicle(); break; }
                if (Input.GetKeyDown(buildModeSwitch)) { currentGameState = GameState.Observe; BM.DestoryBlockInstance(); break; }

                if (Input.GetKeyDown(rotateBlock)) { BM.ChangeBlockInstanceRotation(); break; }
                //rotate block

                if (Input.GetMouseButtonDown(0) && BM.allowPlacing) {BM.PlaceCurrentBlock(); break; }
                else if (Input.GetMouseButtonDown(0) && !BM.allowPlacing) {Debug.Log("Ileagal placment of current block"); break; }

                break;
        }
        //switch between build and non-build mode

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
