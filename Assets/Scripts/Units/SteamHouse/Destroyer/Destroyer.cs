using UnityEngine;
using System.Collections;

public class Destroyer : Vehicle {

	// Use this for initialization
	protected new void Start () 
	{
		//Assign variables for health/movement and so on..
		AssignDetails (ItemDB.Destroyer);
		GetComponent<Movement>().AssignDetails (ItemDB.Destroyer);
        GetComponent<Combat>().AssignDetails(WeaponDB.DestroyerCannon);
        		
        //Call base class start
		base.Start ();
	}
	
	// Update is called once per frame
	protected new void Update () 
	{
		base.Update ();
	}
}