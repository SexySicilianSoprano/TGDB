using UnityEngine;
using System.Collections;

public class IgnorePhysics : MonoBehaviour {

	void OnCollisionEnter(Collision other)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), other.collider);
    }
}
