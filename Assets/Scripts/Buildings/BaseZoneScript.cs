using UnityEngine;
using System.Collections;

public class BaseZoneScript : MonoBehaviour {

    private Vector3 basePosition;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        basePosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>())
        {
            other.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Unit>())
        {
            other.gameObject.transform.parent = null;
        }
    }

    private void KeepRelativePosition(GameObject obj)
    {
 
    }
}
