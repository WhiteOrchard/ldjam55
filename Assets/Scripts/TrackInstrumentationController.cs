using System.Collections.Generic;
using UnityEngine;

public class TrackInstrumentationController : MonoBehaviour
{
    public List<SectorStartBehavior> sectorStartBehaviors = new List<SectorStartBehavior>();

    public Vector3 GetSectorStartPosition(int sectorNumber)
    {
        if (sectorNumber < 0)
        {
            return sectorStartBehaviors[sectorStartBehaviors.Count-1].transform.position;
        }
        if (sectorNumber >= sectorStartBehaviors.Count)
        {
            return sectorStartBehaviors[0].transform.position;
        }
        return sectorStartBehaviors[sectorNumber].transform.position;
    }   
}
