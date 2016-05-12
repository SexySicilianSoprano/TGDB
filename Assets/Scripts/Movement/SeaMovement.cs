using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System;

public abstract class SeaMovement : Movement {

    // Target location
    protected Vector3 targetLocation;

    // Pathfinding variables
    protected float repathRate = 2;
    protected float lastRepath;
    protected bool canSearchAgain = false;
    public bool m_OnMyWay = false;

    // Calculated path
    public Path Path;
        
    // Waypoint we're currently moving towards
    public int currentWaypoint = 0;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 10;

    public Action PathChangedEvent { get; internal set; }

    // Path complete callback
    public void OnPathComplete(Path _p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!_p.error)
        {
            if (Path != null)
            {
                Path.Release(this);
            }

            Path = _p;
            canSearchAgain = true;

            // Claim a new path
            Path.Claim(this);

            //Reset the waypoint counter
            currentWaypoint = 0;
            m_OnMyWay = true;
        }
        else
        {
            Path.Release(this);
            Path = null;
        }
    }    
}
