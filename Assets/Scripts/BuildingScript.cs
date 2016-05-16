using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// This script is placed in Manager gameobject and its purpose is to create
/// new buildings for the player.
/// 
/// Originally written by Armi of The Great Deep Blue and further edited and implemented by yours truly.
/// 
/// - Karl Sartorisio
/// The Great Deep blue
/// </summary>

public class BuildingScript : MonoBehaviour {

	public GameObject[] buildingList; //List of buildings to be built
	private GameObject currentBuilding; //Current building gameobject to be built
	private GameObject tempBuilding; //Temporary building gameobject as it is being built
	public GameObject currentBuildingSpot; //Current spot where building is being placed
	private int buildingListIndex; //The index number of the building from building list
	public Camera camera; //The main camera being used
	public LayerMask layerMask; //Layer mask for raycasting
	public Texture[] textures; //Textures being used for buildings being placed
    public BuildingSpotHandler bsHandler; // Handles building spots
    private Manager m_Manager { get { return GetComponent<Manager>(); } } // Manager reference
    private bool onHold = false;
    private bool buildingInProgress = false;

    // Building variables
    private GameObject constInBuilding;
    private float buildCost;
    private float buildTime;
    private float timer;
    private float moneySpent = 0;
    private float timerDelta;


    // Use this for initialization
    void Start ()
    {
        camera = Camera.main;
        bsHandler = GameObject.Find("Player1").GetComponent<BuildingSpotHandler>();
	}
	
	// Update is called once per frame
	void Update ()
    { 
        // If we are currently building a structure
        if (buildingInProgress && !onHold)
        {
            // Check funds
            if (CheckFunds())
            {
                // Spend some money this tick and advance timer accordingly
                timer = SpendResourceAndBuild(timer, buildCost);
                if (timer == 0)
                {
                    // Building is ready, just finish it
                    StartCoroutine(WaitAndBuild(currentBuildingSpot.transform.position));
                }
            }
        }
        //If we have a current building being placed
        else if (currentBuilding){

            // Show available building spots
            bsHandler.ShowBuildingSpots();

            // Raycast info
			RaycastHit hit;
	        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            // Set mouse position information
			Vector3 mousePos = Input.mousePosition;
			mousePos = new Vector3(mousePos.x,mousePos.y,camera.transform.position.y);
			Vector3 objectPos = camera.ScreenToWorldPoint(mousePos);

			//When mouse button is pushed outside the building area, the current building will be destroyed
			if (Input.GetMouseButton(1))
            {
				Destroy (currentBuilding);
                bsHandler.HideBuildingSpots();
			}
            
			//Casting ray which only hits the colliders in the layer mask
	        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide))
            {
	        	//If the ray hits the Terrain, it will drag the building object on top of it and underneath the mouse cursor
	        	if (hit.transform.gameObject.tag != "BuildingSpot")
                {
					Vector3 target = new Vector3(hit.point.x, hit.point.y + 1.5f, hit.point.z);
                	currentBuilding.transform.position = target;
					currentBuilding.GetComponentInChildren<Renderer>().material.mainTexture = textures[0];
					currentBuilding.GetComponentInChildren<Renderer>().material.color = Color.red;

                	//If a building spot is hit with the ray, the building will turn green and snap to place *SCRAP THAT*
                    //Actually, check if temporary building's Building Being Placed -component collides with anything called BuildingSpot
                	if (hit.transform.IsChildOf(GameObject.Find("Player1").transform) /*currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject && currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject.tag == "BuildingSpot"*/)
                    {
                        currentBuildingSpot = currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject;
                        //currentBuildingSpot = hit.transform.gameObject;
                		currentBuilding.GetComponentInChildren<Renderer>().material.mainTexture = textures[0];
						currentBuilding.GetComponentInChildren<Renderer>().material.color = Color.green;
						currentBuilding.transform.position = currentBuildingSpot.transform.position;
                        
						//If mouse button is pressed on top of a building spot, the object is destroyed and instantiated as a temporary one and turned gray, then calling the timer coroutine
						if (Input.GetMouseButton(0))
                        {
							Destroy (currentBuilding);
							tempBuilding = Instantiate(buildingList[buildingListIndex], currentBuildingSpot.transform.position, Quaternion.identity) as GameObject;
                            //tempBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
                            //tempBuilding.GetComponent<Renderer>().material.color = Color.gray;
                            //Destroy(tempBuilding.GetComponent<Rigidbody>());
                            //tempBuilding.GetComponent<BoxCollider>().isTrigger = false;
                            currentBuildingSpot.SetActive(false);
                            bsHandler.HideBuildingSpots();
                            buildingInProgress = true;
                        }

                        if (Input.GetMouseButton(1))
                        {
                            Destroy(currentBuilding);
                            bsHandler.HideBuildingSpots();
                        }
					}
	        	}
	        } 
		}
	}

	//The building function being called by the GUI button
	public void buildingFunction (int buildingIndex)
    {
        // Create a temporary building for placing purposes
		currentBuilding = Instantiate(buildingList[buildingIndex]) as GameObject;
        currentBuilding.AddComponent<BuildingBeingPlaced>();
        Destroy(currentBuilding.GetComponent<HPBar>());
        Destroy(currentBuilding.GetComponent<RTSEntity>());
        currentBuilding.GetComponent<Collider>().isTrigger = true;        
		buildingListIndex = buildingIndex;

        // Get some sweet data
        GetBuildingData(buildingList[buildingListIndex]);
    }

    //Timer function for when the building is being built
    IEnumerator WaitAndBuild(Vector3 spot)
    {
        // Wait for the building to build itself
        yield return new WaitForSeconds(0);

        // Pick the temporary building's spot and destroy the temp building
        spot = tempBuilding.transform.position;
        Destroy(tempBuilding);

        // Instantiate the real building where the temporary building used to be
        GameObject realBuilding = Instantiate(buildingList[buildingListIndex], new Vector3(spot.x, 1f, spot.z), Quaternion.identity) as GameObject;
        realBuilding.name = buildingList[buildingListIndex].name;
        realBuilding.GetComponent<BoxCollider>().isTrigger = false;

        // Make sure that when the building is destroyed, it will free the building spot
        realBuilding.AddComponent<BuildingOnDestroy>();
        realBuilding.GetComponent<BuildingOnDestroy>().MyBuildingSpot = currentBuildingSpot;

        // Stop building
        timer = 0;
        buildCost = 0;
        moneySpent = 0;
        buildingInProgress = false;
    }

    // Get building's information and assign them to local variables
    private void GetBuildingData(GameObject building)
    {
        constInBuilding = building;
        Item unitItem = ItemDB.AllItems.Find(x => x.Name.Equals(constInBuilding.name));

        buildCost = unitItem.Cost;
        buildTime = unitItem.BuildTime;
        timer = buildTime;
    }

    // Toggle On Hold
    public void ToggleOnHold()
    {
        if (onHold)
        {
            onHold = false;
        }
        else
        {
            onHold = true;
        }
    }

    // Check funds
    public bool CheckFunds()
    {
        if (m_Manager.Resources >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Spends x amount of money this tick
    private float SpendResourceAndBuild(float seconds, float cost)
    {
        float newtimer = seconds;
        float timeBetweenTicks;
        float moneySpentThisTick;

        newtimer -= Time.deltaTime;

        if (newtimer <= 0)
        {
            newtimer = 0;
        }

        timeBetweenTicks = (seconds - newtimer);
        moneySpentThisTick = (cost * timeBetweenTicks) / buildTime;

        moneySpent += moneySpentThisTick;

        if (newtimer == 0)
        {
            if (moneySpent > cost)
            {
                float difference = moneySpent - cost;
                Debug.Log("Last payment difference 1: " + difference);
                moneySpentThisTick -= difference;
                moneySpent -= difference;
            }
            else
            {
                float difference = cost - moneySpent;
                Debug.Log("Last payment difference 2: " + difference);
                moneySpentThisTick += difference;
                moneySpent += difference;
            }
        }

        m_Manager.RemoveResource(moneySpentThisTick);
        return newtimer;
    }
}
