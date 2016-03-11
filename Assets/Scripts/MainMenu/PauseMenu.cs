using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	
	Canvas pauseMenu, optionsMenu, quitMenu;
    //public Button startText;
    //public Button exitText;

    public bool paused = false;

   // StorageScript storage;
   // bool invertY;


	// Use this for initialization
	void Start () 
	{
		optionsMenu = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
		optionsMenu.enabled = false;

		pauseMenu = gameObject.GetComponent<Canvas>();
		pauseMenu.enabled = false;

        quitMenu = GameObject.Find("QuitMenu").GetComponent<Canvas>();
        quitMenu.enabled = false;

        //storage = GameObject.Find("Storage").GetComponent<StorageScript>();



    }


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            if (!paused)
            {
                PauseGame();
            }
            else
            { 
                if (!optionsMenu.enabled && !quitMenu.enabled) UnPauseGame();
                else Close();
            }
		}


    }

	void MouseControl (bool b)
	{
		if (b) 
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		} 
		else 
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

	}

	public void PauseGame () 
	{
		paused = true;
		Time.timeScale = 0;
		MouseControl(true);


		pauseMenu.enabled = true;
	}
	public void UnPauseGame () 
	{
		paused = false;
		Time.timeScale = 1;
		MouseControl(false);

		pauseMenu = pauseMenu.GetComponent<Canvas>();
		pauseMenu.enabled = false;
	}

	public void RestartLevel()
	{
		paused = false;
		Time.timeScale = 1;

        //GameObject.FindWithTag("Player").GetComponent<AudioController>().StoreAndContinue();
        Scene current = SceneManager.GetActiveScene();
		SceneManager.LoadScene(current.name);
        
	}

	public void QuitToMenu()
	{
		paused = false;
		Time.timeScale = 1;

		//GameObject.FindWithTag("Player").GetComponent<AudioController>().StoreAndContinue();

		SceneManager.LoadScene ("MainMenu");
		
	}
    public void OptionsMenu()
    {
        optionsMenu.enabled = true;

        GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>().DisableSubOptions();
    }

    public void QuitMenu()
    {
        quitMenu.enabled = true;
    }

    public void Close()
    {
        if (optionsMenu.enabled) optionsMenu.enabled = false;
        if (quitMenu.enabled) quitMenu.enabled = false;
    }

}
