using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Squad {

    // List of units in this squad
    private string type;
    private List<Unit> units = new List<Unit>();
    private bool hasTask = false;
    private string task;

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

    // Set task. If it is false, AI m
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
