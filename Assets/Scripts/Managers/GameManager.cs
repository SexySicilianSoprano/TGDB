using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IGameManager {

    public static GameManager main;

    private bool m_GameSet = false;
    private Player m_Player1 = new Player();
    private Player m_Player2 = new Player();
    private GameObject m_FloatingFortress1;
    private GameObject m_FloatingFortress2;

    public Player primaryPlayer()
    {
        return m_Player1;
    }

    public Player enemyPlayer()
    {
        return m_Player2;
    }

    public GameObject victoryPanel;
    public GameObject defeatPanel;

    void Awake()
    {
        main = this;
    }

    // Use this for initialization    
    void Start()
    {
        m_Player1.AssignDetails(SetPlayer.Player1);
        m_Player2.AssignDetails(SetPlayer.Player2);

        m_FloatingFortress1 = GameObject.Find("Player1");
        m_FloatingFortress2 = GameObject.Find("Player2");
    }

    // Update is called once per frame
    void Update()
    {
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
}
