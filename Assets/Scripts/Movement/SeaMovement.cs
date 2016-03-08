using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System;

public abstract class SeaMovement : Movement {

    // Target location
    public Vector3 targetPosition;

    // Calculated path
    public Path Path;
        
    // Waypoint we're currently moving towards
    public int currentWaypoint = 0;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 10;

    public Action PathChangedEvent { get; internal set; }

    // Path complete callback
    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            Path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }

    protected void Update() { }
}
