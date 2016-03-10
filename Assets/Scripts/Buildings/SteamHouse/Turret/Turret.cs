using UnityEngine;
using System.Collections;

public class Turret : Building {

	// Use this for initialization
	new void Start () 
	{
		AssignDetails (ItemDB.Turret);
        GetComponent<Combat>().AssignDetails(WeaponDB.TurretCannon);
        //Spawner = gameObject.GetComponent<UnitSpawner>();
        base.Start ();
	}
	
	// Update is called once per frame
	void Update () 
	{
        base.Update();
	}
}
