using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRB : MonoBehaviour
{
    private Rigidbody rb;
    private Transform otransform;
    private Vector3 oppsition;
    private Quaternion orotation;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        oppsition = transform.position;
        orotation = transform.rotation;
    }

    IEnumerator Resetwaiting()
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(1);
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Switch");
            rb.isKinematic = ! rb.isKinematic;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Back"); 
            //Debug.LogError(originTransform.position);
            //Debug.LogError(transform.position);
            transform.position  = oppsition;
            transform.rotation = orotation;

            StartCoroutine(Resetwaiting());
        }
    }
}
