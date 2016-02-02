using UnityEngine;
using System.Collections;

public class Refinery : Building {

	// Use this for initialization
	new void Start () 
	{
		AssignDetails (ItemDB.Refinery);
        //Spawner = gameObject.GetComponent<UnitSpawner>();
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
