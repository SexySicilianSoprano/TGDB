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
    private bool isRotating = false;
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
        controller = GetComponent<RVOController>();
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
        Debug.Log("On my way: " + m_OnMyWay);

        // No path
        if (Path == null)
        {
            return;
        }

        // We have a path
        if (Path != null /*&& Path.Count > 0*/)
        {
            if (!trail.enabled)
            {
                trail.enabled = true;
            }

            // If we're close enough to the next waypoint, jump to next one
            if (Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance && currentWaypoint < Path.vectorPath.Count || Vector3.Distance(transform.position, Path.vectorPath[currentWaypoint]) < nextWaypointDistance && currentWaypoint <= Path.vectorPath.Count)
            {
                if (Path.vectorPath.Count > currentWaypoint)
                {
                    currentWaypoint++;

                    if (Path.vectorPath.Count <= currentWaypoint)
                    {
                        rb.velocity = Vector3.zero;
                        controller.Move(Vector3.zero);
                        Path = null;
                        currentWaypoint = 0;
                        trail.enabled = false;
                        m_OnMyWay = false;
                        canSearchAgain = false;
                        return;
                    }
                }
            }

            // Pick the direction towards the next waypoint
            Vector3 dir = (Path.vectorPath[currentWaypoint] - transform.position).normalized;

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

    // Onward!
    public override void MoveForward()
    {
        controller.Move(m_Parent.transform.forward * Speed);
        //rb.AddForce(m_Parent.transform.forward * Speed);
    }

    // Check if something is in front of you, calculate a new route
    public override bool CheckFront()
    { 
        RaycastHit hit;
        Ray ray = new Ray(m_Parent.transform.position, m_Parent.transform.forward);
        if (Physics.Raycast(ray, out hit, 10))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    // Gives the moving command
    public override void MoveTo(Vector3 location)
    {
        targetLocation = location;
        seeker.StartPath(transform.position, location, OnPathComplete);
    }

    // Stop moving and set path to null
    public override void Stop()
    {
        controller.Move(Vector3.zero);
        rb.velocity = Vector3.zero;
        Path = null;
        m_OnMyWay = false;
        currentWaypoint = 0;
        trail.enabled = false;
    }

    // Go towards the target
    public override void Follow(Transform target)
    {
        MoveTo(target.position);
    }

    // Assigne details
    public override void AssignDetails(Item item)
    {
        Speed = item.Speed / 3;
        CurrentSpeed = 0;
        RotationalSpeed = item.RotationSpeed *2;
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

    private void CloseIn(Vector3 target)
    {
        if (PointingAtTarget(target))
        {
            MoveForward();
        }
        RotateTowards(target);        
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
        if (Time.time - lastRepath >= repathRate && canSearchAgain)
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
 