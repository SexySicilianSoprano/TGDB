using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IManager {

    Player primaryPlayer();
    Player enemyPlayer();

	void BuildingAdded(Building building);
	void BuildingRemoved(Building building);
	void UnitAdded(Unit unit);
	void UnitRemoved(Unit unit);
	int GetUniqueID();	
	
	void AddResource(float amount);
	
	void AddResourceInstant(float amount);
	void RemoveResourceInstant(float amount);
	
	bool CostAcceptable(float cost);
	
	float Resource { get; }
}
