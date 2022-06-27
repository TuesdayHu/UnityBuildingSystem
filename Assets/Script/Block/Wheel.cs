using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : BlockBase
{
    public bool isPowered = true;

    public KeyCode wheelActionKey1 = KeyCode.W;
    public KeyCode wheelActionKey2 = KeyCode.S;
    public float engineForce = 100f;

    private ConfigurableJoint axleJoint;
    private JointDrive workingJointDrive = new JointDrive() { positionSpring = 50, positionDamper = 30, maximumForce = Mathf.Infinity};
    private JointDrive stopingJointDrive = new JointDrive() { positionSpring = 0, positionDamper = 30, maximumForce = Mathf.Infinity };
    public override void SetBlockToMoveable()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Rigidbody[] rbChildList = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rbChild in rbChildList) { rbChild.isKinematic = false; }
    }

    protected override void BlockAction(bool positiveAction)
    {
        int rotateDirection;
        rotateDirection = Convert.ToInt16(positiveAction) * 2 - 1;
        axleJoint.targetAngularVelocity = new Vector3(0, rotateDirection * engineForce, 0);
        axleJoint.slerpDrive = workingJointDrive;
    }

    protected override void BlockStopAction()
    {
        axleJoint.targetAngularVelocity = Vector3.zero;
        axleJoint.slerpDrive = stopingJointDrive;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBlockBase();
        axleJoint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(wheelActionKey1)) { BlockAction(true); }
        else if (Input.GetKey(wheelActionKey2)) { BlockAction(false); }
        else { BlockStopAction(); }
    }
}
