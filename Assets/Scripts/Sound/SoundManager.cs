using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    
    private List<RTSEntity> l_UnitsInCombat = new List<RTSEntity>();
    private float battle = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckForCombat();
    }

    public void PlayMissionMusic(Mission mission) { }

    public void PlayAttackSound(RTSEntity unit) { }

    public void PlaySelectSound(RTSEntity unit) { }

    public void PlayCommandSound(RTSEntity unit) { }

    public void PlayFiringSound(RTSEntity unit) { }
    
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
