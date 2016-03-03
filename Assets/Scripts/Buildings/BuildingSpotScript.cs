using UnityEngine;
using System.Collections;

public class BuildingSpotScript : MonoBehaviour {

    public bool isOccupied = false;

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<RTSEntity>())
        {
            isOccupied = true;
        }
    }
}
