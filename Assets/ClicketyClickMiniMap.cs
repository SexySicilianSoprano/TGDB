using UnityEngine;
using System.Collections;

public class ClicketyClickMiniMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		Debug.Log ("Testing minimap click");
		Camera.main.GetComponent<MinimapArmi> ().MinimapClick ();
	}
}
