using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPrefabListManager : MonoBehaviour
{
    public static BlockPrefabListManager instance { get; private set; }

    public List<GameObject> blockPrefabList = new List<GameObject>();

    private BuildManager BM;
    private int currentIndex = 0;

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
    }

    //initialize the list and the buildmanager selection
    private void Start()
    {
        BM = BuildManager.instance;
        BM.currentBlockPrefab = blockPrefabList[currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
