using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {

    static DontDestroy instance = null;

	void Awake()
	{
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Duplicate destroyed");
        }
        else
        {
            instance = this;
            //Causes UI object not to be destroyed when loading a new scene. If you want it to be destroyed, destroy it manually via script.
            DontDestroyOnLoad(this.gameObject);
        }

	}

	

}
