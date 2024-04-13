using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    public VehicleController vehicleController;

    float rotationSpeed = 100.0f;
    void Start()
    {
        
    }

    void Update()
    {
        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetColumn(0, vehicleController.getAnchorRight());
        matrix.SetColumn(1, vehicleController.getAnchorNormal());
        matrix.SetColumn(2, vehicleController.getAnchorForward());
        matrix[3, 3] = 1.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, matrix.rotation, Time.deltaTime * rotationSpeed);
        transform.position = vehicleController.getCameraPos();
    }
}
