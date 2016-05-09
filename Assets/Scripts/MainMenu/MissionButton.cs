using UnityEngine;
using System.Collections;

public class MissionButton : MonoBehaviour {

    public int missionNumber;

    public void SetMissionDetails()
    {
        GameObject.Find("DataManager").GetComponent<DataStorage>().SetMission(missionNumber);
    }
}
