using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour, ICamera {
	
	//Singleton
	public static MainCamera main;
	
	//Camera Variables
	public float HeightAboveGround = 30.0f;
	public float AngleOffset = 20.0f;
	public float m_MaxFieldOfView = 85.0f;
	public float m_MinFieldOfView = 20.0f;

    //Input-related variables
    public Vector3 i_MousePos;
    public float i_ScrollEvent;
    public float ScrollSpeed = 8.0f;
	public float ScrollAcceleration = 30.0f;	
	public float ZoomRate = 500.0f;
    public int MouseScrollSpeed = 0;
	
	private bool canWeScroll = true;
    private bool atScreenEdge = false;
    private float atScreenEdgeCounter = 0;
	
	public GameObject StartPoint;
	
	private Rect m_Boundries;
	
	void Awake()
	{
		main = this;
	}

	// Use this for initialization
	void Start () 
	{	
		//Set up camera position
		if (StartPoint != null)
		{
			transform.position = new Vector3(StartPoint.transform.position.x, HeightAboveGround, StartPoint.transform.position.z-AngleOffset);
		}
		
		//Set up camera rotation
		transform.rotation = Quaternion.Euler (90-AngleOffset, 0, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
        CheckScreenEdgeEvents();
    }
	
	public void Pan(object sender)
	{
		if (canWeScroll)
		{
			float totalSpeed = MouseScrollSpeed*ScrollAcceleration;
			float targetSpeed = totalSpeed < ScrollSpeed ? totalSpeed : ScrollSpeed;
			
			transform.Translate (i_MousePos.x*Time.deltaTime*targetSpeed, 0, i_MousePos.y*Time.deltaTime*targetSpeed, Space.World);
			
			//Check if we have scrolled past edge
			if (transform.position.x < m_Boundries.xMin)
			{
				transform.position = new Vector3(m_Boundries.xMin, transform.position.y, transform.position.z);
			}
			else if (transform.position.x > m_Boundries.xMax)
			{
				transform.position = new Vector3(m_Boundries.xMax, transform.position.y, transform.position.z);
			}
			
			if (transform.position.z < m_Boundries.yMin)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y, m_Boundries.yMin);
			}
			else if (transform.position.z > m_Boundries.yMax)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y, m_Boundries.yMax);
			}
			
			CheckEdgeMovement ();
		}
	}
	
	public void Move(Vector3 worldPos)
	{
		transform.position = new Vector3(worldPos.x, transform.position.y, worldPos.z);
		CheckEdgeMovement ();
	}
	
	private void CheckEdgeMovement()
	{
		Ray r1 = Camera.main.ViewportPointToRay (new Vector3(0,1,0));
		Ray r2 = Camera.main.ScreenPointToRay (new Vector3(Screen.width, Screen.height-1,0));
		Ray r3 = Camera.main.ViewportPointToRay (new Vector3(0,0,0));
		
		float left, right, top, bottom;
		
		RaycastHit h1;
		
		Physics.Raycast (r1, out h1, Mathf.Infinity, 1<< 16);		
		left = h1.point.x;
		top = h1.point.z;
		
		Physics.Raycast (r2, out h1, Mathf.Infinity, 1<< 16);
		right = h1.point.x;
		
		Physics.Raycast (r3, out h1, Mathf.Infinity, 1<< 16);
		bottom = h1.point.z;
		
		if (left < m_Boundries.xMin)
		{
			Camera.main.transform.Translate (new Vector3(m_Boundries.xMin-left,0,0), Space.World);
		}
		else if (right > m_Boundries.xMax)
		{
			Camera.main.transform.Translate (new Vector3(m_Boundries.xMax-right,0,0), Space.World);
		}
		
		if (bottom < m_Boundries.yMin)
		{
			Camera.main.transform.Translate (new Vector3(0,0,m_Boundries.yMin-bottom), Space.World);
		}
		else if (top > m_Boundries.yMax)
		{
			Camera.main.transform.Translate (new Vector3(0,0,m_Boundries.yMax-top), Space.World);
		}
	}
	
	public void Zoom(object sender)
	{
        
		GetComponent<Camera>().fieldOfView -= MouseScrollSpeed*ZoomRate*Time.deltaTime;
		
		if (GetComponent<Camera>().fieldOfView < m_MinFieldOfView) 
		{
			GetComponent<Camera>().fieldOfView = m_MinFieldOfView;
		}
		else if (GetComponent<Camera>().fieldOfView > m_MaxFieldOfView) 
		{
			GetComponent<Camera>().fieldOfView = m_MaxFieldOfView;
		}
		
		CheckEdgeMovement();
	}
	
	public void DisableScrolling()
	{
		canWeScroll = false;
	}
	
	public void EnableScrolling()
	{
		canWeScroll = true;
	}

	public void SetBoundries (float minX, float minY, float maxX, float maxY)
	{
		m_Boundries = new Rect();
		m_Boundries.xMin = minX;
		m_Boundries.xMax = maxX;
		m_Boundries.yMin = minY+1;
		m_Boundries.yMax = maxY;
	}
	
    private void CheckScreenEdgeEvents()
    {
        atScreenEdge = false;

        if (Input.mousePosition.x == 0)
        {

            atScreenEdge = true;
        }

        if (Input.mousePosition.x >= Screen.width * 0.98f)
        {

            atScreenEdge = true;
        }

        if (Input.mousePosition.y == 0)
        {

            atScreenEdge = true;
        }

        if (Input.mousePosition.y >= Screen.height * 0.98f)
        {

        }

        if (atScreenEdge)
        {
            atScreenEdgeCounter += Time.deltaTime;
            Debug.Log("At Edge lol");
        }
        else
        {
            atScreenEdgeCounter = 0;
        }
    }
}
