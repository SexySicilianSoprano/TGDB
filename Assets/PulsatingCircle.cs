using UnityEngine;
using System.Collections;

public class PulsatingCircle : MonoBehaviour {

	public Color projColorA = Color.red;
	public Color projColorB = Color.red;
	public Color lerpedColor;

	// Use this for initialization
	void Start () {
	}


	// Update is called once per frame
	void Update () {
		lerpedColor = Color.Lerp(projColorA, projColorB, Mathf.PingPong(Time.time * 0.8f, 1));
		GetComponent<Projector>().material.color = lerpedColor;
	}
}
