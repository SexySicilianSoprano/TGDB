using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IGameManager {

    // Singleton
    public static GameManager main;

    // Game Set Bool
    private bool m_GameSet = false;
    private bool m_Victory = false;

    // Map Data
    public string missionName;
    public int missionNumber;
    public string missionPlayerHouse;
    public float missionExp;
    public float missionResources;

    // Player-related variables
    private Player m_Player1 = new Player();
    private Player m_Player2 = new Player();
    private GameObject m_FloatingFortress1;
    private GameObject m_FloatingFortress2;
    public Player primaryPlayer() { return m_Player1; }
    public Player enemyPlayer() { return m_Player2; }

    // Victory & Defeat panels
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    // Manager accessors
    public DataManager m_DataManager;
    public DataStorage m_DataStorage;
    public Manager m_Manager;
    public SoundManager m_SoundManager;

    // Refer singleton
    void Awake()
    {
        main = this;
    }

    // Initialize details   
    void Start()
    {
        // Assign datahandlers
        m_DataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        m_DataStorage = GameObject.Find("DataManager").GetComponent<DataStorage>();
        m_SoundManager = GetComponent<SoundManager>();
        m_Manager = GetComponent<Manager>();

        // Assign mission data
        AssignMissionData(m_DataStorage.GetMission());
        //AssignMissionData(MissionDB.gearsMission1);
        m_Manager.AddResourceInstant(missionResources);
        m_SoundManager.PlayMissionMusic(m_DataStorage.GetMission());

        // Assign player data
        m_Player1.AssignDetails(SetPlayer.Player1);
        m_Player2.AssignDetails(SetPlayer.Player2);
        m_FloatingFortress1 = GameObject.Find("Player1");
        m_FloatingFortress2 = GameObject.Find("Player2");
        m_Manager.m_primaryPlayer = m_Player1.controlledTag;
        m_Manager.m_enemyPlayer = m_Player2.controlledTag;
    }

    // Update is called once per frame
    void Update()
    {
        // When game hasn't been set, keep checking for victory conditions
        if (!m_GameSet)
        {
            // Win condition
            if (!m_FloatingFortress1 && primaryPlayer() == m_Player2 || !m_FloatingFortress2 && primaryPlayer() == m_Player1)
            {
                victoryPanel.SetActive(true);
                m_GameSet = true;
                m_Victory = true;
                SaveProgress();
            }
            // Lose condition
            else if (!m_FloatingFortress1 && primaryPlayer() == m_Player1 || !m_FloatingFortress2 && primaryPlayer() == m_Player2)
            {
                defeatPanel.SetActive(true);
                m_GameSet = true;
                m_Victory = false;
                SaveProgress();
            }

        }
    }

    // Saves progress
    private void SaveProgress()
    {
        // If we win
            // If mission number is the same as available missions + less than total number of missions, open new mission
            // Save accumulated experience points
        if (m_Victory)
        {
            float exp = missionExp + m_Manager.ReturnAccumulatedExperience();
            m_DataStorage.SaveDataRequest(exp);
            m_DataStorage.SaveVictoryData(missionNumber);
        }

        // else
            // Save accumulated experience points with a cut because defeat

    }

    // Get mission data
    private void AssignMissionData(Mission mission)
    {
        // Assign mission data
        missionName = mission.m_name;
        missionNumber = mission.m_number;
        missionExp = mission.m_exp;
        missionResources = mission.m_startingResources;
        //smissionPlayerHouse = mission.m_PlayerHouse;
    }

    // Get the assigned house's arsenal
    private void LoadHouseArsenal(string house)
    {

    }
}
