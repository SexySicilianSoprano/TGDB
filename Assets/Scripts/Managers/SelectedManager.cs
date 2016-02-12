using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectedManager : MonoBehaviour, ISelectedManager {

    // Singleton
    public static SelectedManager main;

    private List<RTSEntity> l_Selected = new List<RTSEntity>();
    private List<IOrderable> SelectedActiveEntities = new List<IOrderable>();
    private List<int> ListOfGroups = new List<int>();

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

    // Removes the unit from selected
    public void RemoveFromSelected(RTSEntity unit)
    {
        l_Selected.Remove(unit);
    }

    // Removes everything from selected
    public void ClearSelected()
    {
        l_Selected.Clear();
        SelectedActiveEntities.Clear();
    }

    // ### Grouping functions ###

    // Create Group with selected units and give it a hotkey
    public void CreateGroup(int number)
    {
        //TODO: Create Group-class

    }

    // Adds the group to selected
    public void SelectGroup(int number)
    {

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
