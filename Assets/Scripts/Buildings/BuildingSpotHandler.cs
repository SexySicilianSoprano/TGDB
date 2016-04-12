using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingSpotHandler : MonoBehaviour {

    public List<GameObject> buildingSpots = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
        GetAllBuildingSpots();
        HideBuildingSpots();
	}

    // Find every building spot under the parent * WORKS BUT NEEDS TO BE REFACTORED *
    private void GetAllBuildingSpots()
    {
        buildingSpots.Add(transform.Find("BuildingSpot").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (1)").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (2)").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (3)").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (4)").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (5)").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (6)").gameObject);
        buildingSpots.Add(transform.Find("BuildingSpot (7)").gameObject);
    }

    // Show the building spot projectors
    public void ShowBuildingSpots()
    {
        foreach (GameObject spot in buildingSpots)
        {
            if (spot.gameObject.activeInHierarchy == true) 
                spot.transform.Find("BSpotProjector").GetComponent<Projector>().enabled = true;
        }
    }

    // Hide the building spot projectors
    public void HideBuildingSpots()
    {
        foreach (GameObject spot in buildingSpots)
        {
            if (spot.gameObject.activeInHierarchy == true)
                spot.transform.Find("BSpotProjector").GetComponent<Projector>().enabled = false;
        }
    }

}
