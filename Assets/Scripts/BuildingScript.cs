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
    public BuildingSpotHandler bsHandler;


	// Use this for initialization
	void Start ()
    {
        camera = Camera.main;
        bsHandler = GameObject.Find("Player1").GetComponent<BuildingSpotHandler>();
	}
	
	// Update is called once per frame
	void Update () {

		//If we have a current building being placed
		if (currentBuilding){

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
					currentBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
					currentBuilding.GetComponent<Renderer>().material.color = Color.red;

                	//If a building spot is hit with the ray, the building will turn green and snap to place *SCRAP THAT*
                    //Actually, check if temporary building's Building Being Placed -component collides with anything called BuildingSpot
                	if (hit.transform.IsChildOf(GameObject.Find("Player1").transform) /*currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject && currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject.tag == "BuildingSpot"*/)
                    {
                        currentBuildingSpot = currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject;
                        //currentBuildingSpot = hit.transform.gameObject;
                		currentBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
						currentBuilding.GetComponent<Renderer>().material.color = Color.green;
						currentBuilding.transform.position = currentBuildingSpot.transform.position;
                        
						//If mouse button is pressed on top of a building spot, the object is destroyed and instantiated as a temporary one and turned gray, then calling the timer coroutine
						if (Input.GetMouseButton(0))
                        {
							Destroy (currentBuilding);
							tempBuilding = Instantiate(buildingList[buildingListIndex], currentBuildingSpot.transform.position, Quaternion.identity) as GameObject;
                            tempBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
                            tempBuilding.GetComponent<Renderer>().material.color = Color.gray;
                            //Destroy(tempBuilding.GetComponent<Rigidbody>());
                            //tempBuilding.GetComponent<BoxCollider>().isTrigger = false;
                            StartCoroutine (WaitAndBuild(2, currentBuildingSpot.transform.position));
                            currentBuildingSpot.SetActive(false);

                            bsHandler.HideBuildingSpots();
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
	}

    //Timer function for when the building is being built
    IEnumerator WaitAndBuild(float seconds, Vector3 spot)
    {
        // Wait for the building to build itself
        yield return new WaitForSeconds(seconds);

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
    }
}
