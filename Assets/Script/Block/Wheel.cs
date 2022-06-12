using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : BlockBase
{
    public bool isPowered = true;

    public KeyCode wheelActionKey1 = KeyCode.W;
    public KeyCode wheelActionKey2 = KeyCode.S;
    public float engineForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        InitBlockBase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
