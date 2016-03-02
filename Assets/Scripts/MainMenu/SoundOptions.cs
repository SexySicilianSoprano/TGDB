using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour {

    StorageScript storage;

    float masterVolume;
    float musicVolume;
    float effectsVolume;
    float ambientVolume;

    Slider master, music, effects, ambient;


    // Use this for initialization
    void Awake()
    {
        storage = GameObject.Find("Storage").GetComponent<StorageScript>();

        masterVolume = storage.masterVolume; 
        musicVolume = storage.musicVolume; 
        effectsVolume = storage.effectsVolume; 
        ambientVolume = storage.ambientVolume; 


        master = GameObject.Find("Master Volume").GetComponent<Slider>();
        music = GameObject.Find("Music Volume").GetComponent<Slider>();
        effects = GameObject.Find("Sound Effects Volume").GetComponent<Slider>();
        ambient = GameObject.Find("Ambient Effects Volume").GetComponent<Slider>();




        master.value = masterVolume;
        music.value = musicVolume;
        effects.value = effectsVolume;
        ambient.value = ambientVolume;


    }




    public void SetAudioSettings()
    {
        storage.SetAudioSettings(master.value, music.value, effects.value, ambient.value);
    }
}


