using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public Transform rotateCenter;
    private float anglescale = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(rotateCenter.position, Vector3.up,Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime * anglescale);
            transform.RotateAround(rotateCenter.position, Vector3.Cross(transform.forward, new Vector3(0,1,0)), Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime * anglescale);
        }
    }
}
