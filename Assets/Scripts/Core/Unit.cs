using UnityEngine;
using System.Collections;
using System;

//[RequireComponent(typeof(Selected))]
public class Unit : RTSEntity, IOrderable{

    protected SelectedManager m_selectedManager
    {
        get
        {
            return GameObject.Find("Manager").GetComponent<SelectedManager>();
        }
    }

    protected UIManager m_UIManager
    {
        get
        {
            return GameObject.Find("Manager").GetComponent<UIManager>();
        }
    }

    private Player primaryPlayer()
    {
        return m_UIManager.primaryPlayer();
    }

    protected void Start()
    {
        
    }

    protected void Update()
    {
        AstarPath.active.UpdateGraphs(GetComponent<BoxCollider>().bounds);
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
            case Const.TEAM_gearsHouse:

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
        return GetComponent<Combat>();
    }

    public bool IsMoveable()
    {
        return GetComponent<Movement>();
    }

    public bool IsInteractable()
    {
        return false;
    }

    public bool IsGatherable()
    {
        return GetComponent<ResourceGathering>();
    }
    
    public void GiveOrder(Order order)
    {
        switch (order.OrderType)
        {
            // Stop Order
            case Const.ORDER_STOP:

                if (IsAttackable())
                {
                    GetComponent<Combat>().Stop();
                }

                if (IsGatherable())
                {
                    GetComponent<ResourceGathering>().Stop();
                }

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
                
                if (IsAttackable())
                {
                    GetComponent<Combat>().Stop();
                }

                if (IsGatherable())
                {
                    GetComponent<ResourceGathering>().Stop();
                }

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
                // TODO: actual deployable shit
                GetComponent<Movement>().Stop();
                break;

            // Attack Order
            case Const.ORDER_ATTACK:
                
                if (IsAttackable())
                {
                    // Stop combat
                    GetComponent<Combat>().Stop();

                    // Attack                    
                    GetComponent<Combat>().AttackCommand(order.Target);
                }
                break;

            case Const.ORDER_GATHER:
                if (IsGatherable())
                {
                    // Stop gathering and give new gathering order
                    GetComponent<ResourceGathering>().Stop();
                    GetComponent<ResourceGathering>().Gather(order.Mine);
                }
                break;

        }
    }

    public bool ShouldInteract(HoverOver hoveringOver)
    {
        switch (hoveringOver)
        {
            case HoverOver.Land:
                return IsMoveable();

            case HoverOver.Building:
            case HoverOver.Ship:
            case HoverOver.Submarine:
            case HoverOver.AirUnit:
                return IsAttackable();

            case HoverOver.Mine:
                return IsGatherable();
                
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
        if (gameObject.layer == primaryPlayer().controlledLayer)
        {
            
        }

        //Remove object from selected manager
       m_selectedManager.RemoveFromSelected(this);
       m_selectedManager.RemoveFromGroup(this);
    }

    
}
