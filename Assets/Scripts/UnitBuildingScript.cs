using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitBuildingScript : MonoBehaviour {

	public GameObject[] unitBuildingList;
	public int maxQueuedUnits = 10;
	public List<GameObject> unitBuildingQueue;
	private bool isAlreadyBuilding;
    private bool navalYardIsSet;
    private bool spawnPointFound;
    public GameObject navalYard;
    public List<GameObject> spawnPointList = new List<GameObject>();
    private Func<bool> meinDel;
    private GameObject spawnPoint;

    // Use this for initialization
    void Start ()
    {
        meinDel = () => spawnPointFound;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (unitBuildingQueue.Count > 0){
			StartBuilding();
		}
        
        FindSpawnSpot();
	}

	public void BuildNewUnit(int unit)
    {
		CheckNavalYard();
		CheckFunds();
        if (unitBuildingQueue.Count < maxQueuedUnits)
        {
            unitBuildingQueue.Add(unitBuildingList[unit]);
        }
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
        else if (navalYardIsSet && GameObject.Find("NavalYard"))
        {
            return true;
        }
        else
        {
            navalYardIsSet = false;
            return false;
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
                spawnPointFound = true;
                return spot;
            }
        }
        spawnPointFound = false;
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

        spawnPoint = FindSpawnSpot();

        yield return new WaitUntil(() => spawnPointFound == true);
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
        else
        {
            WaitAndBuild(2, unitBuildingList[0]);
        }

	}
}
