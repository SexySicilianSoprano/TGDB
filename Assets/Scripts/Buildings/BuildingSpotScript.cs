using UnityEngine;
using System.Collections;

public class BuildingSpotScript : MonoBehaviour {

    public bool isOccupied = false;

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<Unit>())
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<Unit>())
        {
            isOccupied = false;
        }
    }
}
