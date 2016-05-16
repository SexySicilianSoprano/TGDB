using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataStorage : MonoBehaviour {

    // Current Mission Data
    private Mission m_Mission;

    // Current House Data, should only store either "gears", "scales" or "mechanus"
    private string currentHouse = "gears";

    // Multipliers
    private float expMultiplier = 1;

    // Temporary player data
    private float experience;
    private float scorepoints;

    // DataManager accessor
    private DataManager m_DataManager { get { return GetComponent<DataManager>(); } }

    // Arsenal List
    public List<GameObject> UnitList = new List<GameObject>();
    public List<GameObject> BuildingList = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Handling received data and sending them towards DataManager for saving
    public void SaveDataRequest(float score)
    {
        float exp = CalculateExp(score);

        switch (currentHouse)
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

        m_DataManager.SetExperience(currentHouse, exp);
        m_DataManager.p_Exp += exp;
        m_DataManager.p_Score += score;
    }

    // Calculates experience from actual score
    private float CalculateExp(float score)
    {
        return (score / 10) * expMultiplier;
    }

    // Set current house
    public void SetHouse(string house)
    {
        currentHouse = house;
    }

    // Fetch current house
    public string GetHouse()
    {
        return currentHouse;
    }

    // Set mission data
    public void SetMission(int mission)
    {
        switch (currentHouse)
        {
            case "gears":
                m_Mission = MissionDB.gearsMissions.Find(x => x.m_number == mission);
                break;
            case "scales":
                m_Mission = MissionDB.scalesMissions.Find(x => x.m_number == mission);
                break;
            case "mechanus":
                m_Mission = MissionDB.mechanusMissions.Find(x => x.m_number == mission);
                break;
        }
    }

    // Fetch current mission data
    public Mission GetMission()
    {
        return m_Mission;
    }

    // Add to expMultiplier
    public void AddExpMultiplier(float value)
    {
        expMultiplier += value;
    }

    // Set expMultiplier back to standard 100%
    public void ClearMultiplier()
    {
        expMultiplier = 1;
    }

    // Save and alter data after a victory
    public void SaveVictoryData(int mission)
    {
        int available = 0;
        int max = 0;

        // Get available and total number of missions from respective house
        switch (currentHouse)
        {
            case "gears":
                available = m_DataManager.gearsMissions;
                max = m_DataManager.gearsMax;
                break;
            case "scales":
                available = m_DataManager.scalesMissions;
                max = m_DataManager.scalesMax;
                break;
            case "mechanus":
                available = m_DataManager.mechanusMissions;
                max = m_DataManager.mechanusMax;
                break;
        }

        // Advance available mission data if mission was the latest one and not the final one
        if (mission == available && mission < max)
        {
            switch (currentHouse)
            {
                case "gears":
                    m_DataManager.gearsMissions++;
                    break;
                case "scales":
                    m_DataManager.scalesMissions++;
                    break;
                case "mechanus":
                    m_DataManager.mechanusMissions++;
                    break;
            }
        }
    }

    // Get unit and building prefabs and add them to lists, so that they may be added to player's arsenal
    public void SetHouseArsenal()
    {
        // Get unit prefab paths
        GameObject destroyer = Resources.Load("Models/Units/" + currentHouse + "House/Destroyer/Destroyer", typeof(GameObject)) as GameObject;
        GameObject scout = Resources.Load("Models/Units/" + currentHouse + "House/Scout/Scout", typeof(GameObject)) as GameObject; ;
        GameObject fishingboat = Resources.Load("Models/Units/" + currentHouse + "House/FishingBoat/FishingBoat", typeof(GameObject)) as GameObject; ;
        GameObject dreadnought = Resources.Load("Models/Units/" + currentHouse + "House/Destroyer/Destroyer", typeof(GameObject)) as GameObject;

        // Get building prefab paths
        GameObject floatingfortress = Resources.Load("Models/Buildings/" + currentHouse + "House/FloatingFortress/Player1", typeof(GameObject)) as GameObject;
        GameObject navalyard = Resources.Load("Models/Buildings/" + currentHouse + "House/NavalYard/NavalYard", typeof(GameObject)) as GameObject;
        GameObject refinery = Resources.Load("Models/Buildings/" + currentHouse + "House/Refinery/Refinery", typeof(GameObject)) as GameObject;
        GameObject laboratory = Resources.Load("Models/Buildings/" + currentHouse + "House/Laboratory/Laboratory", typeof(GameObject)) as GameObject;

        // Add units to arsenal list
        UnitList.Add(destroyer);
        UnitList.Add(fishingboat);
        UnitList.Add(scout);
        UnitList.Add(dreadnought);

        // Add buildings to arsenal list
        BuildingList.Add(floatingfortress);
        BuildingList.Add(navalyard);
        BuildingList.Add(refinery);
        BuildingList.Add(laboratory);
    }

}
