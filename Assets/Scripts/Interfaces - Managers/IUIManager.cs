using UnityEngine;
using System.Collections;
using System;

public interface IUIManager 
{
    Player primaryPlayer();
    Player enemyPlayer();

	bool IsShiftDown
	{
		get;
		set;
	}
	
	bool IsControlDown
	{
		get;
		set;
	}
	
	Mode CurrentMode
	{
		get;
	}       

    HoverOver HoverOverState
    {
        get;
    }

    InteractionState CurrentState
    {
        get;
    }
	
	bool IsCurrentUnit(RTSEntity obj);	
	
	void UserPlacingBuilding(Item item, Action callbackFunction);
    
    void SwitchMode(Mode mode);

}
