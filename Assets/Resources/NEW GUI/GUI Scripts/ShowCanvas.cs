using UnityEngine;
using System.Collections;

public class ShowCanvas : MonoBehaviour {

	public void ToggleCanvas() {
		//This code of line sets the rule for canvas to be active if it isn't already, and vice versa
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
