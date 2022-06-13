using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketEngine : BlockBase
{
    public KeyCode blockActionKey = KeyCode.Space;
    public float engineForce = 10f;

    public override void BlockAction(bool positiveAction)
    {
        Rigidbody RB;
        if (TryGetComponent<Rigidbody>(out RB))
        {
            RB.AddForce(-1 * engineForce * this.transform.up);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBlockBase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (GIM.currentGameState == GameInputManager.GameState.Play)
        {
            if (Input.GetKey(blockActionKey)) { BlockAction(true); }
        }
    }
}
