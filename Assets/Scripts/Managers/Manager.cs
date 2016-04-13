using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Manager : MonoBehaviour, IManager {

    // Singleton
    public static Manager main;

    // Components Manager needs to deal with
    public GameManager m_GameManager;

    // Player variables
    public Player primaryPlayer() { return GetComponent<GameManager>().primaryPlayer(); }
    public Player enemyPlayer() { return GetComponent<GameManager>().enemyPlayer(); }
    public string m_primaryPlayer;
    public string m_enemyPlayer;

    // Resource
    public float Resources
    {
        get;
        set;
    }

    // Earned experience points
    public float earnedExperience;

    // Lists of defeated and lost units and buildings
    public List<RTSEntity> defeatedBuildings = new List<RTSEntity>();
    public List<RTSEntity> defeatedUnits = new List<RTSEntity>();
    public List<RTSEntity> lostBuildings = new List<RTSEntity>();
    public List<RTSEntity> lostUnits = new List<RTSEntity>();

    void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start()
    {
        Initialise();
    }
	
	// Update is called once per frame
	void Update()
    {
        UpdateResourceText();
	}

    // Initialise data
    private void Initialise()
    {
        ItemDB.Initialise();
        AssignPlayerInfo();
    }

    // Assign player details
    private void AssignPlayerInfo()
    {

    }

    // Returns total amount of experience
    public float ReturnAccumulatedExperience()
    {
        float accExp = 0;
        accExp += earnedExperience;
        return accExp;
    }

    // Update Resource text on GUI
    private void UpdateResourceText()
    {
        Text text = GameObject.Find("UI").transform.Find("Resources").transform.Find("Text").GetComponent<Text>();
        text.text = Resources.ToString();
    }

    // Calculate accumulated experience points by adding up all defeated and lost units with respective multipliers
    public void CalculateAccumulatedExperience()
    {
        earnedExperience =
            (defeatedBuildings.Count * 100f)
            + (defeatedUnits.Count * 10f)
            - (lostBuildings.Count * 50f)
            - (lostUnits.Count * 5f);
    }

    public void BuildingAdded(Building building)
    {
        throw new NotImplementedException();
    }

    public void BuildingRemoved(Building building)
    {
        throw new NotImplementedException();
    }

    public void UnitAdded(Unit unit)
    {
        throw new NotImplementedException();
    }

    public void UnitRemoved(Unit unit)
    {
        throw new NotImplementedException();
    }

    public int GetUniqueID()
    {
        throw new NotImplementedException();
    }

    // Functions for handling in-game resources
    public void AddResource(float amount)
    {
        Resources += amount;
    }

    public void RemoveResource(float amount)
    {
        Resources -= amount;
    }

    public void AddResourceInstant(float amount)
    {
        Resources += amount;
    }

    public void RemoveResourceInstant(float amount)
    {
        Resources -= amount;
    }

    public bool CostAcceptable(float cost)
    {
        if (cost <= Resources)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
