using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/*
    This is the <BoatMovement> script. This component is added to boats to give them
    the ability to move and navigate around. This scripts inherits from <SeaMovement>, which in turn
    inherits from an abstract class called <Movement>.

    The component interracts with <Seeker> component of A* Pathfinding Project. It also determines whether
    the unit is affected by ocean currents or not.
*/
[RequireComponent(typeof(RTSEntity))]
public class BoatMovement : SeaMovement {
    
    private Quaternion m_LookRotation;
    private Vector3 m_Direction;
    private bool m_PlayMovingSound = false;
    private bool m_SoundIsPlaying = false;
    private Seeker seeker;

    public bool AffectedByCurrent = true;
	public Rigidbody rb;    

    public float RotationalSpeed 
	{
		get;
		private set;
	}
	
	public float Acceleration
	{
		get;
		private set;
	}

    public override Vector3 TargetLocation
    {
        get
        {
            throw new NotImplementedException();
        }
    }
    
    // Use this for initialization
    void Start () 
	{
        seeker = GetComponent<Seeker>(); // Seeker is a pathfinding component attached to each gameobject that needs to move
		m_Parent = GetComponent<RTSEntity>(); // This unit
        rb = GetComponent<Rigidbody>(); // This unit's rigidbody
        //m_CurrentTile.SetOccupied(m_Parent, false);

    }

    private new void Update()
    {
        base.Update();

       
        if (Path == null)
        {
        return;
            /*for (int i = 1; i < Path.Count; i++)
            {
                Debug.DrawLine(Path[i - 1], Path[i]);
            } */
        }        

        if (Path != null /*&& Path.Count > 0*/)
        {
            if (Vector3.Distance(targetPosition, m_Parent.transform.position) < 10)
            {
                // Speed change
            }
            //We have a path, lets move!
            m_PlayMovingSound = true;
            AffectedByCurrent = false;
            MoveForward();
            
            //Make sure we're pointing at the target            
            if (!PointingAtTarget())
            {
                RotateTowards(targetPosition);
            }           
            
            UpdateCurrentTile();

            if (currentWaypoint >= Path.vectorPath.Count || Vector3.Distance(m_Parent.transform.position, targetPosition) < 7)
            {
                rb.velocity = Vector3.zero;
                Path = null;
                currentWaypoint = 0;
                AffectedByCurrent = true;
                return;
            }
        }
        else
        {
            m_PlayMovingSound = false;
            AffectedByCurrent = true;
        }

        if (m_PlayMovingSound && !m_SoundIsPlaying)
        {
            //sfx_Manager = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/" + m_Parent.Name + "/movement");
            //sfx_Manager.start();
            m_SoundIsPlaying = true;
        }
        else if (!m_PlayMovingSound && m_SoundIsPlaying)
        {
            //sfx_Manager.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //sfx_Manager.release();
            m_SoundIsPlaying = false;
        }
    }
    
    // Turning towards the destination
    private void RotateTowards(Vector3 location)
    {
        m_Direction = (location - m_Parent.transform.position).normalized;

        m_LookRotation = Quaternion.LookRotation(new Vector3(m_Direction.x, m_Direction.y * 0, m_Direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, m_LookRotation, Time.deltaTime * RotationalSpeed);
    }

    // Onward!
    private void MoveForward()
    {
        rb.AddForce(m_Parent.transform.forward * Speed);
    }

    // Has the unit reached its destination?
    private bool HasReachedDestination()
    {

        return false;
    }


    // Gives the moving command
    public override void MoveTo(Vector3 location)
    {
        seeker.StartPath(transform.position, location, OnPathComplete);
        targetPosition = location;
    }

    public override void Stop()
    {
        rb.velocity = Vector3.zero;
        Path = null;
        currentWaypoint = 0;
    }

    public override void Follow(Transform target)
    {
        MoveTo(target.position);
    }


    public override void AssignDetails(Item item)
    {
        Speed = item.Speed / 10;
        CurrentSpeed = 0;
        RotationalSpeed = item.RotationSpeed / 2;
        Acceleration = item.Acceleration;
    }

    private void UpdateCurrentTile()
    {
        
        
    }

    private bool PointingAtTarget()
    {
        Vector3 forwardVector = transform.forward;
        Vector3 targetVector = (Path.vectorPath[currentWaypoint]- transform.position).normalized;

        forwardVector.y = 0;
        targetVector.y = 0;

        float angle = Vector3.Angle(forwardVector, targetVector);
        Vector3 crossProduct = Vector3.Cross(forwardVector, targetVector);

        if (crossProduct.y < 0) angle *= -1;

        if (Mathf.Abs(angle) < 2.0f)
        {
            return true;
        }
        else
        {
            int direction = 1;
            if (angle < 0)
            {
                direction = -1;
            }

            transform.Rotate(0, RotationalSpeed * Time.deltaTime * direction, 0);
        }

        return false;
    }
    
}
 