using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  
/// Basic AI behaviour script
/// 
/// Inherits the AI Core, it is tasked to defend against the player and destroy their base.
/// As the name states, this AI isn't very sophisticated at all, it will just do the basic shit. 
/// Maybe if we had more time to develop and if the developer (yours truly) wasn't so lazy at times,
/// maybe, just maybe, it could've become something fancier.
/// 
/// Oh well.
/// 
/// - Karl Sartorisio
/// 
/// </summary>

public class BasicAI : AICore {

    // List for possible spawn points
    private List<GameObject> spawnPointList = new List<GameObject>(); // For units
    private List<GameObject> buildSpotList = new List<GameObject>(); // For buildings

    // Public lists for buildings and units
    public List<GameObject> buildableUnits = new List<GameObject>();
    public List<GameObject> buildableBuildings = new List<GameObject>();

    // Unit cost variables
    public int lightUnitCost;
    public int mediumUnitCost;
    public int heavyUnitCost;

    // Booleans for building
    private bool buildingBuilding = false;
    private bool buildingUnit = false;

    // Squad variables
    private Squad selectedSquad;

    // Squad priority, should either be "Kill", "Defend" or "Patrol" as of now
    public SquadPriority squadPriority = SquadPriority.Defend;

    // Behavior timer variables
    public float taskTimerInterval;
    private float timer = 0;

    // Use this for initialization
	private void Start ()
    {
        if (!baseBuilding)
        {
            baseBuilding = GameObject.Find("Player2").GetComponent<RTSEntity>();
            /*
            RTSEntity[] getBases = FindObjectsOfType<FloatingFortress>();

            foreach (RTSEntity basebuild in getBases)
            {
                if (basebuild.tag == "Player2")
                {
                    baseBuilding = basebuild;
                    break;
                }
            }*/
        }

        // if enemy base is not set, find one
        if (!enemyBase)
        {
            enemyBase = GameObject.Find("Player1").GetComponent<RTSEntity>();
        }

        FindExistingBuildings();
        FindExistingUnits();
	}
	
	// Update is called once per frame 
	private void Update ()
    { 
        // Advance timer
        timer = AdvanceTimer(timer);

        // When timer reaches interval time, AI will do what it is supposed to do
        if (timer >= taskTimerInterval)
        {
            // Restart timer
            timer = 0;

            // If there are no Naval Yards, build one by fetching the corresponding Item and by finding a building spot
            if (navalYards.Count < maxNavalYards)
            {
                Item newNavalYard = GetItem(buildableBuildings[0]);
                GameObject buildingSpot = GetBuildingSpot();

                if (buildingSpot && !buildingBuilding)
                {
                    buildingBuilding = true;
                    StartCoroutine(BuildBuilding(newNavalYard, 0, buildingSpot, newNavalYard.BuildTime));
                }
            }

            // Select behaviour based on threshold level.
            switch (currentThreshold)
            {
                case Threshold.StandBy:
                    StandByBehavior();
                    break;
                case Threshold.Low:
                    LowThreatBehaviour();
                    break;
                case Threshold.High:
                    HighThreatBehaviour();
                    break;
                case Threshold.BaseUA:
                    BaseUABehaviour();
                    break;
            }
        }
    }

    // StandBy behaviour, doesn't do moves and only builds if necessary. 25% resources in use.
    private void StandByBehavior()
    {
        float usableResources = maxResources * 0.25f;
        Debug.Log("BasicAI: Behaviour mode: StandBy");

        // Building actions

        // Constructions

        // Units
        // Only build if not all resources in use
        if (resourcesInUse < usableResources)
        {
            Debug.Log("BasicAI: We can use resources");

            // Create variables the AI needs to use within this loop
            string unitToBuild = "None"; // AI uses this to determine if it needs to build a specific type of unit. If not, squads are filled and maybe a new one is needed
            
            if (totalSquads.Count > 0)
            {
                Debug.Log("BasicAI: We have " + totalSquads.Count + " squads");

                foreach (Squad squad in totalSquads)
                {
                    if (squad.IsFull() == false)
                    {
                        if (squad.HasEnoughHeavyUnits() == false && (usableResources - resourcesInUse) > heavyUnitCost)
                        {
                            // Choose heavy unit to build
                            Debug.Log("BasicAI: Building Heavy Unit");
                            selectedSquad = squad;
                            unitToBuild = "Heavy";
                            break;
                        }

                        if (squad.HasEnoughMediumUnits() == false && (usableResources - resourcesInUse) > mediumUnitCost)
                        {
                            // Choose medium unit to build
                            Debug.Log("BasicAI: Building Medium Unit");
                            selectedSquad = squad;
                            unitToBuild = "Medium";
                            break;
                        }
                        if (squad.HasEnoughLightUnits() == false && (usableResources - resourcesInUse) > lightUnitCost)
                        {
                            // Choose medium unit to build
                            Debug.Log("BasicAI: Building Light Unit");
                            selectedSquad = squad;
                            unitToBuild = "Light";
                            break;
                        }
                    }
                }               
            }
            // If previous loop hasn't determined a unit to build, check squads and form a new squad is possible     
            if (unitToBuild == "None")
            {
                Debug.Log("BasicAI: No squads are free");

                bool looping = true;
                // Do a loop to figure out a squad to create
                //while (looping)
                //{
                    if (totalSquads.Count < (maxKillSquads + maxDefendSquads))
                    {
                        // Priority is Defend
                        if (squadPriority == SquadPriority.Defend)
                        {
                            if (defendSquads.Count < maxDefendSquads)
                            {
                                CreateSquad(SquadPriority.Defend);
                                looping = false;
                            }

                            if (killSquads.Count < maxKillSquads)
                            {
                                CreateSquad(SquadPriority.Kill);
                                looping = false;
                            }

                            if (patrolSquads.Count < maxPatrolSquads)
                            {
                                CreateSquad(SquadPriority.Patrol);
                                looping = false;
                            }
                        } // Priority is Kill
                        else if (squadPriority == SquadPriority.Kill)
                        {
                            if (killSquads.Count < maxKillSquads)
                            {
                                CreateSquad(SquadPriority.Kill);
                                looping = false;

                            }

                            if (defendSquads.Count < maxDefendSquads)
                            {
                                CreateSquad(SquadPriority.Defend);
                                looping = false;
                            }

                            if (patrolSquads.Count < maxPatrolSquads)
                            {
                                CreateSquad(SquadPriority.Patrol);
                                looping = false;
                            }
                        }  // Priority is Patrol
                        else if (squadPriority == SquadPriority.Patrol)
                        {
                            if (patrolSquads.Count < maxPatrolSquads)
                            {
                                CreateSquad(SquadPriority.Patrol);
                                looping = false; ;
                            }

                            if (defendSquads.Count < maxDefendSquads)
                            {
                                CreateSquad(SquadPriority.Defend);
                                looping = false;
                            }

                            if (killSquads.Count < maxKillSquads)
                            {
                                CreateSquad(SquadPriority.Kill);
                                looping = false;
                            }
                        }
                        else
                        {
                            looping = false;
                        }
                    //}
                }
            }
            else 
            // We have a squad slot, build a unit
            {
                if (FindSpawnSpot() != null)
                {
                    if (!buildingUnit)
                    {
                        switch (unitToBuild)
                        {
                            case "Light":
                                Debug.Log("BasicAI: Build Light Unit");
                                StartCoroutine(BuildUnit(buildableUnits[0], FindSpawnSpot(), selectedSquad, GetItem(buildableUnits[0]).BuildTime));
                                break;
                            case "Medium":
                                Debug.Log("BasicAI: Build Medium Unit");
                                StartCoroutine(BuildUnit(buildableUnits[0], FindSpawnSpot(), selectedSquad, GetItem(buildableUnits[0]).BuildTime));
                                break;
                            case "Heavy":
                                StartCoroutine(BuildUnit(buildableUnits[0], FindSpawnSpot(), selectedSquad, GetItem(buildableUnits[0]).BuildTime));
                                Debug.Log("BasicAI: Build Heavy Unit");
                                break;
                        }
                    }
                }
            }
        }

        // Command actions  
        // None     

    }

    // Low threat behaviour, only does defensive moves and builds if necessary, 50% resources in use.
    private void LowThreatBehaviour()
    {
        float usableResources = maxResources * 0.50f;
        Debug.Log("BasicAI: Behaviour mode: Low");
    }

    // High threat behaviour, does both defensive and attack moves, builds if necessary. 75% resources in use.
    private void HighThreatBehaviour()
    {
        float usableResources = maxResources * 0.75f;
        Debug.Log("BasicAI: Behaviour mode: High");

    }

    // Highest threat behaviour, furiously defends the base building with everything they've got. 100% resources in use.
    private void BaseUABehaviour()
    {
        float usableResources = maxResources * 1;
        Debug.Log("BasicAI: Behaviour mode: BaseUnderAttack");
    }

    private void CalculatePriority()
    {
        
    }

    // Specific attack order
    private void AttackOrder(RTSEntity target, Squad squad)
    {

    }

    // Aggressive move order
    private void AttackMoveOrder(Vector3 location, Squad squad)
    {

    }

    // Plain move order
    private void MoveOrder(Vector3 location, Squad squad)
    {
        foreach (Unit unit in squad.units)
        {

        }
    }

    // Defensive move order
    private void DefensiveMoveOrder(Vector3 location, Squad squad)
    {

    }

    // Find closest turret to enemy and return a defend move position in defend move scenario
    private Vector3 GetClosestTurretPosition()
    {
        Vector3 target = new Vector3();
        return target;
    }

    // Find enemy attack move position in attack move scenario
    private Vector3 GetEnemyBaseAttackPosition()
    {
        Vector3 target = new Vector3();
        return target;
    }

    // Find the defend move position in BaseUA scenario
    private Vector3 GetOwnBaseDefendPosition()
    {
        Vector3 target = new Vector3();
        return target;
    }

    // Building gets destroyed, what do we do?
    public void BuildingDestroyed(Building building)
    {

    }

    // A unit gets destroyed. Find out the lists it is in and update them.
    public void UnitDestroyed(Unit unit)
    {

    }

    // Find all the existing units
    private void FindExistingUnits()
    {
        Unit[] unitarray;
        unitarray = FindObjectsOfType<Unit>();

        foreach (Unit unit in unitarray)
        {
            if (unit.tag == "Player2")
            {
                AssignUnitToList(unit);
                AssignUnitToSquadByTotal(unit);
            }
        }        
    }

    // Create squad and set some details
    private void CreateSquad(SquadPriority priority)
    {
        if (priority == SquadPriority.Defend)
        {
            Squad newSquad = new Squad();
            totalSquads.Add(newSquad);
            defendSquads.Add(newSquad);
            newSquad.SetSquadType(priority.ToString());
            newSquad.SetMaxNumberOfUnits(maxUnitsInSquad, maxLightUnitsInSquad, maxMediumUnitsInSquad, maxHeavyUnitsInSquad);

            Debug.Log("BasicAI: Create " + priority.ToString() + " Squad");
            return;
        }

        if (priority == SquadPriority.Kill)
        {
            Squad newSquad = new Squad();
            totalSquads.Add(newSquad);
            killSquads.Add(newSquad);
            newSquad.SetSquadType(priority.ToString());
            newSquad.SetMaxNumberOfUnits(maxUnitsInSquad, maxLightUnitsInSquad, maxMediumUnitsInSquad, maxHeavyUnitsInSquad);

            Debug.Log("BasicAI: Create " + priority.ToString() + " Squad");
            return;
        }

        if (priority == SquadPriority.Patrol)
        {
            Squad newSquad = new Squad();
            totalSquads.Add(newSquad);
            patrolSquads.Add(newSquad);
            newSquad.SetSquadType(priority.ToString());
            newSquad.SetMaxNumberOfUnits(maxUnitsInSquad, maxLightUnitsInSquad, maxMediumUnitsInSquad, maxHeavyUnitsInSquad);

            Debug.Log("BasicAI: Create " + priority.ToString() + " Squad");
            return;
        }
    }

    // Assign unit to lists accordingly
    private void AssignUnitToList(Unit unit)
    {
        totalUnits.Add(unit);

        if (unit.UnitType == Const.UNIT_Light)
        {
            lightUnits.Add(unit);
        }
        else if (unit.UnitType == Const.UNIT_Medium)
        {
            mediumUnits.Add(unit);
        }
        else if (unit.UnitType == Const.UNIT_Heavy)
        {
            heavyUnits.Add(unit);
        }
    }

    // First search for a squad which has an empty slot for the type of unit queried then add it into the list
    private void AssignUnitToSquadByTotal(Unit unit)
    {
        foreach (Squad squad in totalSquads)
        {
            if (squad.GetCountOfUnits() < squad.maxNumberOfUnits)
            {
                int unitType = unit.UnitType;
                switch (unitType)
                {
                    case Const.UNIT_Light:
                        if (squad.GetCountOfLightUnits() < squad.maxNumberOfLightUnits)
                        {
                            squad.AddUnit(unit);
                            return;
                        }
                        break;
                    case Const.UNIT_Medium:
                        if (squad.GetCountOfMediumUnits() < squad.maxNumberOfMediumUnits)
                        {
                            squad.AddUnit(unit);
                            return;
                        }
                        break;
                    case Const.UNIT_Heavy:
                        if (squad.GetCountOfHeavyUnits() < squad.maxNumberOfHeavyUnits)
                        {
                            squad.AddUnit(unit);
                            return;
                        }
                        break;
                }
            }
        }
    }

    // Adds unit into specific squad
    private void AssingUnitToSquadBySquad(Unit unit, Squad squad)
    {
        squad.AddUnit(unit);
    }
    
    // Find all existing buildings at start and list all the buildings AI owns
    private void FindExistingBuildings()
    {
        Building[] buildarray;
        buildarray = FindObjectsOfType<Building>();

        foreach (Building building in buildarray)
        {
            if (building.tag == "Player2")
            {
                AssignBuildingToList(building);

                if (building.name == "Naval Yard")
                {
                    SetSpawnSpotsByNavalYard(building.GetComponent<NavalYard>());
                }
            }
        }
    }

    // Assign building to lists accordingly
    private void AssignBuildingToList(Building building)
    {
        totalBuildings.Add(building);

        if (building.GetComponent<Turret>())
        {
            turrets.Add(building);
        }
        else if (building.GetComponent<NavalYard>())
        {
            navalYards.Add(building);
        }
    }

    // Set spawning spots via Naval Yard
    private void SetSpawnSpotsByNavalYard(NavalYard navalYard)
    {
        spawnPointList.Add(navalYard.transform.GetChild(0).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(1).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(2).gameObject);
        spawnPointList.Add(navalYard.transform.GetChild(3).gameObject);
    }

    // Set spawning spot via different object than Naval Yard
    private void SetSpawnSpotsByZoneObject(GameObject zone)
    {
        spawnPointList.Add(zone);
    }

    // Find a spawning spot
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

    // Find Item from ItemDB by GameObject name
    private Item GetItem(GameObject obj)
    {
        Item item = ItemDB.AllItems.Find(x => x.Name.Equals(obj.name));
        return item;
    }

    // Find a building spot from list
    private GameObject GetBuildingSpot()
    {
        GameObject newSpot;
        foreach (GameObject spot in buildSpotList)
        {
            if (spot.activeSelf)
            {
                newSpot = spot;
                return newSpot;
            }
        }
        return null;
    }

    // Build a unit
    IEnumerator BuildUnit(GameObject unit, GameObject spot, Squad squad, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        //Fetch item
        Item item = GetItem(unit);

        resourcesInUse += item.Cost;

        // Create a new spawn point from assigned spawn spot
        Vector3 newPos = new Vector3(
            spot.transform.position.x,
            1.1f, 
            spot.transform.position.y);

        // Also a new rotation position
        Quaternion spawnRot = new Quaternion(
            Quaternion.identity.x,
            90f,
            Quaternion.identity.z,
            Quaternion.identity.w);

        // Instantiate new unit
        GameObject newUnit = Instantiate(unit, newPos, spawnRot) as GameObject;
        newUnit.tag = "Player2";

        // Add unit to squad
        squad.AddUnit(newUnit.GetComponent<Unit>());

        // List this unit appropriately
        totalUnits.Add(newUnit.GetComponent<Unit>());

        switch (item.UnitType)
        {
            case Const.UNIT_Light:
                lightUnits.Add(newUnit.GetComponent<Unit>());
                break;
            case Const.UNIT_Medium:
                mediumUnits.Add(newUnit.GetComponent<Unit>());
                break;
            case Const.UNIT_Heavy:
                heavyUnits.Add(newUnit.GetComponent<Unit>());
                break;
        }
    }

    // Build a building
    IEnumerator BuildBuilding(Item building, int index, GameObject spot, float seconds)
    {
        yield return new WaitForSeconds(seconds);  
          
        // Instantiate the building in the desired spot
        GameObject realBuilding = Instantiate(buildableBuildings[index], new Vector3(spot.transform.position.x, 1f, spot.transform.position.z), Quaternion.identity) as GameObject;
        realBuilding.name = building.Name;
        realBuilding.GetComponent<BoxCollider>().isTrigger = false;

        // Make sure that when the building is destroyed, it will free the building spot
        realBuilding.AddComponent<BuildingOnDestroy>();
        realBuilding.GetComponent<BuildingOnDestroy>().MyBuildingSpot = spot;

        // Add cost to resources in use
        resourcesInUse += building.Cost;
    }

    // Advance behavior timer
    private float AdvanceTimer(float seconds)
    {
        float newTime = seconds;
        newTime += Time.deltaTime;
        return newTime;
    }
}

public enum SquadPriority
{
    Defend,
    Kill,
    Patrol
}
