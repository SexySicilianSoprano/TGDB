using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IGameManager {

    // Singleton
    public static GameManager main;

    // Game Set Bool
    private bool m_GameSet = false;

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
    public Manager m_Manager;

    // Refer singleton
    void Awake()
    {
        main = this;
    }

    // Initialize details   
    void Start()
    {
        m_DataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        m_Player1.AssignDetails(SetPlayer.Player1);
        m_Player2.AssignDetails(SetPlayer.Player2);

        m_FloatingFortress1 = GameObject.Find("Player1");
        m_FloatingFortress2 = GameObject.Find("Player2");
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

            }
            // Lose condition
            else if (!m_FloatingFortress1 && primaryPlayer() == m_Player1 || !m_FloatingFortress2 && primaryPlayer() == m_Player2)
            {
                defeatPanel.SetActive(true);
                m_GameSet = true;
            }
        }
    }

    private void SaveProgress()
    {
        // If we win
            // If mission number is the same as available missions + less than total number of missions, open new mission
            // Save accumulated experience points

        // else
            // Save accumulated experience points with a cut because defeat

    }

    private void AssignMissionData(Mission mission)
    {
        // Assign mission data
    }
}
