using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

/// <summary>
/// 
/// Attach this component to building menu buttons to give them a behavior.
/// Remember to add the public variable index a value to call for whatever it is supposed to build.
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// 
/// </summary>

public class ButtonClickBehaviour : MonoBehaviour, IPointerClickHandler {

    // Index for spawning purposes
    public int index;
    
    Transform m_Parent { get { return gameObject.transform.parent; } }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (m_Parent.name == "ConstPanel")
            {
                BuildingScript builder = GameObject.Find("Manager").GetComponent<BuildingScript>();

                if (builder.onHold)
                {
                    // Toggle on hold
                    builder.ToggleOnHold();
                }
                else
                {
                    // Build this structure
                    builder.buildingFunction(index);
                }
            }
            else if (m_Parent.name == "UnitPanel")
            {
                UnitBuildingScript unitBuilder = GameObject.Find("Manager").GetComponent<UnitBuildingScript>();

                if (unitBuilder.onHold)
                {
                    // Toggle on hold
                    unitBuilder.ToggleOnHold();
                }
                else
                {
                    // Build this unit
                    unitBuilder.BuildNewUnit(index);
                }
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (m_Parent.name == "ConstPanel")
            {
                BuildingScript builder = GameObject.Find("Manager").GetComponent<BuildingScript>();

                if (builder.onHold)
                {
                    builder.DeleteBuilding();
                }
                else
                {
                    builder.ToggleOnHold();
                }
            }
            else if (m_Parent.name == "UnitPanel")
            {
                UnitBuildingScript unitBuilder = GameObject.Find("Manager").GetComponent<UnitBuildingScript>();

                if (unitBuilder.onHold)
                {
                    unitBuilder.DeleteLastInQueue();
                }
                else
                {
                    unitBuilder.ToggleOnHold();
                }
            }
        }
    }
}
