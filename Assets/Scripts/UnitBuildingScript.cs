using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnitBuildingScript : MonoBehaviour {

	public GameObject[] unitBuildingList;
	public int maxQueuedUnits = 10;
	public List<GameObject> unitBuildingQueue;
	private bool isAlreadyBuilding;
    private bool navalYardIsSet;
    private bool spawnPointFound;
    private bool unitReady;
    private bool onHold = false;
    public GameObject navalYard;
    public List<GameObject> spawnPointList = new List<GameObject>();
    private GameObject spawnPoint;

    private GameObject unitInBuilding;
    private float unitCost;
    private float unitBuildTime;
    private float timer;
    private float moneySpent = 0;
    private float timerDelta;

    private Manager m_Manager { get { return GetComponent<Manager>(); } }

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (unitBuildingQueue.Count > 0)
        {
            if (!onHold)
            {
                if (isAlreadyBuilding == false)
                {
                    StartBuilding();
                }
                else
                {
                    if (CheckFunds())
                    {
                        timer = SpendResourceAndBuild(timer, unitCost);

                        if (timer == 0)
                        {
                            StartCoroutine(WaitAndBuild(unitInBuilding));
                            isAlreadyBuilding = false;
                        }
                    }
                }
            }
		}

        if (!FindSpawnSpot())
        {
            FindSpawnSpot();
        }

        UpdateQueueText();
	}

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

	public void StartBuilding()
    {
        unitInBuilding = unitBuildingQueue[0];
        Item unitItem = ItemDB.AllItems.Find(x => x.Name.Equals(unitInBuilding.name));

        unitCost = unitItem.Cost;
        unitBuildTime = unitItem.BuildTime;
        timer = unitBuildTime;

        if (isAlreadyBuilding == false)
        {
            //if (CheckFunds(unitItem.Cost))
            //{
                //m_Manager.RemoveResource(unitItem.Cost);
                isAlreadyBuilding = true;
            //}
        }
	}

    IEnumerator WaitAndBuild(GameObject unit)
    {
        //yield return new WaitForSeconds(seconds);
        spawnPoint = FindSpawnSpot();
        
        if (spawnPoint != null)
        {
            yield return new WaitForSeconds(0);
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
            timer = 0;
            unitCost = 0;
            moneySpent = 0;
            unitInBuilding = null;
        }
        else
        {
            Debug.Log("Aint no spot mate");
            WaitAndBuild(unit);
        }
	}
    
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
        moneySpentThisTick = (cost * timeBetweenTicks) / unitBuildTime;

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

        Debug.Log("Timer cost: " + moneySpent);

        m_Manager.RemoveResource(moneySpentThisTick);
        return newtimer;
    }

    /*
    private float SpendResourceAndBuild(float seconds, float cost)
    {
        float newtimer = seconds;
        float timeBetweenTicks = 1f;
        float moneyPerTick = cost / unitBuildTime;
        float timeAfterLastTick;

        newtimer -= Time.deltaTime;

        if (newtimer <= 0)
        {
            newtimer = 0;
        }

        timeAfterLastTick = (seconds - newtimer);
        timerDelta += timeAfterLastTick;

        if (timerDelta >= timeBetweenTicks || newtimer == 0)
        {
            m_Manager.RemoveResource(moneyPerTick);
            timerDelta = 0;
        }
        
        Debug.Log("Timer cost: " + moneySpent);

        return newtimer;
    } */

    private void UpdateQueueText()
    {
        if (unitBuildingQueue.Count > 0)
        {
            Text text = GameObject.Find("UI").transform.Find("SideMenu").transform.Find("UnitPanel").transform.Find("DestroyerBtn").transform.Find("Text (1)").GetComponent<Text>();
            text.text = unitBuildingQueue.Count.ToString();
        }
        else
        {
            Text text = GameObject.Find("UI").transform.Find("SideMenu").transform.Find("UnitPanel").transform.Find("DestroyerBtn").transform.Find("Text (1)").GetComponent<Text>();
            text.text = "";
        }
    }
}
