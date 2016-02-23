using UnityEngine;
using System.Collections;

/// <summary>
/// Show canvas. Like the name says, show the specific canvas, and if it's open already, close it.
/// </summary>

public class ShowCanvas : MonoBehaviour {

	public void ToggleCanvas() {
		//This code of line sets the rule for canvas to be active if it isn't already, and vice versa
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
