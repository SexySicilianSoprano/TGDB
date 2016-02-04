using UnityEngine;
using System.Collections;
using System;

public class Manager : MonoBehaviour, IManager {

    // Singleton
    public static Manager main;

    // Player variables
    public Player primaryPlayer
    {
        get 
        {
            return GetComponent<GameManager>().primaryPlayer();
        }
    }
    public Player enemyPlayer
    {
        get
        {
            return GetComponent<GameManager>().enemyPlayer();
        }
    }
    public string m_primaryPlayer;
    public string m_enemyPlayer;

    public int Money
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    // Use this for initialization
    void Start ()
    {
        Initialise();     
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    private void Initialise()
    {
        ItemDB.Initialise();
        AssignPlayerInfo();
    }

    private void AssignPlayerInfo()
    {
        m_primaryPlayer = primaryPlayer.controlledTag;
        m_enemyPlayer = enemyPlayer.controlledTag;
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

    public void AddMoney(float money)
    {
        throw new NotImplementedException();
    }

    public void AddMoneyInstant(float money)
    {
        throw new NotImplementedException();
    }

    public void RemoveMoneyInstant(float money)
    {
        throw new NotImplementedException();
    }

    public bool CostAcceptable(float cost)
    {
        throw new NotImplementedException();
    }
}
