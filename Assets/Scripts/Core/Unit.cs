using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Selected))]
public class Unit : RTSEntity, IOrderable {

    //Member Variables
    protected bool m_IsMoveable = true;
    protected bool m_IsDeployable = false;
    protected bool m_IsAttackable = true;
    protected bool m_IsInteractable = false;

    protected IGUIManager m_guiManager
    {
        get;
        private set;
    }

    protected ISelectedManager m_selectedManager
    {
        get;
        private set;
    }

    protected IUIManager m_UIManager
    {
        get;
        private set;
    }

    private Player primaryPlayer
    {
        get
        {
            return m_UIManager.primaryPlayer();
        }
    }

    protected void Start()
    {
        // m_guiManager = ManagerResolver.Resolve<IGUIManager>();
        m_selectedManager = ManagerResolver.Resolve<ISelectedManager>();
        m_UIManager = ManagerResolver.Resolve<IUIManager>();
        // ManagerResolver.Resolve<IManager>().UnitAdded(this);
  
        /*
		m_IsDeployable = this is IDeployable;
		m_IsAttackable = this is IAttackable;
		m_IsInteractable = this is IInteractable;
        */
    }

    protected void Update()
    {
        
    }

    public override void SetSelected()
    {
        if (!GetComponent<Selected>().IsSelected)
        {
            GetComponent<Selected>().SetSelected();
        }
    }

    public override void SetDeselected()
    {
        GetComponent<Selected>().SetDeselected();
    }

    public override void AssignToGroup(int groupNumber)
    {
        GetComponent<Selected>().AssignGroupNumber(groupNumber);
    }

    public override void RemoveFromGroup()
    {
        GetComponent<Selected>().RemoveGroupNumber();
    }

    public override void ChangeTeams(int team)
    {
        switch (team)
        {
            case Const.TEAM_STEAMHOUSE:

                break;

        }
    }

    public bool IsDeployable()
    {
        //return m_IsDeployable;
        return this is IDeployable;
    }

    public bool IsAttackable()
    {
        return m_IsAttackable;
    }

    public bool IsMoveable()
    {
        return m_IsMoveable;
    }

    public bool IsInteractable()
    {
        return m_IsInteractable;
    }

    public void GiveOrder(Order order)
    {
        switch (order.OrderType)
        {
            // Stop Order
            case Const.ORDER_STOP:
                                
                GetComponent<Combat>().Stop();
                if (IsMoveable())
                {
                    if (IsDeployable())
                    {
                        CancelDeploy();
                    }
                    GetComponent<Movement>().Stop();
                }
                break;

            // Move Order
            case Const.ORDER_MOVE_TO:

                GetComponent<Combat>().Stop();
                if (IsMoveable())
                {
                    if (IsDeployable())
                    {
                        CancelDeploy();
                    }
                    
                    GetComponent<Movement>().MoveTo(order.OrderLocation);
                }
                break;

            // Deploy Order
            case Const.ORDER_DEPLOY:

                GetComponent<Movement>().Stop();

                ((IDeployable)this).Deploy();
                break;

            // Attack Order
            case Const.ORDER_ATTACK:

                GetComponent<Combat>().Stop();
                if (IsAttackable())
                {
                    // Attack                    
                    GetComponent<Combat>().Attack(order.Target);
                }

                break;

        }
    }

    public bool ShouldInteract(HoverOver hoveringOver)
    {
        switch (hoveringOver)
        {
            case HoverOver.Land:
                return m_IsMoveable;

            case HoverOver.Building:
            case HoverOver.Ship:
            case HoverOver.Submarine:
            case HoverOver.AirUnit:
                return m_IsAttackable;
                
            default:
                Debug.LogError("Switch hoverOver didn't work");
                return false;
        }
    }

    // Trigger reactions for unit creation
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), other.GetComponent<BoxCollider>());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        GetComponent<BoxCollider>().isTrigger = false;
    }

    private void CancelDeploy()
    {
        ((IDeployable)this).StopDeploy();
    }

    new void OnDestroy()
    {
        if (gameObject.layer == primaryPlayer.controlledLayer)
        {
            
        }

        //Remove object from selected manager
       m_selectedManager.RemoveFromSelected(this);
    }
}
