using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectedManager : MonoBehaviour, ISelectedManager {

    // Singleton
    public static SelectedManager main;

    // Active selected list variables
    private List<RTSEntity> l_Selected = new List<RTSEntity>();
    private List<IOrderable> SelectedActiveEntities = new List<IOrderable>();

    // Grouping variables
    private List<int> ListOfGroups = new List<int>();
    private List<RTSEntity> l_Group1 = new List<RTSEntity>();
    private List<RTSEntity> l_Group2 = new List<RTSEntity>();
    private List<RTSEntity> l_Group3 = new List<RTSEntity>();
    private List<RTSEntity> l_Group4 = new List<RTSEntity>();
    private List<RTSEntity> l_Group5 = new List<RTSEntity>();
    private List<RTSEntity> l_Group6 = new List<RTSEntity>();

    // Selected building
    private RTSEntity selectedBuilding;

    void Awake()
    {
        main = this;
    }

    void Update()
    {
        Debug.Log(ActiveEntityCount());
    }

    // ### Selection functions ###

    // Adds the unit to selected
    public void AddToSelected(RTSEntity unit)
    {
        if (unit.GetComponent<Unit>())
        {
            if (!l_Selected.Contains(unit))
            {
                if (unit is IOrderable)
                {
                    SelectedActiveEntities.Add((IOrderable)unit);
                }

                l_Selected.Add(unit);
                unit.SetSelected();
            }
        }
        else
        {
            unit.SetSelected();
            selectedBuilding = unit;
        }
    }

    // Removes the unit from selected
    public void RemoveFromSelected(RTSEntity unit)
    {
        l_Selected.Remove(unit);
        unit.SetDeselected();
    }

    // Checks if the unit is within a group and deletes the unit from it
    public void RemoveFromGroup(RTSEntity unit)
    {
        if (l_Group1.Contains(unit))
        {
            l_Group1.Remove(unit);
        }
        else if (l_Group2.Contains(unit))
        {
            l_Group2.Remove(unit);
        }
        else if (l_Group3.Contains(unit))
        {
            l_Group3.Remove(unit);
        }
        else if (l_Group4.Contains(unit))
        {
            l_Group4.Remove(unit);
        }
        else if (l_Group5.Contains(unit))
        {
            l_Group5.Remove(unit);
        }
        else if (l_Group6.Contains(unit))
        {
            l_Group6.Remove(unit);
        }
        else
        {
            //Do nothing
            //...
            //AHUHUHUHUHU~
        }
    }

    // Removes everything from selected
    public void ClearSelected()
    {
        foreach (RTSEntity unit in l_Selected)
        {
            unit.SetDeselected();
        }
        
        if (selectedBuilding)
        {
            selectedBuilding.SetDeselected();
            selectedBuilding = null;
        }

        l_Selected.Clear();
        SelectedActiveEntities.Clear();
    }

    // ### Grouping functions ###

    // Add selected units to a group
    public void CreateGroup(int number)
    {
        switch (number)
        {
            case 1:
                foreach (RTSEntity unit in l_Selected)
                {
                    l_Group1.Add(unit);
                }
                break;

            case 2:
                foreach (RTSEntity unit in l_Selected)
                {
                    l_Group2.Add(unit);
                }
                break;

            case 3:
                foreach (RTSEntity unit in l_Selected)
                {
                    l_Group3.Add(unit);
                }
                break;

            case 4:
                foreach (RTSEntity unit in l_Selected)
                {
                    l_Group4.Add(unit);
                }
                break;

            case 5:
                foreach (RTSEntity unit in l_Selected)
                {
                    l_Group5.Add(unit);
                }
                break;

            case 6:
                foreach (RTSEntity unit in l_Selected)
                {
                    l_Group6.Add(unit);
                }
                break;
        }

    }

    // Adds the group to selected
    public void SelectGroup(int number)
    {
        ClearSelected();

        switch (number)
        {
            case 1:
                foreach (RTSEntity unit in l_Group1)
                {
                    AddToSelected(unit);
                }
            break;

            case 2:
                foreach (RTSEntity unit in l_Group2)
                {
                    AddToSelected(unit);
                }
                break;

            case 3:
                foreach (RTSEntity unit in l_Group3)
                {
                    AddToSelected(unit);
                }
                break;

            case 4:
                foreach (RTSEntity unit in l_Group4)
                {
                    AddToSelected(unit);
                }
                break;

            case 5:
                foreach (RTSEntity unit in l_Group5)
                {
                    AddToSelected(unit);
                }
                break;

            case 6:
                foreach (RTSEntity unit in l_Group6)
                {
                    AddToSelected(unit);
                }
                break;
        }
    }

    // Give orders to selected units
    public void GiveOrder(Order order)
    {
        foreach (IOrderable orderable in SelectedActiveEntities)
        {
            orderable.GiveOrder(order);
        }
    }

    // ### Functions to use in outside scripts ###

    // Returns the number of units currently selected
    public int ActiveEntityCount()
    {
        return SelectedActiveEntities.Count;
    }

    // Returns the first orderable unit in the selected list, to be used with sounds
    public IOrderable FirstActiveEntity()
    {
        return SelectedActiveEntities[0];
    }

    // Returns a list of active orderable units
    public List<IOrderable> ActiveEntityList()
    {
        return SelectedActiveEntities;
    }

    // Checks whether a certain unit can be found from the selected
    public bool IsEntitySelected(GameObject obj)
    {
        return l_Selected.Contains(obj.GetComponent<RTSEntity>());
    }

}
