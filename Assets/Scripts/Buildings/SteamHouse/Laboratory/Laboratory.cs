using UnityEngine;
using System.Collections;

public class Laboratory : Building {

	// Use this for initialization
	new void Start ()
    {
        AssignDetails(ItemDB.Laboratory);

        base.Start();
    }
	
	// Update is called once per frame
	void Update ()
    {
       
	}
}
