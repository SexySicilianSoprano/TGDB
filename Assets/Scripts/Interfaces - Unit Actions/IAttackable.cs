using UnityEngine;
using System.Collections;

public interface IAttackable {
	
	void Attack(RTSEntity obj);
    void Stop();

}
