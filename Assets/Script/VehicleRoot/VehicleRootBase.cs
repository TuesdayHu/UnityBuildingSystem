using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleRootBase : MonoBehaviour
{
    public float vehicleMass;
    public float vehicleSpeed;
    public float motorPower;
    public struct vehicleEnergy
    {
        public float fuel;
        public float solidFuel;
        public float electricity;
        public float nitrogen;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
