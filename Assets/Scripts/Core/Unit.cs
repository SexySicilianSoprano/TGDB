using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Selected))]
public class Unit : RTSEntity{
	
	//Member Variables
    protected bool m_IsMoveable = true;	
	protected bool m_IsDeployable = false;
	protected bool m_IsAttackable = true;
	protected bool m_IsInteractable = false;

	
	protected void Start()
	{
        /*
		m_IsDeployable = this is IDeployable;
		m_IsAttackable = this is IAttackable;
		m_IsInteractable = this is IInteractable;
        */
    }
	
	protected void Update()
	{
		
	}
		
	public override void SetSelected ()
	{
		
	}

	public override void SetDeselected ()
	{
		
	}

	public override void AssignToGroup (int groupNumber)
	{

	}

	public override void RemoveFromGroup ()
	{

	}
	
	public override void ChangeTeams(int team)
	{
		
	}

	public bool IsDeployable ()
	{
		return m_IsDeployable;
	}

	public bool IsAttackable ()
	{
        return m_IsAttackable;
	}

	public bool IsMoveable ()
	{
		return m_IsMoveable;
	}

	public bool IsInteractable ()
	{
		return m_IsInteractable;
	}

    public void GiveOrder ()
	{
		
	}

	public bool ShouldInteract ()
	{
        return false;
	}

    // Trigger reactions for unit creation
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == playerLayer && other == GetComponent<BoxCollider>())
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
		
	}
   
    new void OnDestroy()
	{
        
	}
}
