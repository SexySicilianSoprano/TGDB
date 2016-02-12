using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
/*
    This component is attached to the Manager gameobject. Its purpose is to tie in
    UI-related components and determine, what input does at which point.
    It controls different states to do this.
    This component borrows its base from an old RTS Engine made by Brett Hewitt,
    but is heavily modified and altered to suit TGDB's needs.
    Also to get rid of the old style script-drawn GUI bullshit.

    - Karl Sartorisio
    The Great Deep Blue
*/

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
        return GetComponent<Manager>().primaryPlayer; // Returns controlling player's info
    }

    public Player enemyPlayer()
    {
        return GetComponent<Manager>().enemyPlayer; // Returns enemy player's info
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
    private ISelectedManager m_SelectedManager;
    private ICamera m_Camera;
    private ICursorManager m_CursorManager;
    //private IGUIManager m_GuiManager;
    //private IMiniMapController m_MiniMapController;
    //private IEventsManager m_EventsManager;

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

    // Instantiator for ManagerResolver
    void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start()
    {
        //Resolve interface variables
        m_SelectedManager = ManagerResolver.Resolve<ISelectedManager>();
        m_Camera = ManagerResolver.Resolve<ICamera>();
        m_CursorManager = ManagerResolver.Resolve<ICursorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isSelecting);
        CheckHoverOver();
        
        switch (m_Mode)
        {
            case Mode.Normal:
                ModeNormalBehaviour();
                break;

            case Mode.Menu:
                break;

            case Mode.PlaceBuilding:
                ModePlaceBuildingBehaviour();
                break;
        }        
    }

    // Checks what is the mouse pointing at. Called by Update.
    private void CheckHoverOver()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(8 << 14)))
        {
            currentObject = hit.collider.gameObject;

            // What sort of an object are we pointing at?
            switch (currentObject.layer)
            {
                case 8:
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
                    m_Identifier = Identifier.Neutral;
                    break;
            }        
        }
        else
        {
            // Raycast doesn't match with any of the known elements, thus we're on GUI
            Debug.Log("On GUI");
            hoverOver = HoverOver.GUI;
            m_Identifier = Identifier.Neutral;
        }
    }

    // Listens to mouse input actions and does shit whenever we click the mouse buttons
    private void GetMouseAction()
    {
        // Right Mouse Button up, what happens next?
        if (Input.GetMouseButtonUp(1) && hoverOver == HoverOver.Land && m_SelectedManager.ActiveEntityCount() > 0)
        {
            // Create move order
            m_SelectedManager.GiveOrder(Orders.CreateMoveOrder(Input.mousePosition));
        }

        // Left Mouse Button down, what happens?
        if (Input.GetMouseButtonDown(0) && hoverOver == HoverOver.Land)
        {
            // Deselect selected units and start selecting new units
            m_SelectedManager.ClearSelected();
            isSelecting = true;
            v_mousePosition = Input.mousePosition;
        }

        // Left Mouse Button up, what happens?
        if (Input.GetMouseButtonUp(0))
        {
            // Selecting endes
            isSelecting = false;
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
        {
            return false;
        }           

        var camera = Camera.main;
        var viewportBounds =
            SelectionBox.GetViewportBounds(camera, v_mousePosition, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
    }

    private void ModeNormalBehaviour()
    {        
        interactionState = InteractionState.Nothing;
        GetMouseAction();

        if (hoverOver == HoverOver.Menu || m_SelectedManager.ActiveEntityCount() == 0 )
        {
            //Nothing orderable Selected or mouse is over menu or support is selected
            CalculateInteraction(hoverOver, ref interactionState);
        }
        else if (m_SelectedManager.ActiveEntityCount() == 1)
        {
            //One object selected
            CalculateInteraction(m_SelectedManager.FirstActiveEntity(), hoverOver, m_Identifier, ref interactionState);
        }
        else
        {
            //Multiple objects selected, need to find which order takes precedence									
            CalculateInteraction(m_SelectedManager.ActiveEntityList(), hoverOver, m_Identifier, ref interactionState);
        }

        //Tell the cursor manager to update itself based on the interactionstate
        m_CursorManager.UpdateCursorIcon(interactionState);
    }

    private void CalculateInteraction(HoverOver hoveringOver, ref InteractionState interactionState)
    {
        switch (hoveringOver)
        {
            case HoverOver.Menu:
            case HoverOver.Land:
                //Normal Interaction
                interactionState = InteractionState.Nothing;
                break;

            case HoverOver.Building:
                //Select interaction
                interactionState = InteractionState.Select;
                break;

            case HoverOver.Ship:
            case HoverOver.Submarine:
            case HoverOver.AirUnit:
                interactionState = InteractionState.Select;
                break;
        }
    }

    
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

        if (hoverOver == HoverOver.Building && identifier == Identifier.Friend)
        {
            //Check if building can interact with object (repair building for example)
            if (currentObject.GetComponent<Building>().InteractWith(obj))
            {
                //Interact Interaction
                interactionState = InteractionState.Interact;
                return;
            }
        }

        if (identifier == Identifier.Friend || hoverOver == HoverOver.Building || hoverOver == HoverOver.Ship || hoverOver == HoverOver.AirUnit || hoverOver == HoverOver.Submarine)
        {
            //Select Interaction
            interactionState = InteractionState.Select;
            return;
        }

        //Invalid interaction
        interactionState = InteractionState.Invalid;
    }
    
    // UP FOR REVISIONING
    private void CalculateInteraction(List<IOrderable> list, HoverOver hoveringOver, Identifier identifier, ref InteractionState interactionState)
    {
        foreach (IOrderable obj in list)
        {
            bool ShouldInterractB = obj.ShouldInteract(hoveringOver);

            if (ShouldInterractB)
            {
                if (hoveringOver == HoverOver.Ship || hoverOver == HoverOver.Submarine || hoverOver == HoverOver.AirUnit)
                {
                    CalculateInteraction(obj, hoveringOver, identifier, ref interactionState);
                    return;
                }
            }
        }
        CalculateInteraction(hoveringOver, ref interactionState);
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            var rect = SelectionBox.GetScreenRect(v_mousePosition, Input.mousePosition);
            SelectionBox.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            SelectionBox.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }


    
    //-----------------------------------UP FOR DELETION / REVISION---------------------------------

    // UP FOR REVISIONING
    private void ModePlaceBuildingBehaviour()
    {
        //Get current location and place building on that location
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 11))
        {
            m_ObjectBeingPlaced.transform.position = hit.point;
        }

        if (m_ObjectBeingPlaced.GetComponent<BuildingBeingPlaced>().BuildValid == true)
        {
            m_PositionValid = true;
        }
        else
        {
            m_PositionValid = false;
        }

        if (m_PositionValid)
        {
            m_ObjectBeingPlaced.GetComponent<BuildingBeingPlaced>().SetToValid();
        }
        else
        {
            m_ObjectBeingPlaced.GetComponent<BuildingBeingPlaced>().SetToInvalid();
        }

    }

    private void KeyBoardPressedHandler()
    {
        //e.Command();
    }

        // UP FOR DELETION ????
    private void ScrollWheelHandler(object sender)
    {
        //Zoom In/Out
        m_Camera.Zoom(sender);
        //m_MiniMapController.ReCalculateViewRect();
    }

    // UP FOR DELETION ????
    private void MouseAtScreenEdgeHandler(object sender)
    {
        //Pan
        m_Camera.Pan(sender);
        //m_MiniMapController.ReCalculateViewRect();
    }


    public void UserPlacingBuilding(Item item, Action callbackFunction)
    {
        SwitchToModePlacingBuilding(item, callbackFunction);
    }

    // Determine what to do with this
    private void SwitchToModeNormal()
    {
        if (m_ObjectBeingPlaced)
        {
            Destroy(m_ObjectBeingPlaced);
        }
        m_CallBackFunction = null;
        m_ItemBeingPlaced = null;
        m_Mode = Mode.Normal;
    }

    // REVISION
    private void SwitchToModePlacingBuilding(Item item, Action callBackFunction)
    {
        m_Mode = Mode.PlaceBuilding;
        m_CallBackFunction = callBackFunction;
        m_ItemBeingPlaced = item;
        m_ObjectBeingPlaced = (GameObject)Instantiate(m_ItemBeingPlaced.Prefab);
        m_ObjectBeingPlaced.AddComponent<BuildingBeingPlaced>();
    }

    //-------------------------------------------------------------------------------------

    public bool IsCurrentUnit(RTSEntity obj)
    {
        return currentObject == obj.gameObject;
    }    

    // Switches the Mode to your choosing
    public void SwitchMode(Mode mode)
    {
        switch (mode)
        {

            case Mode.Normal:
                SwitchToModeNormal();
                break;

            case Mode.Menu:

                break;

            case Mode.Disabled:

                break;
        }
    }

    

    
}

public enum Identifier
{
    Neutral,
    Friend,
    Enemy,
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
}

public enum Mode
{
    Normal,
    Menu,
    PlaceBuilding,
    Disabled,
}