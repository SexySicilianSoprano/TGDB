using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MissionDataHandler : MonoBehaviour {

    // Fetch the current selected mission from data handler
    public Mission currentMission
    {
        get
        {
            return GameObject.Find("DataManager").GetComponent<DataStorage>().GetMission();
        }
    }

    // Text boxes for mission details
    private Text missionName { get { return transform.Find("MissionName").GetComponent<Text>(); } }
    private Text missionInfo { get { return transform.Find("MissionInfo").transform.Find("Text").GetComponent<Text>(); } }

    // Show mission details
    public void ShowMissionDetails()
    {
        missionName.text = (currentMission.m_number+1).ToString() + ": " + currentMission.m_name;
        missionInfo.text = currentMission.m_description;
    }
}
