using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeaponDB {

    private static List<Weapon> AllWeapons = new List<Weapon>();

    public static Weapon DestroyerCannon = new Weapon
    {
        ID = 0,
        Name = "Destroyer Cannon",
        Damage = 10,
        Range = 50,
        FireRate = 30,
        TurretSpeed = 2.0f,
        isAntiArmor = true,
        isAntiStructure = false,
        //Projectile = ProjectileDB.CannonBall
        
    };

    public static Weapon ScoutMachineGun = new Weapon
    {
        ID = 0,
        Name = "Scout Machinegun",
        Damage = 2,
        Range = 100,
        FireRate = 160,
        TurretSpeed = 3f,
        isAntiArmor = true,
        isAntiStructure = false,
        //Projectile = ProjectileDB.CannonBall
    };

    public static Weapon TurretCannon = new Weapon
    {
        ID = 0,
        Name = "Turret Cannon",
        Damage = 10,
        Range = 100,
        FireRate = 30,
        TurretSpeed = 2.0f,
        isAntiArmor = true,
        isAntiStructure = false,
        //Projectile = ProjectileDB.CannonBall

    };

    public static void Initialise()
    {
        InitialiseWeapon(DestroyerCannon);
        InitialiseWeapon(ScoutMachineGun);
        InitialiseWeapon(TurretCannon);
    }

    private static void InitialiseWeapon(Weapon weapon)
    {
        AllWeapons.Add(weapon);
    }

}
