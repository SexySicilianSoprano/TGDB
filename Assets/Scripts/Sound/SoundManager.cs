using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    private FMOD.Studio.EventInstance background;
    private List<RTSEntity> l_UnitsInCombat = new List<RTSEntity>();
    private float battle = 0;
    private Vector3 mainCameraPos { get { return GameObject.Find("MyCamera").transform.position; } }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckForCombat();
    }

    public void PlayMissionMusic(Mission mission)
    {
        string path = "event:/Background Music";
        background = FMODUnity.RuntimeManager.CreateInstance(path);
        // background.start();
    }

    public void PauseMusic()
    {
        background.setPaused(true);
    }

    public void ContinueMusic()
    {
        background.setPaused(false);
    }

    public void StopMusic()
    {
        background.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        background.release();
    }

    public void PlaySpawnSound(string name, Vector3 position)
    {
        if (name != "Fishing Boat")
        {
            try
            {
                Vector3 newPos = new Vector3(position.x, mainCameraPos.y, position.z);
                string path = "event:/" + name + "_ready";
                FMODUnity.RuntimeManager.PlayOneShot(path);
            }
            catch
            {

            }
        }
        
    }

    public void PlayAttackSound(RTSEntity unit, Vector3 position)
    {
        if (unit.Name != "Fishing Boat")
        {
            Vector3 newPos = new Vector3(position.x, mainCameraPos.y, position.z);
            string path = "event:/" + unit.Name.Replace(" ", "") + "_attack";
            FMODUnity.RuntimeManager.PlayOneShot(path);
        }
    }

    public void PlaySelectSound(RTSEntity unit, Vector3 position)
    {
        if (unit.Name != "Fishing Boat")
        {
            string path = "event:/" + unit.Name.Replace(" ", "") + "_aknowledge";
            FMODUnity.RuntimeManager.PlayOneShot(path);
        }
    }

    public void PlayCommandSound(RTSEntity unit, Vector3 position)
    {
        if (unit.Name != "Fishing Boat")
        {
            Vector3 newPos = new Vector3(position.x, mainCameraPos.y, position.z);
            string path = "event:/" + unit.Name.Replace(" ", "") + "_confirm";
            FMODUnity.RuntimeManager.PlayOneShot(path);
        }
    }

    public void PlayFiringSound(RTSEntity unit, Vector3 position)
    {
        if (unit.Name != "Fishing Boat")
        {
            string path = "event:/" + unit.Name.Replace(" ", "") + "_shot";
            FMODUnity.RuntimeManager.PlayOneShot(path);
        }
    }

    public void PlayFiringSound(string name, Vector3 position)
    {
        string path = "event:/" + name + "_shot";
        FMODUnity.RuntimeManager.PlayOneShot(path);
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
