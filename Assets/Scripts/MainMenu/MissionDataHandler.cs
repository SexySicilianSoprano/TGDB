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
    private Text missionName { get { return transform.Find("Missionbg").transform.Find("MissionName").GetComponent<Text>(); } }
    private Text missionNumber { get { return transform.Find("Missionbg").transform.Find("MissionTitle").GetComponent<Text>(); } }
    private Text missionInfo { get { return transform.Find("Missionbg").transform.Find("MissionInfo").transform.Find("Viewport").transform.Find("Text").GetComponent<Text>(); } }

    // Show mission details
    public void ShowMissionDetails()
    {
        missionName.text = currentMission.m_name;
        missionNumber.text = "Mission " + (currentMission.m_number + 1).ToString();
        missionInfo.text = currentMission.m_description;
    }
}
