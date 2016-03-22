﻿using UnityEngine;
using System.Collections;
using System;

public class Manager : MonoBehaviour, IManager {

    // Singleton
    public static Manager main;

    // Components Manager needs to deal with
    public IGameManager m_GameManager;

    // Player variables
    public Player primaryPlayer() { return GetComponent<GameManager>().primaryPlayer(); }
    public Player enemyPlayer() { return GetComponent<GameManager>().enemyPlayer(); }
    public string m_primaryPlayer;
    public string m_enemyPlayer;

    public int Resource
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

    public void AddResource(float amount)
    {
        throw new NotImplementedException();
    }

    public void AddResourceInstant(float amount)
    {
        throw new NotImplementedException();
    }

    public void RemoveResourceInstant(float amount)
    {
        throw new NotImplementedException();
    }

    public bool CostAcceptable(float cost)
    {
        throw new NotImplementedException();
    }
}
