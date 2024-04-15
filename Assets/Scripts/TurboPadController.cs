using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboPadController : MonoBehaviour
{
    int lastLapSpawned = 0;
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
            vehicle.enableTurbo(false);
            if (GameManager.instance.gameMode == GameMode.Summon && vehicle.isPlayer && vehicle.getStartedLapsCount() > lastLapSpawned)
            {
                lastLapSpawned = vehicle.getStartedLapsCount();
                GameManager.instance.addOpponent(vehicle.transform.position, vehicle.transform.rotation, vehicle.getLastCrossedSector());
            }
        }
    }
}
