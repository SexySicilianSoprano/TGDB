using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBuildingScript : MonoBehaviour {

	public GameObject[] unitBuildingList;
	public int maxQueuedUnits = 10;
	public List<GameObject> unitBuildingQueue;
	private bool isAlreadyBuilding;
	public GameObject navalYard;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (unitBuildingQueue.Count > 0){
			StartBuilding();
		}
	}

	public void BuildNewUnit(int unit){
		CheckNavalYard();
		CheckFunds();
		unitBuildingQueue.Add(unitBuildingList[unit]);
	}

	public bool CheckNavalYard(){
		navalYard = GameObject.Find("NavalYard");
		if (navalYard){
			return true;
		} else {
			return false;
		}
	}

	public void CheckFunds(){

	}

	public void StartBuilding(){
		if(isAlreadyBuilding == false){
			StartCoroutine(WaitAndBuild(2, unitBuildingList[0]));
			isAlreadyBuilding = true;
		}
	}

	IEnumerator WaitAndBuild(float seconds, GameObject unit){
		yield return new WaitForSeconds(seconds);
		Instantiate(unit, navalYard.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
		unitBuildingQueue.RemoveAt(0);
		isAlreadyBuilding = false;
	}
}
