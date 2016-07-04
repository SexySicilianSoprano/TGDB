using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// Squad object script
/// 
/// Used to create a squad for whatever purpose AI needs it to. 
/// Currently it can hold a type, a task and number of specific units and a total count of units.
/// AI can also send squads various commands, such as attack, move and patrol commands.
/// 
/// </summary>

public class Squad {

    // List of units in this squad
    private string type;
    public List<Unit> units = new List<Unit>();
    private bool hasTask = false;
    private string task;
    public int maxNumberOfUnits;
    public int maxNumberOfLightUnits;
    public int maxNumberOfMediumUnits;
    public int maxNumberOfHeavyUnits;

    // Add a specific unit into this squad
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    // Set the squad type, should be either Kill, Defend or Patrol
    public void SetSquadType(string newtype)
    {
        type = newtype;
    }

    // Call this to return this squad's type
    public string GetSquadType()
    {
        return type;
    }

    // Returns bool value of whether this squad has a task at hand
    public bool HasTask()
    {
        return hasTask;
    }

    // Returns the string task currently at hand, should be either Kill, Defend or Patrol
    public string GetTask()
    {
        return task;
    }

    // Get the count of units in this squad
    public int GetCountOfUnits()
    {
        return units.Count;
    }

    // Set the maximum amount of units this squad can have
    public void SetMaxNumberOfUnits(int max, int light, int medium, int heavy)
    {
        maxNumberOfUnits = max;
        maxNumberOfLightUnits = light;
        maxNumberOfMediumUnits = medium;
        maxNumberOfHeavyUnits = heavy;
    }

    // Returns a count of light units via RTSEntity's int UnitType variable
    public int GetCountOfLightUnits()
    {
        int number = 0;

        foreach (Unit unit in units)
        {
            if (unit.UnitType == Const.UNIT_Light)
            {
                number++;
            }
        }

        return number;
    }

    // Returns a count of medium units via RTSEntity's int UnitType variable
    public int GetCountOfMediumUnits()
    {
        int number = 0;

        foreach (Unit unit in units)
        {
            if (unit.UnitType == Const.UNIT_Medium)
            {
                number++;
            }
        }

        return number;
    }

    // Returns a count of heavy units via RTSEntity's int UnitType variable
    public int GetCountOfHeavyUnits()
    {
        int number = 0;

        foreach (Unit unit in units)
        {
            if (unit.UnitType == Const.UNIT_Heavy)
            {
                number++;
            }
        }

        return number;
    }

    // Set task. If it is false, AI may set a new task, should be either Kill, Defend or Patrol
    public void SetTask(bool set, string newTask)
    {
        hasTask = set;

        if (set == true)
        {
            task = newTask;
        }
        else
        {
            task = "";
        }
    }

    // Tell the squad to move to a specific location
    public void GiveSquadMoveOrder(Vector3 location)
    {

    }


    // Tell the squad to attack a specific target
    public void GiveSquadAttackOrder(RTSEntity target)
    {

    }

    // Tell the squad to follow a certain patrol path
    public void GiveSquadPatrolOrder(Vector3[] path)
    {

    }
}
