using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlOptions : MonoBehaviour
{

    StorageScript storage;
    bool invertY;

    Toggle invert;

    // Use this for initialization
    void Awake()
    {
        storage = GameObject.Find("Storage").GetComponent<StorageScript>();
        invertY = storage.invertY;


        //invert = GameObject.Find("Toggle_Invert").GetComponent<Toggle>();


       


        invert.isOn = invertY;


    }



   /* public void ToggleInvert()
    {


        storage.ToggleInvert(GameObject.Find("Toggle_Invert").GetComponent<Toggle>().isOn);
    }*/


}
