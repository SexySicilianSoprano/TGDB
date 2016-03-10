using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour {

    StorageScript storage;
    bool showFPS;

    Toggle fps;

    // Use this for initialization
    void Awake()
    {
        storage = GameObject.Find("Storage").GetComponent<StorageScript>();
        showFPS = storage.fps;


        fps = GameObject.Find("Toggle_FPS").GetComponent<Toggle>();





        fps.isOn = showFPS;


    }



    public void ToggleFPSCounter()
    {


        storage.ToggleFPSCounter(GameObject.Find("Toggle_FPS").GetComponent<Toggle>().isOn);
    }

    public void ToggleTooltips()
    {


        storage.ToggleTooltips(GameObject.Find("Toggle_Hints").GetComponent<Toggle>().isOn);
    }

}
