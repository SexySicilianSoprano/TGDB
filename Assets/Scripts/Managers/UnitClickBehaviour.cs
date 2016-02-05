using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

/*
    This component class determines what the unit does when it gets clicked.
    It is supposed to interract with UIManager.
    - Karl Sartorisio
    The Great Deep Blue
*/
public class UnitClickBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    // What is the pointer pointing at?
    private HoverOver hoverOver 
    {
        get
        {
            return m_UIManager.HoverOverState;
        }
    }

    // Accesses the UIManager and determines mouse action when it is clicked, if normal then it's used to command units
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

    // Other variables that the class needs to deal with
    private IUIManager m_UIManager; // This is used to communicate with the UIManager
    private ISelectedManager m_SelectedManager; // This is used to communicate with the SelectedManager
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
    
    void Update()
    {
        //ReadStates();
        Debug.Log(hoverOver);
        
        switch (hoverOver)
        {
            case HoverOver.Land:
                break;
            case HoverOver.FriendlyUnit:
                break;
            case HoverOver.EnemyUnit:
                break;
            case HoverOver.GUI:
                break;
            case HoverOver.Menu:
                break;
        }
    }
       
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clickade");
        // Is it a left mouse click?
        if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Left && eventData.dragging)
        {
            // Single clicked or drag selected, what happens next?
            Debug.Log("Clickan");

            switch (m_Mode)
            {
                case Mode.Normal:
                    // This unit is selected
                    switch (m_State)
                    {
                        case InteractionState.Select:
                            SetSelected();
                            break;
                    }
                    break;
            }
        }

        // Is it a double click?
        if (eventData.button == PointerEventData.InputButton.Left && DoubleClickCheck(eventData) == true)
        {
            // We've double clicked, what happens next?
            switch (hoverOver)
            {
                case HoverOver.FriendlyUnit:
                    // Select all similar units
                    // GetAllSimilarUnits();
                    break;

                case HoverOver.EnemyUnit:
                    // Unit is oh so evil, nothing happens
                    break;

            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (m_SelectedManager.ActiveEntityCount() > 0)
            {
                switch (hoverOver)
                {
                    case HoverOver.FriendlyUnit:
                        // 
                        break;

                    case HoverOver.EnemyUnit:
                        // Unit is oh so evil, let's fuck him up!
                        m_SelectedManager.GiveOrder(Orders.CreateAttackOrder(currentUnit));
                        break;
                }
            }
        }
    }

    // Used to determine hoverover state
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (unitTag)
        {
            case "Player1":
                
                break;

            case "Player2":
                break;

            case "GUI":
                break;

            case null:
                break;
        }
        
    }

    // Checks click events for double clicks
    private bool DoubleClickCheck(PointerEventData eventData)
    {
        int DoubleClickTH = 1; // Double click threshold in seconds

        if (eventData.clickCount == 2 && eventData.clickTime <= DoubleClickTH)
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

    private void SetSelected()
    {
        m_SelectedManager.AddToSelected(currentUnit);
    }

    private void GetAllSimilarUnits()
    {

    }
}
