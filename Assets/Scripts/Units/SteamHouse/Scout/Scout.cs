using UnityEngine;
using System.Collections;

public class Scout : Vehicle {

	// Use this for initialization
	protected new void Start ()
    {
        AssignDetails(ItemDB.Scout);
        GetComponent<Movement>().AssignDetails(ItemDB.Scout);
        GetComponent<Combat>().AssignDetails(WeaponDB.TestMachineGun);

        base.Start();
    }
	
	// Update is called once per frame
	protected new void Update ()
    {
        base.Update();
	}
}
