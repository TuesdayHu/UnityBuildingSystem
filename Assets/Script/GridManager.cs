using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [HideInInspector]  public float gridUnit { get;} = 1f;

    public BlockBase centerBlockBase;
    private GameObject centerBlockBaseObject;
    private Vector3 originPoint;

    public class gridPointInfo
    {
        public List<Socket> SocketList = new List<Socket>();

    }




    public Vector3 GrabToNearGridPoint(Vector3 oldPosition)
    {
        float x = oldPosition.x - originPoint.x;
        float y = oldPosition.y - originPoint.y;
        float z = oldPosition.z - originPoint.z;
        
        float newx = Mathf.RoundToInt(x / gridUnit) * gridUnit + originPoint.x;
        float newy = Mathf.RoundToInt(y / gridUnit) * gridUnit + originPoint.y;
        float newz = Mathf.RoundToInt(z / gridUnit) * gridUnit + originPoint.z;

        return new Vector3(newx, newy, newz);
    }
    //Move the grid back to origin according to the center object, then calculate the grid point and move back


    // Start is called before the first frame update
    void Start()
    {
        centerBlockBaseObject = centerBlockBase.transform.parent.gameObject;
        originPoint = centerBlockBaseObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
