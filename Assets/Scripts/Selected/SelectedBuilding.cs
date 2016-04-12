using UnityEngine;
using System.Collections;

public class SelectedBuilding : MonoBehaviour {

	private Building m_Building;
    Projector projector;
    
    // Use this for initialization
    void Start () 
	{
        // Find projector
        projector = transform.Find("Projector").GetComponent<Projector>();        

        //Assign building
        m_Building = GetComponent<Building>();
	}
	
	public void SetSelected()
	{
        //Render projection
        if (projector.enabled == false)
            projector.enabled = true;

        // If building spots, show 'em
        if (GetComponent<BuildingSpotHandler>())
            GetComponent<BuildingSpotHandler>().ShowBuildingSpots();
	}
	
	public void SetDeselected()
	{
        // Stop rendering the projector
        if (projector.enabled == true)
        projector.enabled = false;

        // If building spots, don't show 'em
        if (GetComponent<BuildingSpotHandler>())
            GetComponent<BuildingSpotHandler>().HideBuildingSpots();
    }
	
	void OnDestroy()
	{
		//If this gets destroyed make sure to remove any selection
		SetDeselected ();
	}
}
