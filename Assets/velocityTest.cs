using UnityEngine;
using System.Collections;

public class velocityTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Hiphei!");
		}
		Debug.Log (GetComponent<Rigidbody> ().velocity);
	}
}
