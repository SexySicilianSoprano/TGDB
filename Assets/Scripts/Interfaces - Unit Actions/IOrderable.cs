using UnityEngine;
using System.Collections;

public interface IOrderable {
	
	bool IsDeployable();
	bool IsAttackable();
	bool IsMoveable();
	bool IsInteractable();
    bool IsGatherable();
	
	void GiveOrder(Order order);
	
	bool ShouldInteract(HoverOver hoveringOver);
}
