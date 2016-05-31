using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectedBuilding : MonoBehaviour {

	private Building m_Building;
    public bool isSelected = false;
    Projector projector;
    HPBar hpbar;
    ShowCanvas sideMenu;
    
    // Use this for initialization
    void Start () 
	{
        // Find projector
        projector = transform.Find("Projector").GetComponent<Projector>();
        hpbar = GetComponent<HPBar>();
        sideMenu = GameObject.Find("UI").transform.Find("SideMenu").GetComponent<ShowCanvas>();    

        //Assign building
        m_Building = GetComponent<Building>();
	}
	
	public void SetSelected()
	{
        isSelected = true;
        //Render projection
        if (projector.enabled == false)
            projector.enabled = true;

        if (hpbar)
            hpbar.ShowHealthBar();

        // If building spots, show 'em
        /*
        if (GetComponent<BuildingSpotHandler>())
            GetComponent<BuildingSpotHandler>().ShowBuildingSpots();
        */
        if (GetComponent<FloatingFortress>())
        {
            if (sideMenu.constPanel.activeSelf == false)
            {
                sideMenu.ToggleCanvasConst();
            }
        }

        if (GetComponent<NavalYard>())
        {
            if (sideMenu.unitPanel.activeSelf == false)
            {
                sideMenu.ToggleCanvasUnits();
            }
        }
	}
	
	public void SetDeselected()
	{
        isSelected = false;
        // Stop rendering the projector
        if (projector.enabled == true)
            projector.enabled = false;

        if (hpbar)
            hpbar.HideHealthBar();

        // If building spots, don't show 'em
        /*
        if (GetComponent<BuildingSpotHandler>())
            GetComponent<BuildingSpotHandler>().HideBuildingSpots();
        */
        if (GetComponent<FloatingFortress>())
        {
            if (sideMenu.constPanel.activeSelf == true)
            {
                sideMenu.ToggleCanvasConst();
            }
        }

        if (GetComponent<NavalYard>())
        {
            if (sideMenu.unitPanel.activeSelf == true)
            {
                sideMenu.ToggleCanvasUnits();
            }
        }
    }
	
	void OnDestroy()
	{
		//If this gets destroyed make sure to remove any selection
		SetDeselected ();
	}
}
