using UnityEngine;
using System.Collections;

/// <summary>
/// --- Projectile Behaviour ---
/// 
/// This script is attached to projectiles to give them the behaviour
/// of a bombshell. The object needs a trigger collider to work. since
/// explosion is triggered via collision.
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// </summary>

public class ProjectileBehaviour : MonoBehaviour {

    public Vector3 targetPosition;
    public RTSEntity parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position == targetPosition.normalized)
        {
            Debug.Log("Reached target position");
            Explode();
        }
	}

    // The projectile explodes, what happens and does someone get damaged?
    private void Explode()
    {
        Debug.Log("Explosion called, what happens?");
        // 
        foreach (RaycastHit hit in Sweep())
        {
            if (hit.collider.gameObject.GetComponent<RTSEntity>())
            {
                RTSEntity target = hit.collider.gameObject.GetComponent<RTSEntity>();

                if (Vector3.Distance(transform.position, target.transform.position) <= 3f)
                {
                    Debug.Log("Direct hit!");
                    target.TakeDamage(50f);
                }
                else
                {
                    Debug.Log("Outskirt hit, half damage dealt");
                    target.TakeDamage(25f);
                }
            }
        }

        Destroy(gameObject);
    }

    // Returns all hits within blast radius
    private RaycastHit[] Sweep()
    {
        return Physics.SphereCastAll(transform.position, 7f, transform.forward, 7f, ~(8 << 13));
    }
    
    // Triggers explosion
    private void OnTriggerEnter(Collider other)
    {
        // Get have some variables just to make the if-statement shorter
        Collider parentCol = parent.GetComponent<Collider>();
        Collider sphere = other.GetComponent<SphereCollider>();

        if (other.gameObject.GetComponent<Building>() && other != sphere || other.gameObject.GetComponent<Unit>() && !parentCol || other.tag == "Terrain")
        {
            // And iiiit's a hit! Call explosion and let's see what happens
            Debug.Log("We hit " + other.gameObject + "'s " + other.GetComponent<Collider>());
            Explode();
        }
    }
}
