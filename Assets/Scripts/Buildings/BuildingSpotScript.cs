using UnityEngine;
using System.Collections;

public class BuildingSpotScript : MonoBehaviour {

    public bool isOccupied = false;
    private Projector projector;

    void Start()
    {
        projector = transform.Find("USProjector").GetComponent<Projector>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Unit>())
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collider.GetComponentInChildren<SphereCollider>());
        }

        if (collider.gameObject.GetComponent<Unit>())
        {
            isOccupied = true;
            projector.enabled = false;
            Debug.Log("Get Occupied, bitch!");

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.GetComponent<Unit>())
        {
            isOccupied = false;
            projector.enabled = true;
            Debug.Log("Release occupation");
        }
    }
}
