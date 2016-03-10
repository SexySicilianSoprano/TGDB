using UnityEngine;
using System.Collections;

public class SelectedBuilding : MonoBehaviour {
	
	private Vector3[] WorldVertices = new Vector3[8];
	private Vector3[] WorldHealthVertices = new Vector3[8];
	
	private Vector3[] ScreenVertices = new Vector3[8];
	private Vector3[] ScreenHealthVertices = new Vector3[8];
	
	private Building m_Building;
	
	private float m_HealthSize = 2.0f;
	private float m_HealthWidth;

    Projector projector;
    
    // Use this for initialization
    void Start () 
	{
        // Find projector
        projector = transform.Find("Projector").GetComponent<Projector>();        

        //Assign building
        m_Building = GetComponent<Building>();
	}
	
	public void SetSelected()
	{
        //Render projection
        if (projector.enabled == false)
            projector.enabled = true;
	}
	
	public void SetDeselected()
	{
        if (projector.enabled == true)
        projector.enabled = false;
    }
	
	public void ExecuteFunction()
	{
		
	}
	
	void OnDestroy()
	{
		//If this gets destroyed make sure to remove any selection
		SetDeselected ();
	}
}
