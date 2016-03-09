using UnityEngine;
using System.Collections;

public class BaseZoneScript : MonoBehaviour {

    private Vector3 basePosition;

    // Update is called once per frame
    void Update()
    {
        basePosition = transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>() && other.gameObject.tag == "Player1")
        {
            other.gameObject.transform.SetParent(gameObject.transform);
            other.gameObject.GetComponent<BoatMovement>().AffectedByCurrent = true;
        }

        if (other.gameObject.GetComponent<Building>() && other.gameObject.tag == "Player1" && other.gameObject.GetComponent<BoxCollider>().isTrigger == false)
        {
            other.gameObject.transform.SetParent(gameObject.transform);            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<RTSEntity>())
        {
            other.gameObject.transform.parent = null;
        }
    }

    private void KeepRelativePosition(GameObject obj)
    {
 
    }
}
