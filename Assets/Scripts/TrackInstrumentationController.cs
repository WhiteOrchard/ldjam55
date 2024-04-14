using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackInstrumentationController : MonoBehaviour
{
    public List<SectorStartBehavior> sectorStartBehaviors = new List<SectorStartBehavior>();

    public Vector3 GetSectorStartPosition(int sectorNumber)
    {
        return sectorStartBehaviors[sectorNumber].transform.position;
    }   
}
