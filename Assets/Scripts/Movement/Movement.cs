using UnityEngine;
using System.Collections;

public abstract class Movement : MonoBehaviour{
	
	protected RTSEntity m_Parent;
	protected Vector3 m_Position = new Vector3();

    public Vector3 targetPosition;

    public float Speed { get; protected set; }
	public float CurrentSpeed { get; protected set; }
    public bool stayInPlace = false;

    public abstract bool onTheMove { get; }
    
	public abstract void MoveTo (Vector3 location);

	public abstract void Follow (Transform target);

	public abstract void Stop ();
	
	public abstract void AssignDetails(Item item);

    public abstract void MoveForward();

    public abstract void RotateTowards(Vector3 location);

    public abstract bool PointingAtTarget(Vector3 direction);
}
