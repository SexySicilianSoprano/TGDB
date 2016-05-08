using UnityEngine;
using System.Collections;

public class DataStorage : MonoBehaviour {

    // Current Mission Data
    public Mission m_Mission;

    // Current House Data, should only store either "gears", "scales" or "mechanus"
    public string currentHouse;

    // Multipliers
    public float expMultiplier = 1;

    // Temporary player data
    private float experience;
    private float scorepoints;

    // DataManager accessor
    private DataManager m_DataManager { get { return GetComponent<DataManager>(); } }

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
	}

    // Handling received data and sending them towards DataManager for saving
    public void SaveDataRequest(float score, string house)
    {
        float exp = CalculateExp(score);

        switch (house)
        {
            case "gears":
                m_DataManager.gearsExp += exp;
                break;
            case "scales":
                m_DataManager.scalesExp += exp;
                break;
            case "mechanus":
                m_DataManager.mechanusExp += exp;
                break;
        }

        m_DataManager.p_Exp += exp;
        m_DataManager.p_Score += score;
    }

    // Calculates experience from actual score
    private float CalculateExp(float score)
    {
        return (score / 10) * expMultiplier;
    }
}
