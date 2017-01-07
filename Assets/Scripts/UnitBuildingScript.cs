using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 
/// This script is placed in Manager gameobject and its purpose is to create
/// new units for the player.
/// 
/// Original version written by Armi of The Great Deep Blue and further (a lot actually) edited and developed by yours truly.
/// 
/// - Karl Sartorisio
/// The Great Deep blue
/// 
/// </summary>

public class UnitBuildingScript : MonoBehaviour {

	public List<GameObject> unitBuildingList;
	public int maxQueuedUnits = 10;
	public List<GameObject> unitBuildingQueue;
	private bool isAlreadyBuilding;
    private bool navalYardIsSet;
    private bool spawnPointFound;
    private bool unitReady;
    public bool onHold = false;
    public GameObject navalYard;
    public List<GameObject> spawnPointList = new List<GameObject>();
    private GameObject spawnPoint;
    public Transform unitPanel;

    // Building variables
    private cooldownfill buildCounter;
    private GameObject unitInBuilding;
    private float unitCost;
    private float unitBuildTime;
    private float timer;
    private float moneySpent = 0;
    private float timerDelta;

    private Manager m_Manager { get { return GetComponent<Manager>(); } }
    private SoundManager m_SoundManager { get { return GetComponent<SoundManager>(); } }

    // Update is called once per frame
    void Update ()
    {
        // Check for Naval Yard
        CheckNavalYard();

        // Do we have one?
        if (navalYard)
        {
            // Activate buttons
            ToggleButtonsActive();

            // Do we have units in build queue?
            if (unitBuildingQueue.Count > 0)
            {
                // Building is not on hold?
                if (!onHold)
                {
                    // Are we building already?
                    if (isAlreadyBuilding == false)
                    {
                        // Nope, so start building
                        StartBuilding();
                    }
                    else
                    {
                        // Check for funds
                        if (CheckFunds())
                        {
                            // Advance timer and spend cash
                            timer = SpendResourceAndBuild(timer, unitCost);

                            if (timer == 0)
                            {
                                // Spawn unit and stop building process
                                StartCoroutine(WaitAndBuild(unitInBuilding));
                                isAlreadyBuilding = false;
                            }
                        }
                    }
                }
            }
            // Can't find a spawn spot
            if (!FindSpawnSpot())
            {
                // Try again then, jesus
                FindSpawnSpot();
            }

            // Update queue text numbers on GUI
            UpdateQueueText();
        }
        else
        {
            // Keep buttons disabled until the program finds a naval yard
            ToggleButtonsDisabled();
        }
	}

    // Toggle buttons disabled --- TO BE REWORKED
    private void ToggleButtonsDisabled()
    {
        GameObject buttonMenu = GameObject.Find("UI").transform.Find("SideMenu").transform.Find("UnitPanel").gameObject;
        buttonMenu.transform.Find("DestroyerBtn").GetComponent<Button>().interactable = false;
        buttonMenu.transform.Find("FishingBoatBtn").GetComponent<Button>().interactable = false;
        buttonMenu.transform.Find("ScoutBtn").GetComponent<Button>().interactable = false;
        buttonMenu.transform.Find("DreadnoughtBtn").GetComponent<Button>().interactable = false;
    }

    // Toggle buttons active --- TO BE REWORDED
    private void ToggleButtonsActive()
    {
        GameObject buttonMenu = GameObject.Find("UI").transform.Find("SideMenu").transform.Find("UnitPanel").gameObject;
        buttonMenu.transform.Find("DestroyerBtn").GetComponent<Button>().interactable = true;
        buttonMenu.transform.Find("FishingBoatBtn").GetComponent<Button>().interactable = true;
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

    // Call this from a button
	public void BuildNewUnit(int unit)
    {
        CheckNavalYard();

        if (unitBuildingQueue.Count < maxQueuedUnits)
        {
            unitBuildingQueue.Add(unitBuildingList[unit]);
        }
	}

    // Get a Naval Yard to spawn from
    public bool CheckNavalYard()
    {        
        if (!navalYardIsSet)
        {
            navalYard = FindFriendlyNavalYard();
            if (navalYard != null && navalYard.tag == "Player1")
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
        else if (navalYardIsSet && navalYard != null)
        {
            return true;
        }
        else
        {
            if (buildCounter)
            {
                buildCounter.ClearFill();
                buildCounter = null;
            }

            unitBuildingQueue.Clear();
            spawnPointList.Clear();
            isAlreadyBuilding = false;
            timer = 0;
            unitCost = 0;
            moneySpent = 0;
            unitInBuilding = null;
            onHold = false;
            navalYardIsSet = false;
            return false;
        }
	}

    private GameObject FindFriendlyNavalYard()
    {
        GameObject nyard = null;
        GameObject[] nyardos = FindObjectsOfType<GameObject>();

        foreach (GameObject go in nyardos)
        {
            if (go.GetComponent<NavalYard>() && go.tag == "Player1")
            {
                nyard = go;
            }
        }

        return nyard;
    }

    // Set spawning spots
    private void SetSpawnSpots()
    {
        spawnPointList.Add(navalYard.transform.GetChild(0).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(1).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(2).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(3).gameObject);
    }

    // Find a spawning spot
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

    // Start building the first unit in queue
	public void StartBuilding()
    {
        // Pick the first one in queue and get its data
        unitInBuilding = unitBuildingQueue[0];
        Item unitItem = ItemDB.AllItems.Find(x => x.Name.Equals(unitInBuilding.name));

        unitCost = unitItem.Cost;
        unitBuildTime = unitItem.BuildTime;
        timer = unitBuildTime;

        // Get the respective button cooldownfill
        int unitIndex = unitBuildingList.FindIndex(x => x.name.Equals(unitItem.Name));
        buildCounter = GetCoolDownFill(unitIndex);

        if (isAlreadyBuilding == false)
        {
            isAlreadyBuilding = true;
        }
	}

    // Spawn the unit. No longer actually waits for anything, just didn't bother to change its name or type
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
            m_SoundManager.PlaySpawnSound(unit.name, unit.transform.position);

            // Clear temporary data
            unitBuildingQueue.RemoveAt(0);
            buildCounter.ClearFill();
            buildCounter = null;
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
    
    // Calculate a certain amount of resource spent in the time between last and current tick
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

        buildCounter.SetCoolDownFill(unitBuildTime, timer);
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

    // Updates queue number text
    private void UpdateQueueText()
    {
        // Counters for respective boats in queue
        int destroyers = 0;
        int fishingboats = 0;
        int scouts = 0;
        int dreadnoughts = 0;

        if (unitBuildingQueue.Count > 0)
        {
            // Add each unit to their respective counters
            foreach (GameObject unit in unitBuildingQueue)
            {
                switch (unit.name)
                {
                    case "Destroyer":
                        destroyers++;
                        break;
                    case "Fishing Boat":
                        fishingboats++;
                        break;
                    case "Scout":
                        scouts++;
                        break;
                    case "Dreadnought":
                        dreadnoughts++;
                        break;
                }
            }

            // Text accessors
            Text text = unitPanel.Find("DestroyerBtn").transform.Find("Text").GetComponent<Text>();
            Text text1 = unitPanel.Find("FishingBoatBtn").transform.Find("Text").GetComponent<Text>();
            Text text2 = unitPanel.Find("ScoutBtn").transform.Find("Text").GetComponent<Text>();
            Text text3 = unitPanel.Find("DreadnoughtBtn").transform.Find("Text").GetComponent<Text>();

            // Return counters
            if (destroyers > 0)
            {
                text.text = destroyers.ToString("");
            }
            else
            {
                text.text = "";
            }

            if (fishingboats > 0)
            {
                text1.text = fishingboats.ToString("");
            }
            else
            {
                text1.text = "";
            }

            if (scouts > 0)
            {
                text2.text = scouts.ToString("");
            }
            else
            {
                text2.text = "";
            }

            if (dreadnoughts > 0)
            {
                text3.text = dreadnoughts.ToString("");
            }
            else
            {
                text3.text = "";
            }
            
        }
        else
        {
            // Nothing in queue, return blank
            Text text = unitPanel.Find("DestroyerBtn").transform.Find("Text").GetComponent<Text>();
            text.text = "";

            Text text1 = unitPanel.Find("FishingBoatBtn").transform.Find("Text").GetComponent<Text>();
            text1.text = "";

            Text text2 = unitPanel.Find("ScoutBtn").transform.Find("Text").GetComponent<Text>();
            text2.text = "";

            Text text3 = unitPanel.Find("DreadnoughtBtn").transform.Find("Text").GetComponent<Text>();
            text3.text = "";
        }
    }

    // Find the correct button cooldownfill
    private cooldownfill GetCoolDownFill(int index)
    {
        // Cooldownfill for visual build counter
        cooldownfill fill = new cooldownfill();

        // Unit menu in UI
        Transform buttonMenu = GameObject.Find("UI").transform.Find("SideMenu").transform.Find("UnitPanel");
        //GameObject buttonMenu = unitPanel.gameObject;
        switch (index)
        {
            case 0:
                fill = buttonMenu.transform.Find("DestroyerBtn").transform.Find("Cooldown").GetComponent<cooldownfill>();
                break;
            case 1:
                fill = buttonMenu.transform.Find("FishingBoatBtn").transform.Find("Cooldown").GetComponent<cooldownfill>();
                break;
            case 2:
                fill = buttonMenu.transform.Find("ScoutBtn").transform.Find("Cooldown").GetComponent<cooldownfill>();
                break;
            case 3:
                fill = buttonMenu.transform.Find("DreadnoughtBtn").transform.Find("Cooldown").GetComponent<cooldownfill>();
                break;
        }

        return fill;
    }

    // Deletes last in queue and finally all temp data if queue hits zero
    public void DeleteLastInQueue()
    {
        unitBuildingQueue.RemoveAt(unitBuildingQueue.Count - 1);

        if (unitBuildingQueue.Count == 0)
        {
            buildCounter.ClearFill();
            buildCounter = null;
            isAlreadyBuilding = false;
            timer = 0;
            unitCost = 0;
            moneySpent = 0;
            unitInBuilding = null;
            onHold = false;
        }
    }
}
