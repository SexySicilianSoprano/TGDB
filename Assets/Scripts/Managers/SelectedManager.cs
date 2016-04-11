using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectedManager : MonoBehaviour, ISelectedManager {

    // Singleton
    public static SelectedManager main;

    // Active selected list variables
    private List<RTSEntity> l_Selected = new List<RTSEntity>();
    private List<IOrderable> SelectedActiveEntities = new List<IOrderable>();
    private List<GameObject> l_Printed = new List<GameObject>();

    // Grouping variables
    private List<RTSEntity> l_Group1 = new List<RTSEntity>();
    private List<RTSEntity> l_Group2 = new List<RTSEntity>();
    private List<RTSEntity> l_Group3 = new List<RTSEntity>();
    private List<RTSEntity> l_Group4 = new List<RTSEntity>();
    private List<RTSEntity> l_Group5 = new List<RTSEntity>();
    private List<RTSEntity> l_Group6 = new List<RTSEntity>();

    // Specific selected variables
    private RTSEntity selectedBuilding;

    void Awake()
    {
        main = this;
    }

    // ### Selection functions ###

    // Adds the unit to selected
    public void AddToSelected(RTSEntity unit)
    {
        if (unit.GetComponent<Unit>())
        {
            // We've selecting an unit
            if (!l_Selected.Contains(unit))
            {
                if (unit is IOrderable)
                {
                    SelectedActiveEntities.Add((IOrderable)unit);
                }
                Item item = ItemDB.AllItems.Find(x => x.Name.Contains(unit.Name));
                l_Selected.Add(unit);
                unit.SetSelected();
                PrintSelected(unit);
            }
        }
        else
        {
            // We selected a building, so let's just behave that way
            unit.SetSelected();
            selectedBuilding = unit;
        }
    }

    // Removes the unit from selected
    public void RemoveFromSelected(RTSEntity unit)
    {
        l_Selected.Remove(unit);
        unit.SetDeselected();
        ClearPrints();
    }

    // Select only this unit and delete all others from printed / selected
    public void AddOnlyThis(RTSEntity unit)
    {
        ClearPrints();
        ClearSelected();
        Item item = ItemDB.AllItems.Find(x => x.Name.Contains(unit.Name));
        AddToSelected(unit);
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
        ClearPrints();
    }

    // ### Grouping functions ###

    // Clear a group specific group to for reusal
    private void ClearGroup(int number)
    {
        switch (number)
        {
            case 1:
                foreach (RTSEntity unit in l_Group1)
                {
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "";
                }
                l_Group1.Clear();
                break;

            case 2:
                foreach (RTSEntity unit in l_Group2)
                {
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "";
                }
                l_Group2.Clear();
                break;

            case 3:
                foreach (RTSEntity unit in l_Group3)
                {
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "";
                }
                l_Group3.Clear();
                break;

            case 4:
                foreach (RTSEntity unit in l_Group4)
                {
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "";
                }
                l_Group4.Clear();
                break;

            case 5:
                foreach (RTSEntity unit in l_Group5)
                {
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "";
                }
                l_Group5.Clear();
                break;

            case 6:
                foreach (RTSEntity unit in l_Group6)
                {
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "";
                }
                l_Group6.Clear();
                break;
        }
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

    // Add selected units to a group
    public void CreateGroup(int number)
    {
        ClearGroup(number);

        switch (number)
        {
            case 1:
                foreach (RTSEntity unit in l_Selected)
                {
                    RemoveFromGroup(unit);
                    l_Group1.Add(unit);
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "1";
                }
                break;

            case 2:
                foreach (RTSEntity unit in l_Selected)
                {
                    RemoveFromGroup(unit);
                    l_Group2.Add(unit);
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "2";
                }
                break;

            case 3:
                foreach (RTSEntity unit in l_Selected)
                {
                    RemoveFromGroup(unit);
                    l_Group3.Add(unit);
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "3";
                }
                break;

            case 4:
                foreach (RTSEntity unit in l_Selected)
                {
                    RemoveFromGroup(unit);
                    l_Group4.Add(unit);
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "4";
                }
                break;

            case 5:
                foreach (RTSEntity unit in l_Selected)
                {
                    RemoveFromGroup(unit);
                    l_Group5.Add(unit);
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "5";
                }
                break;

            case 6:
                foreach (RTSEntity unit in l_Selected)
                {
                    RemoveFromGroup(unit);
                    l_Group6.Add(unit);
                    Text text = unit.GetComponent<HPBar>().healthBar.transform.Find("GroupNumber").GetComponent<Text>();
                    text.text = "6";
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


    // ### Other functions (some are called from outside this script) ###

    // Prints selected unit's icons on SelectedPanel
    private void PrintSelected(RTSEntity unit)
    {
        // Get SelectedPanel
        GameObject selectedPanel = GameObject.Find("UI").transform.Find("SelectedPanel").gameObject;

        // Create a new GUI element and set its size and place
        GameObject newSelectedImg = new GameObject();
        newSelectedImg.name = "SelectedImg";
        newSelectedImg.transform.SetParent(selectedPanel.transform);

        // Format position variables and set new values
        newSelectedImg.AddComponent<RectTransform>();
        newSelectedImg.GetComponent<RectTransform>().localScale = Vector3.one;
        newSelectedImg.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        newSelectedImg.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        newSelectedImg.GetComponent<RectTransform>().sizeDelta.Set(10, 10);

        // Add Image and determine picture
        newSelectedImg.AddComponent<Image>();
        Item item = ItemDB.AllItems.Find(x => x.Name.Contains(unit.Name));
        newSelectedImg.GetComponent<Image>().sprite = item.ItemImage;
        newSelectedImg.layer = 5;
        newSelectedImg.GetComponent<Image>().preserveAspect = true;

        // Give the new image gameobject vertical layout group component
        newSelectedImg.AddComponent<VerticalLayoutGroup>();
        newSelectedImg.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;
        newSelectedImg.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
        newSelectedImg.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
        //newSelectedImg.GetComponent<Image>().preserveAspect = true;

        // Give it a button to select itself if necessary
        newSelectedImg.AddComponent<Button>();
        UnityEngine.Events.UnityAction addAction = () => { AddOnlyThis(unit); };
        newSelectedImg.GetComponent<Button>().onClick.AddListener(addAction);

        // Add Health bar to the print
        GameObject newHPBar = Instantiate(unit.GetComponent<HPBar>().healthBar.gameObject);
        newHPBar.transform.SetParent(newSelectedImg.transform);
        newHPBar.GetComponent<RectTransform>().localScale = Vector3.one * 0.7f;
        unit.GetComponent<HPBar>().selectedHealthBar = newHPBar;
        unit.GetComponent<HPBar>().selectedBar = newHPBar.transform.Find("HPBG").transform.Find("HPGreen").GetComponent<Image>();
        //newHPBar.GetComponent<RectTransform>().localPosition = new Vector2(50, 0);

        // Add to printed unit list
        l_Printed.Add(newSelectedImg);
    }

    // Clear the printed list
    private void ClearPrints()
    {
        foreach (GameObject print in l_Printed)
        {
            Destroy(print);
        }

        l_Printed.Clear();
    }

    // Give orders to selected units
    public void GiveOrder(Order order)
    {
        foreach (IOrderable orderable in SelectedActiveEntities)
        {
            orderable.GiveOrder(order);
        }
    }

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
