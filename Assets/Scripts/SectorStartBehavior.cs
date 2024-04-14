using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorStartBehavior : MonoBehaviour
{
    public bool isFinishLine = false;
    public int sectorNumber = 0;

    private void OnTriggerEnter(Collider other)
    {
        VehicleController vehicle = other.GetComponent<VehicleController>();
        if (vehicle != null)
        {
            if (isFinishLine)
            {
                vehicle.newLap();
            }

            vehicle.setLastCrossedSector(sectorNumber);
            Debug.Log("Now in sector " + sectorNumber.ToString());
        }
    }
}
