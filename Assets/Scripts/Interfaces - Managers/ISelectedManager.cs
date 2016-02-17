using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISelectedManager {

	void AddToSelected(RTSEntity unit);
	void RemoveFromSelected(RTSEntity unit);
    void RemoveFromGroup(RTSEntity unit);
	void ClearSelected();
	void CreateGroup(int number);
	void SelectGroup(int number);
	void GiveOrder(Order order);
	
	int ActiveEntityCount();
	
	bool IsEntitySelected(GameObject obj);
	
	IOrderable FirstActiveEntity();
	List<IOrderable>ActiveEntityList();
}
