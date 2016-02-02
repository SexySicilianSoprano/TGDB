using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class BuildingBeingPlaced : MonoBehaviour {

    private List<Material> m_BuildingMaterials = new List<Material>();
    private List<Color> m_ValidBuildingColors = new List<Color>();
    private List<Color> m_InvalidBuildingColors = new List<Color>();

    private float m_AlphaValue = 0.7f;

    public bool BuildValid = true;
    
    // Use this for initialization
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.position = Input.mousePosition;
    }

    public void SetToValid()
    {

    }

    public void SetToInvalid()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.layer == 9 || other.gameObject.layer == 12 || other.gameObject.layer == 3)
        {
            BuildValid = false;
        }
        else
        {
            BuildValid = true;
        }    
    }

    void OnTriggerExit(Collider other) {
        BuildValid = true;
    }
        
	void OnDestroy()
	{

	}
}
