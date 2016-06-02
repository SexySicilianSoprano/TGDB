using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// This component is attached to the Manager gameobject. Its purpose is to tie in
/// UI-related components and determine, what input does at which point.
/// It controls different states to do this.
/// This component borrows its base from an old RTS Engine made by Brett Hewitt,
/// but is heavily modified and altered to suit TGDB's needs.
/// 
/// Also to get rid of the old script-drawn GUI bullshit.
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// 
/// </summary>

public class UIManager : MonoBehaviour, IUIManager {

    //Singleton
    public static UIManager main;
    
    //Input related variables
    private bool isSelecting = false;
    private Vector3 v_mousePosition;

    //Action variables
    private GameObject currentObject;

    //Mode Variables
    private Mode m_Mode = Mode.Normal; // A state which determines whether we can command units normally or if the mouse functions are reserved for other purposes
    private HoverOver hoverOver = HoverOver.Land;// A state which tells us what layer we're pointing at
    private InteractionState interactionState = InteractionState.Nothing;// A state which determines what input actions do, only available in Mode.Normal
    private Identifier m_Identifier = Identifier.Neutral;// Is used to determine whether the item we're pointing at is an ally, enemy or neutral
    
    //Player identifier variables
    public Player primaryPlayer()
    {
        return GetComponent<GameManager>().primaryPlayer(); // Returns controlling player's info
    }

    public Player enemyPlayer()
    {
        return GetComponent<GameManager>().enemyPlayer(); // Returns enemy player's info
    }

    private string m_primaryPlayer
    {
        get { return primaryPlayer().controlledTag; }// Returns controlling player's tag
    }

    private string m_enemyPlayer
    {
        get { return enemyPlayer().controlledTag; } // Returns enemy player's tag       
    }

    //Interface variables the UI needs to deal with
    public SelectedManager m_SelectedManager() { return GetComponent<SelectedManager>(); } 
    public CursorManager m_CursorManager()  { return GetComponent<CursorManager>(); } 
    public GameManager m_GameManager() { return GetComponent<GameManager>(); } 

    // UNDER DELETION / REVISION THREAT
    //Building Placement variables
    private Action m_CallBackFunction;
    private Item m_ItemBeingPlaced;
    private GameObject m_ObjectBeingPlaced;
    private bool m_PositionValid = true;
    private bool m_Placed = false;
    
    public bool IsShiftDown
    {
        get;
        set;
    }

    public bool IsControlDown
    {
        get;
        set;
    }
    
    // State numerator accessors for outside scripts
    public Mode CurrentMode
    {
        get { return m_Mode; }
        set { m_Mode = CurrentMode; }
    }

    public InteractionState CurrentState
    {
        get { return interactionState; }
        set { interactionState = CurrentState; }
    }

    public HoverOver HoverOverState
    {
        get { return hoverOver; }
    }

    public Identifier CurrentIdentifier
    {
        get { return m_Identifier; }
    }

    // Singleton for ManagerResolver
    void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckHoverOver();
        SelectionListener();
        //Debug.Log(hoverOver + " " + interactionState);
        Debug.Log(m_Identifier + " " + hoverOver);

        switch (m_Mode)
        {
            case Mode.Normal:
                ModeNormalBehaviour();
                break;

            case Mode.Menu:
                break;

            case Mode.PlaceBuilding:
                break;
        }        
    }

    // Checks what is the mouse pointing at. Called by Update.
    private void CheckHoverOver()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentObject = hit.transform.gameObject;

            // What sort of an object are we pointing at?
            switch (currentObject.layer)
            {
                case 5:
                case 20:
                    hoverOver = HoverOver.GUI;
                    break;
                case 8:
                case 16:
                case 15:
                case 17:
                    hoverOver = HoverOver.Land;
                    break;
                case 9:
                    hoverOver = HoverOver.Ship;
                    break;
                case 10:
                    hoverOver = HoverOver.Submarine;
                    break;
                case 11:
                    hoverOver = HoverOver.AirUnit;
                    break;
                case 12:
                    hoverOver = HoverOver.Building;
                    break;
                case 13:
                    hoverOver = HoverOver.FogOfWar;
                    break;
                case 14:
                    hoverOver = HoverOver.Shroud;
                    break;
                case 22:
                    hoverOver = HoverOver.Mine;
                    break;
            }

            // Let's identify the unit's owner
            switch (currentObject.tag)
            {
                case "Player1":
                    m_Identifier = Identifier.Friend;
                    break;
                case "Player2":
                    m_Identifier = Identifier.Enemy;
                    break;
                case "Untagged":
                case "Terrain":
                case "Water":
                case "BuildingSpot":
                case "GUI":
                case "HitSurface":
                case "mmCamera":
                    m_Identifier = Identifier.Neutral;
                    break;
            }
        }
        else
        {
            // Raycast doesn't match with any of the known elements, thus we're on GUI
            //Debug.log("On GUI");
            hoverOver = HoverOver.GUI;
            m_Identifier = Identifier.Neutral;
        }
        
    }

    // Listens to mouse input actions and does shit whenever we click the mouse buttons
    private void GetInputAction()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)/* ~(5 << 23)  && EventSystem.current.IsPointerOverGameObject() == false */)
        {       
            // Right Mouse Button up, what happens next?
            if (Input.GetMouseButtonUp(1) && hoverOver == HoverOver.Land && m_SelectedManager().ActiveEntityCount() > 0)
            {
                // Create move order
                m_SelectedManager().GiveOrder(Orders.CreateMoveOrder(hit.point));
            }
            else if (Input.GetMouseButtonUp(1) && interactionState == InteractionState.Gather)
            {
                m_SelectedManager().GiveOrder(Orders.CreateGatherOrder(hit.transform.gameObject.GetComponent<ResourceMine>()));
            }

            // Left Mouse Button down, what happens?
            if (Input.GetMouseButtonDown(0) && hoverOver != HoverOver.GUI)
            {
                // Deselect selected units and start selecting new units
                m_SelectedManager().ClearSelected();
                isSelecting = true;
                v_mousePosition = Input.mousePosition;
            }

        }

        // Left Mouse Button up, what happens?
        if (Input.GetMouseButtonUp(0))
        {
            if (isSelecting)
            {
                // Selecting endes
                isSelecting = false;
                m_SelectedManager().ConfirmToBeSelected();
            }
        }

        // Stippedi stop :D
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (m_SelectedManager().ActiveEntityCount() > 0)
            {
                m_SelectedManager().GiveOrder(Orders.CreateStopOrder());
            }
        }

        // Keypad 1 pressed
        if (Input.GetKeyDown("1"))
        {
            // Are we holding down left control?
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.V))
            {
                // Create group 1
                m_SelectedManager().CreateGroup(1);
            }
            else
            {
                // Select group 1
                m_SelectedManager().SelectGroup(1);
            }
        }

        // Keypad 2 pressed
        if (Input.GetKeyDown("2"))
        {
            // Are we holding down left control? (Placeholder button is V because of retarded Unity Editor)
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.V))
            {
                // Create group 2
                m_SelectedManager().CreateGroup(2);
            }
            else
            {
                // Select group 2
                m_SelectedManager().SelectGroup(2);
            }
        }

        // Keypad 2 pressed
        if (Input.GetKeyDown("3"))
        {
            // Are we holding down left control? (Placeholder button is V because of retarded Unity Editor)
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.V))
            {
                // Create group 3
                m_SelectedManager().CreateGroup(3);
            }
            else
            {
                // Select group 3
                m_SelectedManager().SelectGroup(3);
            }
        }

        // Keypad 4 pressed
        if (Input.GetKeyDown("4"))
        {
            // Are we holding down left control? (Placeholder button is V because of retarded Unity Editor)
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.V))
            {
                // Create group 4
                m_SelectedManager().CreateGroup(4);
            }
            else
            {
                // Select group 4
                m_SelectedManager().SelectGroup(4);
            }
        }

        // Keypad 5 pressed
        if (Input.GetKeyDown("5"))
        {
            // Are we holding down left control? (Placeholder button is V because of retarded Unity Editor)
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.V))
            {
                // Create group 5
                m_SelectedManager().CreateGroup(5);
            }
            else
            {
                // Select group 5
                m_SelectedManager().SelectGroup(5);
            }
        }

        // Keypad 6 pressed
        if (Input.GetKeyDown("6"))
        {
            // Are we holding down left control? (Placeholder button is V because of retarded Unity Editor)
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.V))
            {
                // Create group 6
                m_SelectedManager().CreateGroup(6);
            }
            else
            {
                // Select group 6
                m_SelectedManager().SelectGroup(6);
            }
        }

    }

    // Listens if we're selecting and provides some action for it
    private void SelectionListener()
    {
        // Are we selecting?
        if (isSelecting) 
        {
            // Get every item with a component called RTSEntity
            foreach (var selectable in FindObjectsOfType<Unit>()) 
            {
                // Is the item within selection box boundaries?
                if (IsWithinSelectionBounds(selectable.gameObject))
                {
                    // The unit has to be unselected and has to be friendly
                    if (!m_SelectedManager().ActiveEntityList().Contains((IOrderable)selectable) && selectable.tag == m_primaryPlayer) //
                    {
                        // Unit is not previously selected and is friendly, so let's select it and turn on the projector
                        m_SelectedManager().AddToBeSelected(selectable);
                        // TODO: projector for selected unit indication graphics
                    }
                }
            }
        }
    }

    // When selecting, is there a unit within bounds?
    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
        {
            return false;
        }           

        var camera = Camera.main;
        var viewportBounds = SelectionBox.GetViewportBounds(camera, v_mousePosition, Input.mousePosition);

        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    // Normal input behaviour mode
    private void ModeNormalBehaviour()
    {        
        // interactionState = InteractionState.Nothing;
        GetInputAction();

        if (hoverOver == HoverOver.Menu || m_SelectedManager().ActiveEntityCount() <= 0 )
        {
            // Nothing orderable Selected or mouse is over menu or support is selected
            CalculateInteraction(hoverOver, ref interactionState);
        }
        else if (m_SelectedManager().ActiveEntityCount() == 1)
        {
            // One object selected
            CalculateInteraction(m_SelectedManager().FirstActiveEntity(), hoverOver, m_Identifier, ref interactionState);
        }
        else
        {
            // Multiple objects selected, need to find which order takes precedence									
            CalculateInteraction(m_SelectedManager().ActiveEntityList(), hoverOver, m_Identifier, ref interactionState);
        }

        // Tell the cursor manager to update itself based on the interactionstate
        m_CursorManager().UpdateCursorIcon(interactionState);
    }

    // Calculates interaction state by hoverover, used when nothing is selected
    private void CalculateInteraction(HoverOver hoveringOver, ref InteractionState interactionState)
    {
        switch (hoveringOver)
        {
            case HoverOver.Mine:
            case HoverOver.GUI:
            case HoverOver.Menu:
            case HoverOver.Land:
                interactionState = InteractionState.Nothing;
                break;

            case HoverOver.Building:
            case HoverOver.Ship:
            case HoverOver.Submarine:
            case HoverOver.AirUnit:
                interactionState = InteractionState.Select;
                break;
        }
    }

    // Calculates interaction state by hoverover and identifier, used when units are selected
    private void CalculateInteraction(IOrderable obj, HoverOver hoveringOver, Identifier identifier, ref InteractionState interactionState)
    { 
        
        if (obj.IsAttackable())
        {
            if (identifier == Identifier.Enemy)
            {
                //Attack Interaction
                interactionState = InteractionState.Attack;
                return;
            }
        }

        if (obj.IsDeployable())
        {
            if (((RTSEntity)obj).gameObject.Equals(currentObject))
            {
                //Deploy Interaction
                interactionState = InteractionState.Deploy;
                return;
            }
        }

        if (obj.IsInteractable())
        {
            if (identifier == Identifier.Friend)
            {
                //Check if object can interact with unit (carry all for example)
                if (((IInteractable)obj).InteractWith(currentObject))
                {
                    //Interact Interaction
                    interactionState = InteractionState.Interact;
                    return;
                }
            }
        }

        if (obj.IsMoveable())
        {
            if (hoverOver == HoverOver.Land)
            {
                //Move Interaction
                interactionState = InteractionState.Move;
                return;
            }
        }

        if (obj.IsGatherable())
        {
            if (hoverOver == HoverOver.Mine)
            {
                interactionState = InteractionState.Gather;
                return;
            }
        }
        /*
        
        if (identifier == Identifier.Friend)
        {
            //Check if building can interact with object (repair building for example)
            if (currentObject.GetComponent<Building>().InteractWith(obj))
            {
                //Interact Interaction
                interactionState = InteractionState.Interact;
                return;
            }
            else
            {
                //Select Interaction
                interactionState = InteractionState.Select;
                return;
            }
        }
        else
        {
            interactionState = InteractionState.Attack;
        }
        

        if (identifier == Identifier.Friend)
        {
            interactionState = InteractionState.Select;
            return;
        }
        else if (identifier == Identifier.Enemy)
        {
            interactionState = InteractionState.Attack;
            return;
        }
        else if (identifier == Identifier.Neutral)
        {
            if (hoverOver == HoverOver.Mine)
            {
                interactionState = InteractionState.Gather;
                return;
            }
            else
            {
                interactionState = InteractionState.Move;
                return;
            }
        } */

        //Invalid interaction
        interactionState = InteractionState.Invalid;
        return;
    }
    
    // Calculates what is the interaction with whatever we're pointing at
    private void CalculateInteraction(List<IOrderable> list, HoverOver hoveringOver, Identifier identifier, ref InteractionState interactionState)
    {
        foreach (IOrderable obj in list)
        {
            bool ShouldInterractB = obj.ShouldInteract(hoveringOver);

            if (ShouldInterractB)
            {
                if (hoveringOver == HoverOver.Mine || hoveringOver == HoverOver.Land || hoveringOver == HoverOver.Ship || hoverOver == HoverOver.Submarine || hoverOver == HoverOver.AirUnit || hoverOver == HoverOver.Building)
                {
                    CalculateInteraction(obj, hoveringOver, identifier, ref interactionState);
                    return;
                }
            }
        }
        CalculateInteraction(hoveringOver, ref interactionState);
    }

    // Draws the selection box on UI
    void OnGUI()
    {
        if (isSelecting)
        {
            var rect = SelectionBox.GetScreenRect(v_mousePosition, Input.mousePosition);
            SelectionBox.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            SelectionBox.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public bool IsCurrentUnit(RTSEntity obj)
    {
        return currentObject == obj.gameObject;
    }    
   
}

public enum Identifier
{
    Neutral,
    Friend,
    Enemy
}

public enum HoverOver
{
    Menu,
    Land,
    GUI,
    Ship,
    Submarine,
    AirUnit,
    Building,
    FogOfWar,
    Shroud,
    Mine
}

public enum InteractionState
{
    Nothing = 0,
    Invalid = 1,
    Move = 2,
    Attack = 3,
    Select = 4,
    Deploy = 5,
    Interact = 6,
    Sell = 7,
    CantSell = 8,
    Fix = 9,
    CantFix = 10,
    Disable = 11,
    CantDisable = 12,
    Gather = 13
}

public enum Mode
{
    Normal,
    Menu,
    PlaceBuilding,
    Disabled,
}