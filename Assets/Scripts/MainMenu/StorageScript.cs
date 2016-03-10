using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class StorageScript : MonoBehaviour
{
    public int backgroundMusicPosition;
    public bool invertY = false;
    public bool fps = true;
    public bool tooltips = true;

    public float masterVolume = 100;
    public float musicVolume = 100;
    public float effectsVolume = 100;
    public float ambientVolume = 100;

    public int antiAliasing;

    Scene scene;

    float countdown = 2f;
    int nextScene;

    GameObject storedInteractables;


    void Start()
    {


        
        // print("Storage created.");
        DontDestroyOnLoad(transform.gameObject);

        scene = SceneManager.GetActiveScene();
        if (scene.name != "0.MainMenu" && scene.name != "0.LoadingScreen")
        {


           // ApplyInvert();
            ApplyFPSCounter();
        }

        nextScene = scene.buildIndex + 1;
    }

    void OnLevelWasLoaded()
    {

        countdown = 2f;
        scene = SceneManager.GetActiveScene();
        if (scene.name != "0.LoadingScreen") nextScene = scene.buildIndex + 1;
        if (scene.name == "0.MainMenu") nextScene = 2;

       /* if (scene.name != "0.MainMenu" && scene.name != "0.LoadingScreen")
        {
            ApplyInvert();
            ApplyFPSCounter();
        }*/

        if (storedInteractables != null)
        {
            GameObject.Find("BlueDimension").name = "Distracting the pursuer";
            GameObject.Find("SpawnPoint").name = "Distracting the pursuer";
            GameObject.Find("CheckPoint").name = "Distracting the pursuer";
            GameObject.Find("Interactables").name = "OldInteractables";


            

            Destroy(GameObject.Find("OldInteractables"));

            storedInteractables.SetActive(true);



            storedInteractables.transform.parent = null;

            storedInteractables.name = "Interactables";


            storedInteractables.transform.Find("BlueDimension").gameObject.SetActive(true);

            

            GameObject.Find("Player").transform.position = GameObject.Find("SpawnPoint").transform.position;
            GameObject.Find("Player").transform.rotation = GameObject.Find("SpawnPoint").transform.rotation;

        }

    }

    void Update()
    {
        if (scene.name == "0.LoadingScreen")
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0)
            {
                SceneManager.LoadScene(nextScene);
            }
        }

       /* if (scene.name != "0.MainMenu" && scene.name != "0.LoadingScreen")
        { 
            if (Input.GetKeyDown(KeyCode.F6)) SceneManager.LoadScene("0.LoadingScreen");
            if (Input.GetKeyDown(KeyCode.F3)) GameObject.Find("KillBox").GetComponent<Spawner>().NewDeath();
        }*/
    }


/*    public void ToggleInvert(bool status)
    {
        invertY = status;

        if (scene.name != "0.MainMenu" && scene.name != "0.LoadingScreen") ApplyInvert();
    }
    */

  /*  public void ApplyInvert()
    {
        if (invertY) GameObject.Find("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().mouseLook.invertY = true;

        else GameObject.Find("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().mouseLook.invertY = false;
    }*/

    public void ToggleFPSCounter(bool status)
    {
        fps = status;

        if (scene.name != "0.MainMenu" && scene.name != "0.LoadingScreen") ApplyFPSCounter();
    }

    public void ToggleTooltips(bool status)
    {
        tooltips = status;

        if (scene.name != "0.MainMenu" && scene.name != "0.LoadingScreen") ApplyTooltips();
    }

    public void ApplyTooltips()
    {
        if (tooltips) GameObject.Find("Tooltips").GetComponent<Text>().enabled = true;

        else GameObject.Find("Tooltips").GetComponent<Text>().enabled = false;
    }


    public void ApplyFPSCounter()
    {
        if (fps) GameObject.Find("FPSText").GetComponent<Text>().enabled = true;

        else GameObject.Find("FPSText").GetComponent<Text>().enabled = false;
    }

    public void SetAudioSettings(float masterV, float musicV, float effectsV, float ambientV)
    {
        masterVolume = masterV;
        musicVolume = musicV;
        effectsVolume = effectsV;
        ambientVolume = ambientV;
    }

    public void ApplyAudioSettings()
    {

    }

    public void SetStoredInteractables (GameObject go)
    {
        storedInteractables = go;
    }

}
