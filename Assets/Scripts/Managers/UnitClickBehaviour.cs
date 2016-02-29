using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

/// <summary>
/// This component determines what the unit does when it gets clicked.
/// It interracts with UIManager's different states.
/// 
/// Some info on on clicks:
/// - Left Single Click selects the unit when it's clicked and deselects it when clicked on the ground (Deselection handled in UIManager)
/// - Left Double Click selects all the same type of units on the screen while deselecting previously selected units
/// - Right Single Click 
/// 
/// - Karl Sartorisio
///  The Great Deep Blue
/// </summary>

public class UnitClickBehaviour : MonoBehaviour, IPointerClickHandler
{
    // What is the pointer pointing at?
    private HoverOver hoverOver 
    {
        get
        {
            return m_UIManager.HoverOverState;
        }
    }

    // Accesses the UIManager and determines mouse action mode when it is clicked, if normal then it's used to command units
    private Mode m_Mode 
    {
        get
        {
            return m_UIManager.CurrentMode;
        }
    }   

    // What does the selected unit do when you click
    private InteractionState m_State
    {
        get
        {
            return m_UIManager.CurrentState;
        }
    }

    private Identifier m_Identifier
    {
        get
        {
            return m_UIManager.CurrentIdentifier;
        }
    }

    // Other variables that the class needs to deal with
    private IUIManager m_UIManager; // This is used to communicate with the UIManager
    private ISelectedManager m_SelectedManager; // This is used to communicate with the SelectedManager
    private IGameManager m_GameManager; // This is used to determine player priorities
    private RTSEntity currentUnit; // This is used to hold the unit data this game object has
    private string unitTag; // Who owns this unit?
    private Player primaryPlayer; // Primary player information   
    private Player enemyPlayer; // Enemy player information
    
    void Start()
    {
        unitTag = gameObject.tag; // Get the unit's owner
        m_UIManager = ManagerResolver.Resolve<IUIManager>(); // Get the UIManager that's being used
        m_SelectedManager = ManagerResolver.Resolve<ISelectedManager>(); // Get the SelectedManager that's being used
        currentUnit = GetComponent<RTSEntity>(); // Get the unit data tied to this game object
        primaryPlayer = m_UIManager.primaryPlayer(); // Inject with some sweet data
        enemyPlayer = m_UIManager.enemyPlayer(); // This one too!
    }
    
    // This is called when unit has been clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clickade");
        // Is it a left mouse click?
        if (eventData.button == PointerEventData.InputButton.Left && !DoubleClickCheck(eventData))
        {
            // Single clicked or drag selected, what happens next?
            Debug.Log("Clickan");
            

            switch (m_Mode)
            {
                case Mode.Normal:

                    if (m_State != InteractionState.Invalid)
                    {
                        // Clear the active selected group
                        if (m_SelectedManager.ActiveEntityList() != null)
                        {
                            EmptySelected();
                        }
                        // Is the unit friendly?
                        if (currentUnit.tag == primaryPlayer.controlledTag)
                        {
                            Debug.Log("Selected" + currentUnit);          
                            SetSelected();
                        }
                    }                    
                    break;
            }
        }

        // Is it a double click?
        // TODO: FIX THIS
        if (eventData.button == PointerEventData.InputButton.Left && DoubleClickCheck(eventData))
        {
            Debug.Log("Doubury Clickan");
            // We've double clicked, what happens next?
            switch (hoverOver)
            {
                case HoverOver.Ship:
                case HoverOver.AirUnit:
                case HoverOver.Submarine:
                    // Select all similar units
                    // GetAllSimilarUnits();
                    break;

                case HoverOver.Building:
                    // Unit is oh so evil, nothing happens
                    break;

            }
        }
        // Is it a right click?
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Why, yes it is!
            Debug.Log("Righto Clickan");
            if (m_SelectedManager.ActiveEntityCount() > 0)
            {
                // Get a command that will be sent to selected units
                GetCommand();
            }
        }
    }
    
    // Checks click events for double clicks
    private bool DoubleClickCheck(PointerEventData eventData)
    {
        int DoubleClickTH = 1; // Double click threshold in seconds

        if (eventData.clickCount >= 2 && eventData.clickTime <= DoubleClickTH * Time.deltaTime)
        {
            // Second click happens within threshold, double click is a go
            return true;
        }
        else
        {
            // You were either too slow or didn't click twice
            return false;
        }
    }

    private void GetCommand()
    {
        switch (m_State)
        {
            case InteractionState.Select:
                Debug.Log("Select " + currentUnit);
                //SetSelected();
                break;
            case InteractionState.Move:
                Debug.Log("Move");
                m_SelectedManager.GiveOrder(Orders.CreateMoveOrder(Input.mousePosition));
                break;
            case InteractionState.Attack:
                Debug.Log("Attack " + currentUnit);
                m_SelectedManager.GiveOrder(Orders.CreateAttackOrder(currentUnit));
                break;
            case InteractionState.Deploy:
                Debug.Log("Deploy " + currentUnit);
                m_SelectedManager.GiveOrder(Orders.CreateDeployOrder());
                break;
        }
    }

    private void ListenToDragged()
    {/*
        if (GetComponent<Renderer>().isVisible && m_guiManager.Dragging)
        {
            if (m_guiManager.IsWithin(transform.position))
            {
                m_selectedManager.AddToSelected(this);
            }
            else
            {
                m_selectedManager.RemoveFromSelected(this);
            }
        }*/
    }

    private void SetSelected()
    {
        m_SelectedManager.AddToSelected(currentUnit);
    }

    private void EmptySelected()
    {
        m_SelectedManager.ClearSelected();
    }

    private void GetAllSimilarUnits()
    {
        //TODO
    }
}
