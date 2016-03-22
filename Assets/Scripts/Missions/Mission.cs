using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// This is class to give each individual mission its data like name,
/// mission number, base exp, description and such. The missions are
/// instatiated in static class MissionDB
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// 
/// </summary>

public class Mission : MonoBehaviour {

    // Mission variables
    public string m_name;
    public int m_number;
    public float m_exp;
    public string m_description;
    public string m_Scene;

    // Assign details
    public void AssignDetails(Mission mission)
    {
        m_name = mission.m_name;
        m_number = mission.m_number;
        m_exp = mission.m_exp;
        m_description = mission.m_description;
        m_Scene = mission.m_Scene;
    }
}
