using UnityEngine;
using System.Collections;

public class NewUIManager : MonoBehaviour {    

	public GameObject[] panels;
	public GameObject helpPanel;
	private int currentPanel;

	public void Start(){
		StartCoroutine (WaitAndPutAway());
	}

	IEnumerator WaitAndPutAway(){
		yield return new WaitForSeconds(5);
		helpPanel.SetActive(false);
	}

	public void SwitchPanel(int panelIndex){
		if (panelIndex != currentPanel){
			panels[currentPanel].SetActive(false);
			panels[panelIndex].SetActive(true);
			currentPanel = panelIndex;
		}
	}
}