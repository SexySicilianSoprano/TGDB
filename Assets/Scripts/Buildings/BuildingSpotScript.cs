using UnityEngine;
using System.Collections;

public class BuildingSpotScript : MonoBehaviour {

    public bool isOccupied = false;
    private Projector projector;

    void Start()
    {
        projector = transform.Find("USProjector").GetComponent<Projector>();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.GetComponent<Unit>() && collider == GetComponent<BoxCollider>())
        {
            isOccupied = true;
            projector.enabled = false;

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<Unit>() && collider == GetComponent<BoxCollider>())
        {
            isOccupied = false;
            projector.enabled = true;
        }
    }
}
