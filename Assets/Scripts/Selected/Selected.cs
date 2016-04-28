using UnityEngine;
using System.Collections;

public class Selected : MonoBehaviour {

	public Rigidbody rb;
    public Projector projector;
    public TrailRenderer trail;
    public HPBar hpbar;

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
    private string primaryPlayer
    {
        get
        {
            return GameObject.Find("Manager").GetComponent<GameManager>().primaryPlayer().controlledTag;
        }
    }

    // Use this for initialization
    void Start () 
	{
        projector = GetComponentInChildren<Projector>();
        hpbar = GetComponent<HPBar>();
		IsSelected = false;
		FindMaxWorldSize();
		
		//If this unit is land based subscribe to the path changed event
		SeaMovement seaMovement = GetComponent<SeaMovement>();
		if (seaMovement != null)
		{
			seaMovement.PathChangedEvent += PathChanged;
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
		if (gameObject.tag == primaryPlayer)
        {                        
            IsSelected = true;
			m_JustBeenSelected = true;
			m_JustBeenSelectedTimer = 0;
            GetComponent<BoatMovement>().AffectedByCurrent = false;            
        }

        projector.enabled = true;
        hpbar.ShowHealthBar();
	}
	
	public void SetDeselected()
	{
		IsSelected = false;
		m_JustBeenSelected = false;
        projector.enabled = false;
        GetComponent<BoatMovement>().AffectedByCurrent = true;
        hpbar.HideHealthBar();
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
		Vector3 target = GetComponent<Movement>().targetPosition;
		
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
