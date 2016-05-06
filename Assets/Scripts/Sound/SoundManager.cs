using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    
    private List<RTSEntity> l_UnitsInCombat = new List<RTSEntity>();
    private float battle = 0;
    private Vector3 mainCameraPos { get { return GameObject.Find("MyCamera").transform.position; } }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCombat();
    }

    public void PlayMissionMusic(Mission mission) { }

    public void PlayAttackSound(RTSEntity unit, Vector3 position)
    {
        Vector3 newPos = new Vector3(position.x, mainCameraPos.y, position.z);
        string path = "event:/" + unit.Name.Replace(" ", "") + "_attack";
        FMODUnity.RuntimeManager.PlayOneShot(path, newPos);
    }

    public void PlaySelectSound(RTSEntity unit, Vector3 position)
    {
        string path = "event:/" + unit.Name.Replace(" ", "") + "_aknowledge";
        FMODUnity.RuntimeManager.PlayOneShot(path, position);
    }

    public void PlayCommandSound(RTSEntity unit, Vector3 position)
    {
        Vector3 newPos = new Vector3(position.x, mainCameraPos.y, position.z);
        string path = "event:/" + unit.Name.Replace(" ", "") + "_confirm";
        FMODUnity.RuntimeManager.PlayOneShot(path, newPos);
    }

    public void PlayFiringSound(RTSEntity unit, Vector3 position)
    {
        string path = "event:/" + unit.Name.Replace(" ", "") + "_shot";
        FMODUnity.RuntimeManager.PlayOneShot(path, position);
    }
    
    public void CallForBattleTrue(RTSEntity unit)
    {
        l_UnitsInCombat.Add(unit);
    }

    public void CallForBattleFalse(RTSEntity unit)
    {
        l_UnitsInCombat.Remove(unit);
    }

    private void CheckForCombat()
    {
        if (l_UnitsInCombat.Count <= 0)
        {
            battle = 0;
        }
        else
        {
            battle = 1;
        }
    }
}
