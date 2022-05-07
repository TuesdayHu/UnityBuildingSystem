using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotateSpeed = 500f;
    public float zoomScale = 35f;
    public Transform rotateCenter;

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
            transform.RotateAround(rotateCenter.position, Vector3.up,Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);
            transform.RotateAround(rotateCenter.position, Vector3.Cross(transform.forward, new Vector3(0,1,0)), Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * zoomScale);
        }
    }
}
