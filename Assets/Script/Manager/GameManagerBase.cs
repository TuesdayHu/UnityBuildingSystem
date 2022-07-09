using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBase : MonoBehaviour
{
    //public static GameManagerBase instance { get; private set; }

    private void Awake()
    {
        //if (instance != null && instance != this)
        //{
        //    Debug.LogWarning("found multi instance " + this.name);
        //    Destroy(instance);
        //}
        //else
        //{
        //    instance = this;
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
