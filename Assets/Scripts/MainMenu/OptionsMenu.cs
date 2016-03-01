using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    GameObject soundOptions, graphicsOptions, controlOptions, gameOptions;



    public bool subMenuClosed = false;



    void Start()
    {




        soundOptions = GameObject.Find("SoundOptions");
        graphicsOptions = GameObject.Find("GraphicOptions");
        controlOptions = GameObject.Find("ControlOptions");
        gameOptions = GameObject.Find("GameOptions");

    }



    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {     
            Close();
        }
        
    }

    public void OpenSoundOptions()
    {

        soundOptions.SetActive(true);
        graphicsOptions.SetActive(false);
        controlOptions.SetActive(false);
        gameOptions.SetActive(false);


    }

    public void OpenGraphicsOptions()
    {
        soundOptions.SetActive(false);
        graphicsOptions.SetActive(true);
        controlOptions.SetActive(false);
        gameOptions.SetActive(false);

    }

    public void OpenControlOptions()
    {
        soundOptions.SetActive(false);
        graphicsOptions.SetActive(false);
        controlOptions.SetActive(true);
        gameOptions.SetActive(false);

    }

    public void OpenGameOptions()
    {
        soundOptions.SetActive(false);
        graphicsOptions.SetActive(false);
        controlOptions.SetActive(false);
        gameOptions.SetActive(true);

    }

    public void Close()
    {
        gameObject.GetComponent<Canvas>().enabled = false;

    }

    public void DisableSubOptions()
    {


        soundOptions.SetActive(false);
        graphicsOptions.SetActive(false);
        controlOptions.SetActive(false);
        gameOptions.SetActive(false);
    }


}
