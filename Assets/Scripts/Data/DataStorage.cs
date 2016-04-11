using UnityEngine;
using System.Collections;

public class DataStorage : MonoBehaviour {

    // Current Mission Data
    public Mission m_Mission;

    // Multipliers
    public float expMultiplier = 1;

    // Temporary player data
    private float experience;
    private float scorepoints;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
	}

    // Handling received data and sending them towards DataManager for saving
    public void SaveDataRequest(float score, string house)
    {
        DataManager dMana = GetComponent<DataManager>();
        float exp = CalculateExp(score);

        switch (house)
        {
            case "gears":
                dMana.gearsExp += exp;
                break;
            case "scales":
                dMana.scalesExp += exp;
                break;
            case "mechanus":
                dMana.mechanusExp += exp;
                break;
        }

        dMana.p_Exp += exp;
        dMana.p_Score += score;
    }

    // Calculates experience from actual score
    private float CalculateExp(float score)
    {
        return (score / 10) * expMultiplier;
    }

}
