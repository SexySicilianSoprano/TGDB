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

    // Squad priority, should either be "Kill", "Defend" or "Patrol" as of now
    public string squadPriority = "Defend";

    // Behavior timer variables
    public float taskTimerInterval;
    private float timer = 0;

    // Use this for initialization
	void Start ()
    {
        FindExistingBuildings();
        FindExistingUnits();
	}
	
	// Update is called once per frame 
	void Update ()
    { 
        // Advance timer
        timer = AdvanceTimer(timer);

        // When timer reaches interval time, AI will do what it is supposed to do
        if (timer >= taskTimerInterval)
        {
            // Restart timer
            timer = 0;
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
    private void StandByBehavior() { }

    // Low threat behaviour, only does defensive moves and builds if necessary, 50% resources in use.
    private void LowThreatBehaviour() { }

    // High threat behaviour, does both defensive and attack moves, builds if necessary. 75% resources in use.
    private void HighThreatBehaviour() { }

    // Highest threat behaviour, furiously defends the base building with everything they've got. 100% resources in use.
    private void BaseUABehaviour() { }

    // Specific attack order
    private void AttackOrder(RTSEntity target)
    {

    }

    // Aggressive move order
    private void AttackMoveOrder(Vector3 location)
    {

    }

    // Plain move order
    private void MoveOrder(Vector3 location)
    {

    }

    // Defensive move order
    private void DefensiveMove(Vector3 location)
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
    public void BuildingDestroyed()
    {

    }

    // Find all the existing
    private void FindExistingUnits()
    {
        Unit[] unitarray;
        unitarray = FindObjectsOfType<Unit>();

        foreach (Unit unit in unitarray)
        {
            if (unit.tag == "Player2")
            {
                AssignUnitToList(unit);
                AssignUnitToSquad(unit);
            }
        }        
    }

    // Assign unit to lists accordingly
    private void AssignUnitToList(Unit unit)
    {
        totalUnits.Add(unit);

        if (unit.GetComponent<Scout>())
        {
            lightUnits.Add(unit);
        }
        else if (unit.GetComponent<Destroyer>())
        {
            mediumUnits.Add(unit);
        }
        /*else if (unit.GetComponent<Dreadnought>())
        {
            heavyUnits.Add(unit);
        }*/
    }
    
    private void AssignUnitToSquad(Unit unit)
    {
        foreach (Squad squad in totalSquads)
        {
            if (squad.GetSquadType() == squadPriority)
            {


            }

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

    IEnumerator BuildUnit(Item unit, GameObject building, float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator BuildBuilding(Item building, GameObject spot, float seconds)
    {
        yield return new WaitForSeconds(seconds);    
    }

    // Advance behavior timer
    private float AdvanceTimer(float seconds)
    {
        float newTime = seconds;
        newTime += Time.deltaTime;
        return newTime;
    }
}
