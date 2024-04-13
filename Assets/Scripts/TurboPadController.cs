using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboPadController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            vehicle.enableTurbo();
            Debug.Log("Turbo enabled");
        }
    }
}
