using UnityEngine;
using System.Collections;

public class QuitApplication : MonoBehaviour {

    /// <summary>
    /// This Script is made for quitting the game. Put this for UI buttons as OnClick() command for it to work, from MenuUI/PauseUI etc. game objects.
    /// </summary>

	public void Quit()
	{
		//If we are running in a standalone build of the game
	#if UNITY_STANDALONE
		//Quit the application
		Application.Quit();
	#endif

		//If we are running in the editor
	#if UNITY_EDITOR
		//Stop playing the scene
		UnityEditor.EditorApplication.isPlaying = false;
	#endif
	}
}
