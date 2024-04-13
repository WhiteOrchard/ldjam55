using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    GameObject vehicle;
    // Start is called before the first frame update
    void Start()
    {
        vehicle = GameObject.Find("Vehicle");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(vehicle.transform);
        transform.position = vehicle.transform.position - vehicle.transform.forward + Vector3.up;
    }
}
