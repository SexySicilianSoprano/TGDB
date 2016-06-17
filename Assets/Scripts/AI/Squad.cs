using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad {

    // List of units in this squad
    private string type;
    public List<Unit> units = new List<Unit>();
    private bool hasTask = false;
    private string task;
    private int maxNumberOfUnits;

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void SetType(string newtype)
    {
        type = newtype;
    }

    public bool GetTask()
    {
        return hasTask;
    }

    // Set the maximum amount of units this squad can have
    public void SetMaxNumberOfUnits(int value)
    {

    }

    // Set task. If it is false, AI may set a new task
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
}
