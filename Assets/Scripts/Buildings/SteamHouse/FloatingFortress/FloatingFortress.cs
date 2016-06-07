using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class FloatingFortress : Building {

	// Use this for initialization
	new void Start () 
	{
		//Assign all the details
		AssignDetails (ItemDB.FloatingFortress);
		
		//Tell the base class to start as well, must be done after AssignDetails
		base.Start();
	}

    new void Update()
    {
        base.Update();
        AstarPath.active.UpdateGraphs(GetComponent<BoxCollider>().bounds);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //showCanvasScript.ToggleCanvasConst();
        //doesn't work, need to get this work to get the thing optimized
    }
}
