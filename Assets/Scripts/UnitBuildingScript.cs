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
    private bool unitReady;
    public GameObject navalYard;
    public List<GameObject> spawnPointList = new List<GameObject>();
    private Func<bool> meinDel;
    private GameObject spawnPoint;

    private Manager m_Manager { get { return GetComponent<Manager>(); } }

    // Use this for initialization
    void Start ()
    {
        meinDel = () => spawnPointFound;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (unitBuildingQueue.Count > 0)
        {
			StartBuilding();            
		}

        if (!FindSpawnSpot())
        {
            FindSpawnSpot();
        }
	}

	public void BuildNewUnit(int unit)
    {
        CheckNavalYard();

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

	public bool CheckFunds(float cost)
    {
        if (m_Manager.Resources >= cost)
        {
            return true;
        }
        else
        {
            return false;
        }
	}

	public void StartBuilding()
    {
        GameObject newUnit = unitBuildingList[0];
        Item unitItem = ItemDB.AllItems.Find(x => x.Name.Equals(newUnit.name));
        Debug.Log(unitItem.Name);
        if (isAlreadyBuilding == false)
        {
            if (CheckFunds(unitItem.Cost))
            {
                m_Manager.RemoveResource(unitItem.Cost);
                isAlreadyBuilding = true;
                StartCoroutine(WaitAndBuild(unitItem.BuildTime, unitItem.Cost, unitBuildingList[0]));
            }
        }
	}

    IEnumerator WaitAndBuild(float seconds, float cost, GameObject unit)
    {
        //StartCoroutine(SpendResourceAndBuild(seconds, cost));
        yield return new WaitForSeconds(seconds);
        spawnPoint = FindSpawnSpot();

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
            isAlreadyBuilding = false;
            StartBuilding();
        }
	}
    
    IEnumerator SpendResourceAndBuild(float seconds, float cost)
    {
        float timer = seconds * Time.deltaTime;
        float timeAfterLastTick = 0;
        float timeBetweenTicks = 0;
        float moneySpentThisTick = 0;

        while (timer > 0)
        {
            timeAfterLastTick = timer;
            timeBetweenTicks = timeAfterLastTick - timer;
            moneySpentThisTick = cost / timeBetweenTicks;
            m_Manager.RemoveResource(moneySpentThisTick);
            timer -= Time.deltaTime;
        }

        yield return new WaitUntil(() => timer <= 0);
        unitReady = true;
    }
}
