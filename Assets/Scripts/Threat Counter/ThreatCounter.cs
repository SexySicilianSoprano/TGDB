using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// ThreatCounter
/// 
/// This component receives messages from units and buildings and calculates a threat value.
/// The threat value is then sent forward to AI and Sound Manager, which will determine how to behave according to the threat value.
/// 
/// - Karl Sartorisio
/// 
/// </summary>

public class ThreatCounter : MonoBehaviour {

    private int unitsInCombat; // Amount of units set in combat
    private int threat; // Actual value of threat

    // Timer variables
    private float timer;
    public float threatReadInterval;

	// Update is called once per frame
	void Update ()
    {
        timer = TimerAdvance(timer);
        if (timer >= threatReadInterval)
        {
            CalculateThreat();
            timer = 0;
            //Debug.Log("Threat calculated, result: " + threat);
            SendThreatMessageToAI();
        }
	}

    // Add one unit to combat
    public void SetUnitToCombat()
    {
        unitsInCombat++;
    }

    // Take one unit from combat
    public void SetUnitOutOfCombat()
    {
        unitsInCombat--;
    }

    // Advance timer
    private float TimerAdvance(float seconds)
    {
        float newTime = seconds;
        newTime += Time.deltaTime;
        return newTime;
    }

    // Calculate amount of threat
    private void CalculateThreat()
    {
        threat = unitsInCombat * 10;
    }

    // Checks if we have an active AI and sends it a message containing threat value
    private void SendThreatMessageToAI()
    {
        GameObject AICore = GameObject.Find("AICore");

        if (AICore)
        {
            AICore.GetComponent<AICore>().SendMessage("GetThreat", threat);
        }
    }

    // Checks if we have an active Sound Manager and sends it a message containing threat value
    private void SendThreatMessageToSoundSystem()
    {
        GameObject manager = GameObject.Find("Manager");

        if (manager)
        {
            if (manager.GetComponent<SoundManager>())
            {
                manager.GetComponent<SoundManager>().SendMessage("GetThreat", threat);
            }
        }
    }

}
