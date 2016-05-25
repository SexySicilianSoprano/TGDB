using UnityEngine;
using System.Collections;

public class ManagerListener : MonoBehaviour {

    public DataManager dataManager;

	// Use this for initialization
	void Start ()
    {
        if (!GameObject.Find("DataManager"))
        {
            GameObject dManager = Instantiate(dataManager, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            dManager.name = "DataManager";
        }
	}
}
