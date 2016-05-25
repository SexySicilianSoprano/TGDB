using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBarScaling : MonoBehaviour {

	private float startValue;
	private float currentValue;
	private float startHeight;
	private float startWidth;
	public GameObject cameraObj;
	private float cameraDistance;

	// Use this for initialization
	void Start () {
		cameraObj = GameObject.Find ("MyCamera");
		cameraDistance = Vector3.Distance(cameraObj.transform.position, cameraObj.GetComponent<MyCamera>().focusPoint.transform.position);
		startValue = 0.022f;
		startHeight = GetComponent<Image> ().rectTransform.sizeDelta.x;
		startWidth = GetComponent<Image> ().rectTransform.sizeDelta.y;
	}
	
	// Update is called once per frame
	void Update () {
		cameraDistance = Vector3.Distance(cameraObj.transform.position, cameraObj.GetComponent<MyCamera>().focusPoint.transform.position);

		currentValue = 1 / cameraDistance;
		GetComponent<Image> ().rectTransform.sizeDelta = new Vector2 (startWidth / startValue * currentValue, startHeight / startValue * currentValue);
	}
}
