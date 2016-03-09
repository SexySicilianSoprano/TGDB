using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBuildingScript : MonoBehaviour {

	public GameObject[] unitBuildingList;
	public int maxQueuedUnits = 10;
	public List<GameObject> unitBuildingQueue;
	private bool isAlreadyBuilding;
    private bool navalYardIsSet;
	public GameObject navalYard;
    public List<GameObject> spawnPointList = new List<GameObject>();
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (unitBuildingQueue.Count > 0){
			StartBuilding();
		}
	}

	public void BuildNewUnit(int unit)
    {
		CheckNavalYard();
		CheckFunds();
		unitBuildingQueue.Add(unitBuildingList[unit]);
	}

    public bool CheckNavalYard()
    {
        if (!navalYardIsSet)
        {
            navalYard = GameObject.Find("NavalYard");
            if (navalYard)
            {
                Debug.Log("Set naval yardo");
                SetSpawnSpots();
                navalYardIsSet = true;
                return true;
            }
            else {
                return false;
            }
        }
        else
        {
            return true;
        }
	}

    private void SetSpawnSpots()
    {
        spawnPointList.Add(navalYard.transform.GetChild(0).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(1).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(2).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(3).gameObject);
    }

    private GameObject FindSpawnSpot()
    {
        foreach (GameObject spot in spawnPointList)
        {
            if (!spot.GetComponent<BuildingSpotScript>().isOccupied)
            {
                return spot;
            }
        }
        return null;
    }

	public void CheckFunds()
    {

	}

	public void StartBuilding()
    {
		if (isAlreadyBuilding == false)
        {
			StartCoroutine(WaitAndBuild(2, unitBuildingList[0]));
			isAlreadyBuilding = true;
		}
	}

	IEnumerator WaitAndBuild(float seconds, GameObject unit)
    {
		yield return new WaitForSeconds(seconds);

        GameObject spawnPoint = FindSpawnSpot();
        if (spawnPoint != null)
        {            
            // Create a new spawn point
            Vector3 spawnPos = new Vector3(
                spawnPoint.transform.position.x,
                1.1f,
                spawnPoint.transform.position.z);

            // Also a new rotation position
            Quaternion spawnRot = new Quaternion(
                Quaternion.identity.x,
                90f,
                Quaternion.identity.z,
                Quaternion.identity.w);

            // Instantiate
            Instantiate(unit, spawnPos, spawnRot);
            //Instantiate(unit, navalYard.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
            unitBuildingQueue.RemoveAt(0);
            isAlreadyBuilding = false;
        }
	}
}
