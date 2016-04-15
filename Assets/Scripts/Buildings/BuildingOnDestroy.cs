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
        else
        {
            Vector3 newPos = new Vector3(transform.position.x, 1.0f, transform.position.z);
            GameObject newMine = Instantiate(Resources.Load("Other/ResourceMine"), newPos, Quaternion.identity) as GameObject;
            newMine.GetComponent<ResourceMine>().resource = 2000;
            newMine.GetComponent<ResourceMine>().maxGatherers = 1;
        }
    }
} 
