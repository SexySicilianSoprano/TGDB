using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class BuildingBeingPlaced : MonoBehaviour {

    public GameObject collidingObject;

    void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "BuildingSpot")
        {
            Debug.Log("Entered BuildingSpot");
            collidingObject = other.gameObject;
        }
        else
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
        }

    }

    void OnTriggernExit(Collider other)
    {
        collidingObject = null;
    }
        
	void OnDestroy()
	{

	}
}
