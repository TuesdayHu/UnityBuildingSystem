using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
    public bool allowAttach = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!allowAttach)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
