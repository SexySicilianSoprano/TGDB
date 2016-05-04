using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// This static class stores mission data that will be loaded in the mission selection menu
/// and transferred to the scene itself, so that we can handle things like score, experience and
/// unlocking new missions
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// 
/// </summary>

public static class MissionDB {

    public static List<Mission> gearsMissions = new List<Mission>();
    public static List<Mission> scalesMissions = new List<Mission>();
    public static List<Mission> mechanusMissions = new List<Mission>();

    /* #### GEARS MISSIONS #### */

    public static Mission gearsMission1 = new Mission()
    {
        m_name = "Into the Yonder",
        m_number = 1,
        m_exp = 500,
        m_startingResources = 4000f,
        m_description = "The first mission, good luck!",
        m_Scene = "TestLevel_1"
    };

    public static Mission gearsMission2 = new Mission()
    {
        m_name = "Into the Yonder... again!",
        m_number = 2,
        m_exp = 800,
        m_startingResources = 4000f,
        m_description = "The second mission, food luck!",
        m_Scene = "TestLevel_1"
    };

    public static Mission gearsMission3 = new Mission()
    {
        m_name = "Into the Yonder... Once More!",
        m_number = 3,
        m_exp = 1000,
        m_startingResources = 4000f,
        m_description = "The third mission, good fuck!",
        m_Scene = "TestLevel_1"
    };

    // #### Initialisation ####

    // Initialise specific House missions
    private static void InitialiseGearsMission(Mission mission)
    {
        gearsMissions.Add(mission);
    }

    private static void InitialiseScalesMission(Mission mission)
    {
        scalesMissions.Add(mission);
    }

    private static void InitialiseMechanusMission(Mission mission)
    {
        mechanusMissions.Add(mission);
    }

    // Initialise all missions
    public static void Initialise()
    {
        InitialiseGearsMission(gearsMission1);
        InitialiseGearsMission(gearsMission2);
        InitialiseGearsMission(gearsMission3);
    }
}
