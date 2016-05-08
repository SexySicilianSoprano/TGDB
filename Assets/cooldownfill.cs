using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

/// <summary>
/// 
/// Cooldownfill. This code is activating the cooldown panel, and determines the filling speed. 
/// It's being called from Unit Building Script and Building Script
/// 
/// </summary>

public class cooldownfill : MonoBehaviour {

	public Image cooldown;

    public void SetCoolDownFill(float max, float current)
    {
        cooldown.fillAmount = current / max;
    }

    public void ClearFill()
    {
        cooldown.fillAmount = 0;
    }
}