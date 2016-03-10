using UnityEngine;
using System.Collections;

public class BuildingScript : MonoBehaviour {

	public GameObject[] buildingList; //List of buildings to be built
	private GameObject currentBuilding; //Current building gameobject to be built
	private GameObject tempBuilding; //Temporary building gameobject as it is being built
	public GameObject currentBuildingSpot; //Current spot where building is being placed
	private int buildingListIndex; //The index number of the building from building list
	public Camera camera; //The main camera being used
	public LayerMask layerMask; //Layer mask for raycasting
	public Texture[] textures; //Textures being used for buildings being placed


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		//If we have a current building being placed
		if (currentBuilding){

			RaycastHit hit;
	        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			Vector3 mousePos = Input.mousePosition;
			mousePos = new Vector3(mousePos.x,mousePos.y,camera.transform.position.y);
			Vector3 objectPos = camera.ScreenToWorldPoint(mousePos);

			//When mouse button is pushed outside the building area, the current building will be destroyed
			if (Input.GetMouseButton(0)){
				Destroy (currentBuilding);
			}
            
			//Casting ray which only hits the colliders in the layer mask
	        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {

	        	//If the ray hits the Terrain, it will drag the building object on top of it and underneath the mouse cursor
	        	if (hit.transform.tag == "Terrain" )
                {
					Vector3 target = new Vector3(hit.point.x, hit.point.y + 1.5f, hit.point.z);
                	currentBuilding.transform.position = target;
					currentBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
					currentBuilding.GetComponent<Renderer>().material.color = Color.red;

                	//If a building spot is hit with the ray, the building will turn green and snap to place *SCRAP THAT*
                    //Actually, check if temporary building's Building Being Placed -component collides with anything called BuildingSpot
                	if (currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject == GameObject.Find("BuildingSpot"))
                    {
                        currentBuildingSpot = currentBuilding.GetComponent<BuildingBeingPlaced>().collidingObject;
                        //currentBuildingSpot = hit.transform.gameObject;
                		currentBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
						currentBuilding.GetComponent<Renderer>().material.color = Color.green;
						currentBuilding.transform.position = currentBuildingSpot.transform.position;

						//If mouse button is pressed on top of a building spot, the object is destroyed and instantiated as a temporary one and turned gray, then calling the timer coroutine
						if (Input.GetMouseButton(0)){
							Destroy (currentBuilding);
							tempBuilding = Instantiate(buildingList[buildingListIndex], currentBuildingSpot.transform.position, Quaternion.identity) as GameObject;
                            tempBuilding.GetComponent<Renderer>().material.mainTexture = textures[0];
                            tempBuilding.GetComponent<Renderer>().material.color = Color.gray;
                            //Destroy(tempBuilding.GetComponent<Rigidbody>());
                            //tempBuilding.GetComponent<BoxCollider>().isTrigger = false;
                            StartCoroutine (WaitAndBuild(2, currentBuildingSpot.transform.position));
                            currentBuildingSpot.SetActive(false);
						}
					}
	        	}
	        } 
		}
	}

	//The building function being called by the GUI button
	public void buildingFunction (int buildingIndex){
		currentBuilding = Instantiate(buildingList[buildingIndex]) as GameObject;
        currentBuilding.AddComponent<BuildingBeingPlaced>();
        //currentBuilding.GetComponent<SelectedBuilding>().enabled = false;
        currentBuilding.GetComponent<Collider>().isTrigger = true;
		buildingListIndex = buildingIndex;
	}

	//Timer function for when the building is being built
	IEnumerator WaitAndBuild(float seconds, Vector3 spot){
		yield return new WaitForSeconds(seconds);
        spot = tempBuilding.transform.position;
        Destroy (tempBuilding);
		GameObject realBuilding = Instantiate(buildingList[buildingListIndex], new Vector3(spot.x, 1f, spot.z), Quaternion.identity) as GameObject;
        realBuilding.name = buildingList[buildingListIndex].name;
        realBuilding.GetComponent<BoxCollider>().isTrigger = false;
    }
}
