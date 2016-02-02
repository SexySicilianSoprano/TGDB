using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemButton : MonoBehaviour
{
    private bool placingBuilding = false;
    private Building m_Host;
    private List<Item> m_Items = new List<Item>();
    
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Building GetHost()
    {
        return null;
    }

    public Item GetItem(int id)
    {
        Item valueToReturn = ItemDB.AllItems[id];
        return valueToReturn;
    }

    public void Execute(int item)
    {
        Item itemBeingBuilt = GetItem(item);
        Debug.Log("Executing button with " + itemBeingBuilt.Name);
        if (!placingBuilding)
        {
            GameObject newObject = Instantiate(itemBeingBuilt.Prefab) as GameObject;
            newObject.transform.position = Input.mousePosition;
            newObject.AddComponent<BuildingBeingPlaced>();
            SwitchState(State.placing);
        }
        else
        {

        }
        
    }

    private void SwitchState(State state)
    {
        switch (state)
        {
            case State.standby:
                placingBuilding = false;
            break;

            case State.placing:
                placingBuilding = true;
            break;

        }
    }

    public enum State
    {
        standby,
        placing
    }

}
    
