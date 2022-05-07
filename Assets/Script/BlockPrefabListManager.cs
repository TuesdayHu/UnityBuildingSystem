using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPrefabListManager : MonoBehaviour
{
    public List<GameObject> blockPrefabList = new List<GameObject>();

    private BuildManager BM;

    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        BM = FindObjectOfType<BuildManager>().GetComponent<BuildManager>();
        //Debug.LogError(BM.transform.position);
        BM.currentBlock = blockPrefabList[currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        //Switch the BuildManager scelection to any Prefab on the blockPrefabList according to the num input 1-9-0
        if (!BM.playingFlag && Input.anyKeyDown)
        {
            if (int.TryParse(Input.inputString, out currentIndex))
            {
                Debug.Log("Pressed " + currentIndex);
                BM.currentBlock = blockPrefabList[currentIndex];
                BM.UpdateCurrentBlockInstance(BM.currentBlock);

            }
        }
  
    }
}
