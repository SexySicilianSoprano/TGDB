using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Info box. This code implies when Infobox pops up, and when not. 
/// </summary>

public class InfoBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject myInfo; //This is the Info Box

	void Awake (){
		//Hide the Infobox at the start of the game. It's hidden from scene by default but this helps confirm it.
		myInfo.SetActive(false);	
	}
		
	void Update() {
		//Update the location of the infobox to be close to the cursor all the time. 
		//TO THINK ABOUT: if this should be in fixed position instead, since it can get annoying this way.
		myInfo.transform.position = Input.mousePosition;
	}

	//When cursor enters the object, show infobox
	public void OnPointerEnter (PointerEventData eventData) {
		Debug.Log("Entered areaaa! :D");
		//myInfo.SetActive(true);	
	}

	//when cursor leaves the object, hide infobox
	public void OnPointerExit(PointerEventData eventData) {
		Debug.Log("Not hovered! :<");
		myInfo.SetActive(false);

	}
}
