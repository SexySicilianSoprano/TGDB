using UnityEngine;
using System.Collections;

public class BuildingOnDestroy : MonoBehaviour {
    
    void OnDestroy()
    {
        if (GetComponent<NavalYard>())
        {
            GameObject.Find("Player1").transform.Find("BuildingSpot").gameObject.SetActive(true);
        }
    }
}
