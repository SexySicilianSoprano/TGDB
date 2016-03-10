using UnityEngine;
using System.Collections;

public class FishingBoat : Vehicle {

	// Use this for initialization
	protected new void Start () 
	{
		//Assign variables for health/movement and so on..
		AssignDetails (ItemDB.FishingBoat);
		GetComponent<Movement>().AssignDetails (ItemDB.FishingBoat);
        //GetComponent<Combat>().AssignDetails(WeaponDB.TestCannon);
        		
        //Call base class start
		base.Start ();
	}
	
	// Update is called once per frame
	protected new void Update () 
	{
		base.Update ();
	}
}