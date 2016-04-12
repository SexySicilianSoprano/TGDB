using UnityEngine;
using System.Collections;

public class BuildingOnDestroy : MonoBehaviour {

    // This building's building spot
    public GameObject MyBuildingSpot;
    void OnDestroy()
    {
        if (GetComponent<NavalYard>())
        {
            // Free the reserved building spot for use
            MyBuildingSpot.SetActive(true);
        }
    }
}
