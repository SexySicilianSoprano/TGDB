using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour, IUIManager {

    //Singleton
    public static UIManager main;

    //Managers
    public static CursorManager m_CursorManager;

    //Action Variables
    
    private GameObject currentObject;

    //Mode Variables
    private Mode m_Mode = Mode.Normal;

    //Player identifier variables
    public Player primaryPlayer()
    {
        return GetComponent<GameManager>().primaryPlayer();
    }

    public Player enemyPlayer()
    {
        return GetComponent<GameManager>().enemyPlayer();
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
    private IGUIManager m_GuiManager;
    private IMiniMapController m_MiniMapController;
    private IEventsManager m_EventsManager;

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

    public Mode CurrentMode
    {
        get
        {
            return m_Mode;
        }
    }

    void Awake()
    {
        main = this;
    }


    // Use this for initialization
    void Start()
    {
        Debug.Log(primaryPlayer().controlledLayer + " swag");
        //Resolve interface variables
        m_SelectedManager = ManagerResolver.Resolve<ISelectedManager>();
        m_Camera = ManagerResolver.Resolve<ICamera>();
        m_GuiManager = ManagerResolver.Resolve<IGUIManager>();
        m_MiniMapController = ManagerResolver.Resolve<IMiniMapController>();

        //Attach Event Handlers
        m_EventsManager = ManagerResolver.Resolve<IEventsManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void ModeNormalBehaviour()
    {/*
        //Handle all non event, and non gui UI elements here
        hoverOver = HoverOver.Land;
        InteractionState interactionState = InteractionState.Nothing;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //Are we hovering over the GUI or the main screen?
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(1 << 4 | 6 << 16)))
        {
            //Mouse on main screen, let's do some shit
            currentObject = hit.collider.gameObject;
            switch (currentObject.layer)
            {
                case 8:
                case 9:
                    if (currentObject.layer == primaryPlayer.controlledLayer)
                    {
                        hoverOver = HoverOver.FriendlyUnit;
                    }
                    else
                    {
                        hoverOver = HoverOver.EnemyUnit;
                    }

                    break;

                case 11:
                case 17:
                    //Terrain or shroud
                    hoverOver = HoverOver.Land;
                    break;

                case 12:
                    //Friendly Building
                    hoverOver = HoverOver.FriendlyBuilding;
                    break;

                case 13:
                    //Enemy building
                    hoverOver = HoverOver.EnemyBuilding;
                    break;
            }
            
        }
        else
        {
            //Mouse is over GUI
            hoverOver = HoverOver.Menu;
        }

        if (hoverOver == HoverOver.Menu || m_SelectedManager.ActiveEntityCount() == 0 || m_GuiManager.GetSupportSelected != 0)
        {
            //Nothing orderable Selected or mouse is over menu or support is selected
            CalculateInteraction(hoverOver, ref interactionState);
        }
        else if (m_SelectedManager.ActiveEntityCount() == 1)
        {
            //One object selected
            CalculateInteraction(m_SelectedManager.FirstActiveEntity(), hoverOver, ref interactionState);
        }
        else
        {
            //Multiple objects selected, need to find which order takes precedence									
            CalculateInteraction(m_SelectedManager.ActiveEntityList(), hoverOver, ref interactionState);
        }

        //Tell the cursor manager to update itself based on the interactionstate
        m_CursorManager.UpdateCursor(interactionState);*/
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

            case HoverOver.FriendlyBuilding:
                //Select interaction
                interactionState = InteractionState.Select;

                //Unless a support item is selected
                if (m_GuiManager.GetSupportSelected == Const.MAINTENANCE_Sell)
                {
                    //Sell
                    if (currentObject.GetComponent<Building>().CanSell())
                    {
                        interactionState = InteractionState.Sell;
                    }
                    else
                    {
                        interactionState = InteractionState.CantSell;
                    }
                }
                else if (m_GuiManager.GetSupportSelected == Const.MAINTENANCE_Fix)
                {
                    //Fix
                    if (currentObject.GetComponent<Building>().GetHealthRatio() < 1)
                    {
                        interactionState = InteractionState.Fix;
                    }
                    else
                    {
                        interactionState = InteractionState.CantFix;
                    }
                }
                else if (m_GuiManager.GetSupportSelected == Const.MAINTENANCE_Disable)
                {
                    //Disable
                }
                break;

            case HoverOver.FriendlyUnit:
            case HoverOver.EnemyUnit:
            case HoverOver.EnemyBuilding:
                interactionState = InteractionState.Select;

                break;
        }
    }

    
    private void CalculateInteraction(IOrderable obj, HoverOver hoveringOver, ref InteractionState interactionState)
    {/*
        if (obj.IsAttackable())
        {
            if (hoverOver == HoverOver.EnemyUnit || hoverOver == HoverOver.EnemyBuilding)
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
            if (hoverOver == HoverOver.FriendlyUnit)
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

        if (hoverOver == HoverOver.FriendlyBuilding)
        {
            //Check if building can interact with object (repair building for example)
            if (currentObject.GetComponent<Building>().InteractWith(obj))
            {
                //Interact Interaction
                interactionState = InteractionState.Interact;
                return;
            }
        }

        if (hoverOver == HoverOver.FriendlyUnit || hoverOver == HoverOver.FriendlyBuilding || hoverOver == HoverOver.EnemyUnit || hoverOver == HoverOver.EnemyBuilding)
        {
            //Select Interaction
            interactionState = InteractionState.Select;
            return;
        }

        //Invalid interaction
        interactionState = InteractionState.Invalid; */
    }
    

    // TODO: 
    // Break the loop and return the desired interaction
    private void CalculateInteraction(List<IOrderable> list, HoverOver hoveringOver, ref InteractionState interactionState)
    {
        foreach (IOrderable obj in list)
        {
            bool ShouldInterractB = obj.ShouldInteract(hoveringOver);

            if (ShouldInterractB)
            {
                if (hoveringOver == HoverOver.EnemyUnit)
                {
                    CalculateInteraction(obj, hoveringOver, ref interactionState);
                    return;
                }
            }
        }
        CalculateInteraction(hoveringOver, ref interactionState);
    }

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

    //----------------------Mouse Button Handler------------------------------------
    private void ButtonClickedHandler()
    {
        //If mouse is over GUI then we don't want to process the button clicks
        
    }
    //-----------------------------------------------------------------------------

    //------------------------Mouse Button Commands--------------------------------------------
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

    public void LeftButton_DoubleClickDown()
    {
        if (currentObject.layer == primaryPlayer().controlledLayer)
        {
            //Select all units of that type on screen

        }
    }

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
                }
                break;

            case Mode.PlaceBuilding:
                if (m_Placed)
                {
                    m_Placed = false;
                }
                break;
        }
    }

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

    public void RightButton_DoubleClick()
    {

    }

    public void MiddleButton_SingleClick()
    {

    }

    public void MiddleButton_DoubleClick()
    {

    }
    //------------------------------------------------------------------------------------------

    

    private void ScrollWheelHandler(object sender)
    {
        //Zoom In/Out
        m_Camera.Zoom(sender);
        m_MiniMapController.ReCalculateViewRect();
    }

    private void MouseAtScreenEdgeHandler(object sender)
    {
        //Pan
        m_Camera.Pan(sender);
        m_MiniMapController.ReCalculateViewRect();
    }

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

    private void SwitchToModePlacingBuilding(Item item, Action callBackFunction)
    {
        m_Mode = Mode.PlaceBuilding;
        m_CallBackFunction = callBackFunction;
        m_ItemBeingPlaced = item;
        m_ObjectBeingPlaced = (GameObject)Instantiate(m_ItemBeingPlaced.Prefab);
        m_ObjectBeingPlaced.AddComponent<BuildingBeingPlaced>();
    }

    
}

public enum HoverOver
{
    Menu,
    Land,
    GUI,
    FriendlyUnit,
    EnemyUnit,
    FriendlyBuilding,
    EnemyBuilding,
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