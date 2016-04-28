using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

/// <summary>
/// Cooldownfill. This code is activating the cooldown panel, and determines the speed. 
/// TODO: Needs to be edited to fit different times.
/// </summary>

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
			//cooldown.fillAmount += 1.0f/waitTime * Time.deltaTime;
		}
	}

    public void SetCoolDownFill(float max, float current)
    {
        cooldown.fillAmount += current / max * Time.deltaTime;
    }

    public void ClearFill()
    {
        cooldown.fillAmount = 0;
    }
}