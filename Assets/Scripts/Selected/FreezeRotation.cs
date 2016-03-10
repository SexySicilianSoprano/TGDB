using UnityEngine;
using System.Collections;

public class FreezeRotation : MonoBehaviour {

    private Quaternion rotation;
    public bool Freeze = false;

    // Use this for initialization
    void Start ()
    {
        // Set a static rotation
        rotation = new Quaternion(90, 0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (Freeze)
        {
            // Fix rotation when frozen
            transform.rotation = rotation;
        }
    }
}
