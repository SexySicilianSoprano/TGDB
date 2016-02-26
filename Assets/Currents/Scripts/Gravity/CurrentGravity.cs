using UnityEngine;
using System.Collections;

/// <summary>
/// This is the <CurrentGravity> script. Name's kinda stupid, I know.
/// This component is attached to objects that become pieces of a current.
/// Adjust velocity from Unity editor, note that the velocity is divided by 10 to give more
/// room for fine-tuning if need be.So probably you should add more than 100 to actually move something.
///
/// Of course, feel free to change this anytime.
///
/// As you may notice, the direction where force is added comes from the component's host
/// gameobject's transform.forward, which means you only need to point the object's blue arrow in editor
/// towards the direction you want force be added to.
///
/// - Karl Sartorisio
///
/// The Great Deep Blue
/// </summary>

public class CurrentGravity : MonoBehaviour {

    // Adjust velocity to add more force into colliding objects
    public float velocity = 0;

    // Adjust divider if you want to have bigger numbers to work with, in case you want smaller or larger room for adjustments
    private float divider = 100f;

    // Collides with things, make sure this gameobject's collider is a trigger, otherwise it won't work
   	void OnTriggerStay (Collider other)
    {
        // Is the colliding object a building?      
        if (other.gameObject.GetComponent<Building>())
        {            
            // Just move with the current, baby! Add force into colliding object
            other.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * (velocity / divider));
            Debug.Log("testi");
        } 
        // Is the colliding object an unit that's affected by currents?
        else if (other.gameObject.GetComponent<Unit>() && other.gameObject.GetComponent<BoatMovement>().AffectedByCurrent)
        {            
            // Just move with the current, baby! Add force into colliding object
            other.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * (velocity / divider));
        }       
    }
}
