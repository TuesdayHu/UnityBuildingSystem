using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotateSpeed = 500f;
    public float zoomScale = 35f;
    public float panSpeed = 20f;

    public Transform rotateCenter;
    public Vector3 panOffset = Vector3.zero;
    


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

        if (Input.GetMouseButton(2))
        {
            transform.position += -1 * transform.right * panSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            transform.position += -1 * transform.up * panSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        }

        //if (Input.GetKeyDown(KeyCode.F)){ transform.position = panOffset; panOffset = Vector3.zero; }

        if (Input.mouseScrollDelta.y != 0)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * zoomScale);
        }
    }
}
