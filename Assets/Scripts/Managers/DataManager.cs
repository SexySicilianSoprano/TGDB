using UnityEngine;
using System.Collections;

/// <summary>
///  This is the <DataManager>. Its purpose is to handle PlayerPrefs and
///  all saved game data we want to manage. It will be started on start-up and closed
///  only when the game is turned off.
/// 
///  TODO: Switch the names of this and DataStorage, because otherwise it's pretty retarded
///  
/// - Karl Sartorisio
/// The Great Deep Blue
/// </summary>

public class DataManager : MonoBehaviour {

    // Available missions
    public int gearsMissions;
    public int scalesMissions;
    public int mechanusMissions;

    // Maximum missions
    public int gearsMax = 10;
    public int scalesMax = 10;
    public int mechanusMax = 10;

    // Player Data
    public int p_Rank;
    public float p_Exp;
    public string p_Name;
    public float p_Score;

    // House Data
    public float gearsExp;
    public float scalesExp;
    public float mechanusExp;
    public int gearsRank;
    public int scalesRank;
    public int mechanusRank;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
        // Fetch data from storage
        LoadData();
        MissionDB.Initialise();
    }

    // Loads data from PlayerPrefs, if they're not set, set them.
    void LoadData()
    {
        gearsMissions = PlayerPrefs.GetInt("gearsMissions");
        scalesMissions = PlayerPrefs.GetInt("scalesMissions");
        mechanusMissions = PlayerPrefs.GetInt("mechanusMissions");

        // If we start the game the first time, initialize game data
        if (gearsMissions == 0)
        {
            PlayerPrefs.SetInt("gearsMissions", 1);
        }

        if (scalesMissions == 0)
        {
            PlayerPrefs.SetInt("scalesMissions", 1);
        }

        if (mechanusMissions == 0)
        {
            PlayerPrefs.SetInt("mechanusMissions", 1);
        }

        // Loading player overall data
        p_Name = PlayerPrefs.GetString("playerName");
        p_Exp = PlayerPrefs.GetFloat("playerExp");
        p_Rank = PlayerPrefs.GetInt("playerRank");
        p_Score = PlayerPrefs.GetFloat("playerScore");

        // Load House-specific data
        gearsRank = PlayerPrefs.GetInt("gearsRank");
        scalesRank = PlayerPrefs.GetInt("scalesRank");
        mechanusRank = PlayerPrefs.GetInt("mechanusRank");
        gearsExp = PlayerPrefs.GetFloat("gearsExp");
        scalesExp = PlayerPrefs.GetFloat("scalesExp");
        mechanusExp = PlayerPrefs.GetFloat("mechanusExp");
    }

    public void SaveAll()
    {
        // Save mission data
        PlayerPrefs.SetInt("gearsMissions", gearsMissions);
        PlayerPrefs.SetInt("scalesMissions", scalesMissions);
        PlayerPrefs.SetInt("mechanusMissions", mechanusMissions);

        // Save overall data
        SaveExp(p_Exp);
        SaveName(p_Name);
        SaveRank(p_Rank);
        SaveScore(p_Score);

        // Save House data
        SaveHouseExp("gearsExp", gearsExp);
        SaveHouseExp("scalesExp", scalesExp);
        SaveHouseExp("mechanusExp", mechanusExp);
        SaveHouseRank("gearsRank", gearsRank);
        SaveHouseRank("scalesRank", scalesRank);
        SaveHouseRank("mechanusRank", mechanusRank);
    }

    // Save data on specific house and mission number.
    public void SaveProgress(string house, int mission)
    {
        PlayerPrefs.SetInt(house, mission);
    }

    // Save player name
    public void SaveName(string player)
    {
        PlayerPrefs.SetString("playerName", player);
    }

    // Save overall exp
    public void SaveExp(float exp)
    {
        PlayerPrefs.SetFloat("playerExp", exp);
    }
    
    // Save overall rank
    public void SaveRank(int rank)
    {
        PlayerPrefs.SetInt("playerRank", rank);
    }

    public void SaveScore(float score)
    {
        PlayerPrefs.SetFloat("playerScore", score);
    }

    // Save House rank
    public void SaveHouseRank(string house, int rank)
    {
        PlayerPrefs.SetInt(house, rank);
    }

    // Save House experience
    public void SaveHouseExp(string house, float exp)
    {
        PlayerPrefs.SetFloat(house, exp);
    }

    // Get a specific int
    public void LoadInt(string key)
    {
        PlayerPrefs.GetInt(key);
    }

    // Get a specific float
    public void LoadFloat(string key)
    {
        PlayerPrefs.GetFloat(key);
    }

    // Get a specific string
    public void LoadString(string key)
    {
        PlayerPrefs.GetString(key);
    }

    // Set a specific int
    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    // Set a specific float
    public void SaveFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    // Set a specific string
    public void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public void SetExperience(string house, float exp)
    {
        switch (house)
        {
            case "gears":
                gearsExp += exp;
                break;
            case "scales":
                scalesExp += exp;
                break;
            case "mechanus":
                mechanusExp += exp;
                break;
        }

        // TODO: check for rank thresholds and apply if necessary
        // TODO: an actual rank threshold
        // TODO: an actual rank system
    }

    void OnApplicationQuit()
    {
        Debug.Log("Save all data, y'all");
        SaveAll(); 
    }

}
