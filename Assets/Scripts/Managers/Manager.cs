using UnityEngine;
using System.Collections;
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

    public float Resource
    {
        get;
        private set;
    }

    void Awake()
    {
        main = this;
    }

    // Use this for initialization
    void Start()
    {        
        Initialise();
        Resource = m_GameManager.missionResources;
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
        Resource += amount;
    }

    public void RemoveResource(float amount)
    {
        Resource -= amount;
    }

    public void AddResourceInstant(float amount)
    {
        Resource += amount;
    }

    public void RemoveResourceInstant(float amount)
    {
        Resource -= amount;
    }

    public bool CostAcceptable(float cost)
    {
        if (cost <= Resource)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
