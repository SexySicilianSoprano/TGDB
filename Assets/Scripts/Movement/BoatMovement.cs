using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Pathfinding.RVO;
using Pathfinding;

/// <summary>
/// 
///   This is the <BoatMovement> script. This component is added to boats to give them
///   the ability to move and navigate around. This scripts inherits from <SeaMovement>, which in turn
///   inherits from an abstract class called <Movement>.
/// 
///   The component interracts with <Seeker> component of A* Pathfinding Project. It also determines whether
///   the unit is affected by ocean currents or not.
///    
///   - Karl Sartorisio
///   The Great Deep Blue
///   
/// </summary>

[RequireComponent(typeof(RTSEntity))]
public class BoatMovement : SeaMovement {
    
    // Variables for rotation and direction
    private Quaternion m_LookRotation;

    // Booleans
    private bool m_PlayMovingSound = false; // True calls for playing the sound, false stops it
    private bool m_SoundIsPlaying = false; // Is the sound currently playing or not?
    private bool m_Waiting = false; // Is this unit waiting?
    private bool isRotating = false; // Is this unit rotating? (not sure if in use)
    public bool AffectedByCurrent = true; // Is this unit affected by ocean currents?    
    public bool moving = false;

    // Use this outside this script to determine if we're moving or not
    public override bool onTheMove
    {
        get
        {
            if (isInCombat)
            {
                return m_OnMyWay;
            }
            else
            {
                return false;
            }
        }
    }

    private bool isInCombat
    {
        get
        {
            if (GetComponent<Combat>())
            {
                return GetComponent<Combat>().isInCombat;
            }
            else return false;
        }
    }

    // Variable for Seeker-component
    private Seeker seeker;
    private RVOController controller;
    private TrailRenderer trail;
    private GridGraph grid;

    // Variable for rigidbody
	public Rigidbody rb;

    // Velocity values
    public float RotationalSpeed;
    public float Acceleration;
    
    // Use this for initialization
    void Start () 
	{
        seeker = GetComponent<Seeker>(); // Seeker is a pathfinding component attached to each gameobject that needs to move
        trail = GetComponentInChildren<TrailRenderer>(); // Trail renderer for water trail effect
		m_Parent = GetComponent<RTSEntity>(); // This unit
        rb = GetComponent<Rigidbody>(); // This unit's rigidbody        

        seeker.pathCallback += OnPathComplete;

        StartCoroutine(RepeatRepath());
    }

    // Called in every frame at fixed rate to reduce FPS exploitation and performance issues
    private void FixedUpdate()
    {
        //Debug.Log("Total: " + Path.vectorPath.Count + " - Waypoint: " + currentWaypoint);
        //Debug.Log("On my way: " + m_OnMyWay);

        // No path
        if (Path == null)
        {
            return;
        }

        // We have a path
        if (Path != null /*&& Path.Count > 0*/)
        {
            // Pick the direction towards the next waypoint
            Vector3 dir = CheckBestRoute();
            
            if (m_Waiting)
            {
                rb.velocity = Vector3.zero;
                return;
            }

            // If we're close enough to the next waypoint, jump to next one
            if (Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance * 3 && currentWaypoint < Path.vectorPath.Count || Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance && currentWaypoint <= Path.vectorPath.Count)
            {
                if (currentWaypoint <= Path.vectorPath.Count)
                {
                    currentWaypoint++;
                }

                if (Path.vectorPath.Count <= currentWaypoint)
                {
                    Stop();
                    return;
                }  
            }

            // We have a path, lets move!
            m_PlayMovingSound = true;
            AffectedByCurrent = false;

            if (m_OnMyWay)
            {
                MoveForward();

                if (!PointingAtTarget(dir))
                {
                    RotateTowards(dir);
                }
               
            }
        }
        else
        {
            // We're not moving, so stop playing moving sounds and let the current a
            m_PlayMovingSound = false;

            // Are we in the base area? If so, follow the base
            if (gameObject.transform.parent)
            {
                AffectedByCurrent = false;
                gameObject.transform.position = gameObject.transform.localPosition;
            }
            else if (stayInPlace)
            {
                AffectedByCurrent = false;
            }
            else
            {
                AffectedByCurrent = true;
            }
        }

        // Play and stop sound when called
        if (m_PlayMovingSound && !m_SoundIsPlaying)
        {
            m_SoundIsPlaying = true;
        }
        else if (!m_PlayMovingSound && m_SoundIsPlaying)
        {
            m_SoundIsPlaying = false;
        }
    }
    
    // Turning towards the destination
    public override void RotateTowards(Vector3 dir)
    {        
        Vector3 m_Direction = dir;

        m_LookRotation = Quaternion.LookRotation(new Vector3(m_Direction.x, m_Direction.y * 0, m_Direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, m_LookRotation, Time.deltaTime * RotationalSpeed);
                
    }

    // Checks best route from last point downwards, I call it "the countdown pathsearch".
    // Basically what it does is raycast towards each waypoint from last to first, detecting if anything is in between,
    // either returning a path as Vector3 direction or lowering the index to search a closer waypoint to pick
    // Far from best, but somewhat effective and fast. So far no idea how it works on slower PCs or when stressed.
    private Vector3 CheckBestRoute()
    {
        LayerMask mask = 8 << 23;
        List<Vector3> vPath = Path.vectorPath;
        int index = vPath.Count-1;

        while (true)
        {
            float distance = Vector3.Distance(transform.position, vPath[index]);
            RaycastHit hit;
            Vector3 castPos = new Vector3(transform.position.x, 2f, transform.position.z);
            Vector3 castTarg = new Vector3(vPath[index].x, 2f, vPath[index].z);
            Ray ray = new Ray(castPos, (vPath[index] - transform.position).normalized);

            if (Physics.Raycast(ray, out hit, distance, ~mask))
            {
                index--;

                if (hit.transform.gameObject.layer == 9 && hit.distance < 8 && hit.transform.GetComponent<Movement>().onTheMove)
                {
                    m_Waiting = true;
                }
                else
                {
                    m_Waiting = false;
                }
            }
            else
            {
                currentWaypoint = index;
                return (vPath[index] - transform.position).normalized;
            }
        }
    }

    // Onward!
    public override void MoveForward()
    {
        //controller.Move(m_Parent.transform.forward * Speed);
        rb.AddForce(transform.forward * Speed);

        if (!trail.enabled)
        {
            trail.enabled = true;
        }
    }
         
    // Gives the moving command
    public override void MoveTo(Vector3 location)
    {
        m_Waiting = false;
        targetLocation = location;
        seeker.StartPath(transform.position, location, OnPathComplete);
    }

    // Stop moving and set path to null
    public override void Stop()
    {
        //controller.Move(Vector3.zero);
        rb.velocity = Vector3.zero;
        Path = null;
        canSearch = false;
        canSearchAgain = false;
        m_OnMyWay = false;
        currentWaypoint = 0;
        trail.enabled = false;
        m_Waiting = false;
    }

    // Go towards the target
    public override void Follow(Transform target)
    {
        MoveTo(target.position);
    }

    // Assigne details
    public override void AssignDetails(Item item)
    {
        Speed = item.Speed / 6;
        CurrentSpeed = 0;
        RotationalSpeed = item.RotationSpeed / 3;
        Acceleration = item.Acceleration / 3;
    }

    // Is the unit facing target direction?
    public override bool PointingAtTarget(Vector3 direction)
    {
        // Set unit's forward vector and the direction we're going towards
        Vector3 forwardVector = transform.forward;
        Vector3 targetVector = direction;

        // Since we're moving on the land (ocean surface, but still), we can't have Vector.y to be be any highger
        forwardVector.y = 0;
        targetVector.y = 0;

        // Get he angle and cross product of previously set vectors
        float angle = Vector3.Angle(forwardVector, targetVector);
        Vector3 crossProduct = Vector3.Cross(forwardVector, targetVector);

        // If cross product is less than zero, adjust angle
        if (crossProduct.y < 0) angle *= -1;

        // If we're angle is close enough, we're facing the target
        if (Mathf.Abs(angle) < 2.0f)
        {
            return true;
        }
        else
        { 
            // Since we're not, we need to turn the ship and return false for now
            int dir = 1;
            if (angle < 0)
            {
                dir = -1;
            }

            transform.Rotate(0, RotationalSpeed * dir * Time.deltaTime, 0);
        }

        return false;
    }

    IEnumerator RepeatRepath()
    {
        while (true)
        {
            float v = TrySearchPath();
            yield return new WaitForSeconds(v);
        }
    }

    public float TrySearchPath()
    {
        if (Time.time - lastRepath >= repathRate && canSearch && canSearchAgain)
        {
            SearchPath();
            return repathRate;
        }
        else
        {
            float v = repathRate - (Time.time - lastRepath);
            return v < 0 ? 0 : v;
        }
    }

    /** Requests a path to the target */
    public virtual void SearchPath()
    {
        lastRepath = Time.time;
        
        canSearchAgain = false;
        //We should search from the current position
        seeker.StartPath(transform.position, targetLocation, OnPathComplete);
    }
}
 