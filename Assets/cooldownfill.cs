using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class cooldownfill : MonoBehaviour {

	public Image cooldown;
	public bool coolingDown;
	public float waitTime = 5.0f;

	// Update is called once per frame
	void Update () 
	{
		if (coolingDown == true)
		{
			//Reduce fill amount over 5 seconds
			cooldown.fillAmount += 1.0f/waitTime * Time.deltaTime;
		}
	}
}