using UnityEngine;
using System.Collections;

public class Selected : MonoBehaviour {

	public Rigidbody rb;
	public bool IsSelected
	{
		get;
		private set;
	}
	
	private bool m_JustBeenSelected = false;
	private float m_JustBeenSelectedTimer = 0;
		
	private float m_OverlayWidth = 0;
	private float m_OverlayLength = 0;
	private float m_MainMenuWidth;
	
	private Vector3 m_WorldExtents;

    //Player identifier variables
    private int primaryPlayer
    {
        get
        {
            return GameObject.Find("Manager").GetComponent<GameManager>().primaryPlayer().controlledLayer;
        }
    }

    // Use this for initialization
    void Start () 
	{		
		IsSelected = false;
		FindMaxWorldSize();
		
		//If this unit is land based subscribe to the path changed event
		LandMovement landMovement = GetComponent<LandMovement>();
		if (landMovement != null)
		{
			landMovement.PathChangedEvent += PathChanged;
		}
        
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update overlay rect
		Vector3 centerPoint = Camera.main.WorldToScreenPoint (transform.position);		
		
		if (m_JustBeenSelected)
		{
			m_JustBeenSelectedTimer += Time.deltaTime;
			
			if (m_JustBeenSelectedTimer >= 1.0f)
			{
				m_JustBeenSelectedTimer = 0;
				m_JustBeenSelected = false;
			}
		}
	}
	
	private void FindMaxWorldSize()
	{
		//Calculate size of overlay based on the objects size
		Vector3 worldSize = GetComponent<BoxCollider>().bounds.extents;
		float maxDimension = Mathf.Max (worldSize.x, worldSize.z);
		m_WorldExtents = new Vector3(maxDimension, 0, 0);
	}
		
	
	void OnGUI()
	{
		
	}
	
	public void SetSelected()
	{
		if (gameObject.layer == primaryPlayer)
        {                        
            IsSelected = true;
			m_JustBeenSelected = true;
			m_JustBeenSelectedTimer = 0;
			
            GetComponent<VehicleMovement>().AffectedByCurrent = false;
        }

	}
	
	public void SetDeselected()
	{
		IsSelected = false;
		m_JustBeenSelected = false;		

        GetComponent<VehicleMovement>().AffectedByCurrent = true;
	}
	
	public void AssignGroupNumber(int number)
	{
		
	}
	
	public void RemoveGroupNumber()
	{
		
	}
	
	private void GLExecuteFunction()
	{
		//Need to get target location
		Vector3 target = GetComponent<Movement>().TargetLocation;
		
		if (IsSelected && target != Vector3.zero)
		{
			Vector3 screenTarget = Camera.main.WorldToScreenPoint (target);
			Vector3 screenPosition = Camera.main.WorldToScreenPoint (transform.position);
			
			screenTarget.z = 0;
			screenPosition.z = 0;
		
		}
	}
	
	private void PathChanged()
	{
		if (IsSelected)
		{
			m_JustBeenSelected = true;
			m_JustBeenSelectedTimer = 0;
		}
	}
}
