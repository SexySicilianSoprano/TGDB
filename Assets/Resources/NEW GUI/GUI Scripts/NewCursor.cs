using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewCursor : MonoBehaviour {

	public RawImage cursor;
	public GameObject cursor3d;
	
	// Update is called once per frame
	void Update () {
		transform.position = Input.mousePosition;
	}
}
