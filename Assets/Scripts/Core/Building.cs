using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SelectedBuilding))]
public class Building : RTSEntity{

    protected bool m_IsMoveable = true;
    protected bool m_IsDeployable = false;
    protected bool m_IsAttackable = true;
    protected bool m_IsInteractable = false;

    private bool sellable = false;

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

    public void Start()
	{        
        
    }

    public void Update()
    {
        AstarPath.active.UpdateGraphs(GetComponent<BoxCollider>().bounds);
    }

    public int BuildingIdentifier
    {
        get; set;
    }
	
	public bool InteractWith(IOrderable obj)
	{
		return false;
	}
	
	public bool CanSell()
	{
		return sellable;
	}

	public override void SetSelected ()
	{
        GetComponent<SelectedBuilding>().SetSelected();
	}

	public override void SetDeselected ()
    {
        GetComponent<SelectedBuilding>().SetDeselected();
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
}
