using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleRootManager : VehicleRootBase
{
    public static VehicleRootManager instance { get; private set; }

    private float vehicleEnginePower;

    public float VehicleEnginePower { get { return vehicleEnginePower; } set { vehicleEnginePower = value; } }

    // Start is called before the first frame update

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
