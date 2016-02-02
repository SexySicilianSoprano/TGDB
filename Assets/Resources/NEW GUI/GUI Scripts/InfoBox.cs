using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour {

	public GameObject myInfo; //This is the Info Box

	void OnMouseEnter () {
		Debug.Log("Entered areaaa! :D");
		myInfo.SetActive(true);
	}

	//Now to make it appear while Mouse is hovering over button
	void OnMouseOver() {
		Debug.Log("Hoverrrrr! :D");

		myInfo.transform.position = Input.mousePosition;
	}

	void OnMouseExit() {
		Debug.Log("Not hovered! :<");
		myInfo.SetActive(false);
	}
}
