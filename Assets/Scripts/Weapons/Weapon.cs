using UnityEngine;
using System.Collections;

public class Weapon {

    // Insert weapon parameters

    // Weapon Identification
    public int ID;
    public string Name;

    // Weapon type
    public bool isAntiArmor;
    public bool isAntiStructure;

    // Weapon stats
    public float Damage;
    public float Range;
    public float FireRate;
    public float TurretSpeed;

    // Projectile and animation
    //public Projectile Projectile;
   
    public void AssignDetails (Weapon Weapon)
    {
        ID = Weapon.ID;
        Name = Weapon.Name;
        Damage = Weapon.Damage;
        Range = Weapon.Range;
        TurretSpeed = Weapon.TurretSpeed;
        FireRate = Weapon.FireRate;
        isAntiArmor = Weapon.isAntiArmor;
        isAntiStructure = Weapon.isAntiStructure;
        //Projectile = Weapon.Projectile;
    }

}
