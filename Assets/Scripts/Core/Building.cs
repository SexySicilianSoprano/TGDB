using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(SelectedBuilding))]
public class Building : RTSEntity {
	
	private bool sellable = false;
	
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

}
