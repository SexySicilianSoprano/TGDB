using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
/*
    This component is attached to the Manager gameobject. Its purpose is to tie in
    UI-related components and determine, what input does at which point

*/

public class UIManager : MonoBehaviour, IUIManager {

    //Singleton
    public static UIManager main;

    //Managers
    public static CursorManager m_CursorManager;

    // Mouse Event
    public Event MouseEvent = Event.current;

    // Action variables
    private GameObject currentObject;

    //Mode Variables
    private Mode m_Mode = Mode.Normal;
    private HoverOver hoverOver = HoverOver.Land;
    private InteractionState m_State = InteractionState.Nothing;
    private Identifier m_Identifier = Identifier.Neutral;
    
    //Player identifier variables
    public Player primaryPlayer()
    {
        return GetComponent<Manager>().primaryPlayer;
    }

    public Player enemyPlayer()
    {
        return GetComponent<Manager>().enemyPlayer;
    }

    private string m_primaryPlayer
    {
        get
        {
            return primaryPlayer().controlledTag;
        }
    }

    private string m_enemyPlayer
    {
        get
        {
            return enemyPlayer().controlledTag;
        }
    }

    //Interface variables the UI needs to deal with
    private ISelectedManager m_SelectedManager;
    private ICamera m_Camera;
    //private IGUIManager m_GuiManager;
    //private IMiniMapController m_MiniMapController;
    //private IEventsManager m_EventsManager;

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
        get { return m_State; }
        set { m_State = CurrentState; }
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
    }

    // Update is called once per frame
    void Update()
    {
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

    private void GetMouseAction()
    {
        if (MouseEvent.button == 1 && MouseEvent.isMouse)
        {
            m_SelectedManager.GiveOrder(Orders.CreateMoveOrder(Input.mousePosition));
        }
    }
    
    private void ModeNormalBehaviour()
    {
        InteractionState interactionState = InteractionState.Nothing;        
       

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
        m_CursorManager.UpdateCursor(interactionState);
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

    // UP FOR DELETION
    //----------------------Mouse Button Handler------------------------------------
    private void ButtonClickedHandler()
    {
        //If mouse is over GUI then we don't want to process the button clicks
        
    }
    //-----------------------------------------------------------------------------

    //------------------------Mouse Button Commands--------------------------------------------

    // UP FOR DELETION
    public void LeftButton_SingleClickDown()
    {
        switch (m_Mode)
        {
            case Mode.Normal:
                //We've left clicked, what have we left clicked on?
                int currentObjLayer = currentObject.layer;

                if (currentObjLayer == primaryPlayer().controlledLayer)
                {
                    //Friendly Unit, is the unit selected?
                    if (m_SelectedManager.IsEntitySelected(currentObject))
                    {
                        //Is the unit deployable?
                        if (currentObject.GetComponent<Unit>())
                        {
                            if (currentObject.GetComponent<Unit>().IsDeployable())
                            {
                                
                            }
                        }
                    }
                }
                break;

            case Mode.PlaceBuilding:
                //We've left clicked, if we're valid place the building
                if (m_PositionValid)
                {/*
                    GameObject newObject = (GameObject)Instantiate(m_ItemBeingPlaced.Prefab, m_ObjectBeingPlaced.transform.position, m_ObjectBeingPlaced.transform.rotation);
                    

                    newObject.layer = primaryPlayer.controlledLayer;
                    newObject.tag = primaryPlayer.controlledTag;

                    
                    m_CallBackFunction.Invoke();
                    m_Placed = true;
                    newObject.GetComponent<BoxCollider>().isTrigger = false;
                    SwitchToModeNormal();*/
                }
                break;
        }
    }

    // UP FOR DELETION
    public void LeftButton_DoubleClickDown()
    {
        if (currentObject.layer == primaryPlayer().controlledLayer)
        {
            //Select all units of that type on screen

        }
    }

    // UP FOR DELETION
    public void LeftButton_SingleClickUp()
    {
        switch (m_Mode)
        {
            case Mode.Normal:
                //If we've just switched from another mode, don't execute
                if (m_Placed)
                {
                    m_Placed = false;
                    return;
                }
                /*
                //We've left clicked, have we left clicked on a unit?
                int currentObjLayer = currentObject.layer;
                if (!m_GuiManager.Dragging && (currentObjLayer == primaryPlayer().controlledLayer || currentObjLayer == enemyPlayer().controlledLayer || currentObjLayer == 12 || currentObjLayer == 13))
                {
                    if (!IsShiftDown)
                    {
                        m_SelectedManager.ClearSelected();
                    }

                    m_SelectedManager.AddToSelected(currentObject.GetComponent<RTSEntity>());
                }
                else if (!m_GuiManager.Dragging)
                {
                    m_SelectedManager.ClearSelected();
                }*/
                break;

            case Mode.PlaceBuilding:
                if (m_Placed)
                {
                    m_Placed = false;
                }
                break;
        }
    }

    // UP FOR DELETION
    public void RightButton_SingleClick()
    {
        switch (m_Mode)
        {
            case Mode.Normal:
                //We've right clicked, have we right clicked on ground, interactable object or enemy?
                int currentObjLayer = currentObject.layer;

                if (currentObjLayer == 11 || currentObjLayer == 17 || currentObjLayer == 20)
                {
                    //Terrain -> Move Command
                    //m_SelectedManager.GiveOrder(Orders.CreateMoveOrder(WorldPosClick));
                }
                else if (currentObjLayer == primaryPlayer().controlledLayer || currentObjLayer == 14)
                {
                    //Friendly Unit -> Interact (if applicable)
                }
                else if (currentObjLayer == enemyPlayer().controlledLayer || currentObjLayer == 15)
                {
                    //Enemy Unit -> Attack                    
                    //m_SelectedManager.GiveOrder(Orders.CreateAttackOrder(e.target));
                }
                else if (currentObjLayer == 12)
                {
                    //Friendly Building -> Interact (if applicable)
                }
                else if (currentObjLayer == 13)
                {
                    //Enemy Building -> Attack                    
                    //m_SelectedManager.GiveOrder(Orders.CreateAttackOrder(e.target));

                }
                break;

            case Mode.PlaceBuilding:

                //Cancel building placement


                SwitchToModeNormal();
                break;
        }
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

    // UP FOR DELETION
    //-----------------------------------KeyBoard Handler---------------------------------
    private void KeyBoardPressedHandler()
    {
       //e.Command();
    }
    //-------------------------------------------------------------------------------------

    public bool IsCurrentUnit(RTSEntity obj)
    {
        return currentObject == obj.gameObject;
    }
    
    public void UserPlacingBuilding(Item item, Action callbackFunction)
    {
        SwitchToModePlacingBuilding(item, callbackFunction);
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