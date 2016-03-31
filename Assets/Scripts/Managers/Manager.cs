using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        private set;
    }

    // Earned experience points
    public float earnedExperience;

    // Lists of defeated and lost units and buildings
    public List<RTSEntity> DefeatedBuildings = new List<RTSEntity>();
    public List<RTSEntity> DefeatedUnits = new List<RTSEntity>();
    public List<RTSEntity> LostBuildings = new List<RTSEntity>();
    public List<RTSEntity> LostUnits = new List<RTSEntity>();

    void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start()
    {
        Initialise();
        Resources = m_GameManager.missionResources;
    }
	
	// Update is called once per frame
	void Update()
    {

	}

    private void Initialise()
    {
        ItemDB.Initialise();
        AssignPlayerInfo();
    }

    private void AssignPlayerInfo()
    {
        m_primaryPlayer = primaryPlayer().controlledTag;
        m_enemyPlayer = enemyPlayer().controlledTag;
    }

    public float ReturnAccumulatedExperience()
    {
        float accExp = 0;
        accExp += earnedExperience;
        return accExp;
    }

    // Calculate accumulated experience points by adding up all defeated and lost units with respective multipliers
    public void CalculateAccumulatedExperience()
    {
        earnedExperience =
            (DefeatedBuildings.Count * 100f)
            + (DefeatedUnits.Count * 10f)
            - (LostBuildings.Count * 50f)
            - (LostUnits.Count * 5f);
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
