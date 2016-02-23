using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// This is the <TurretCombat> script. It's used to give behaviour to enemy towers
/// It is fundamentally same as <CannonCombat>, but modified to fire autonomously (wait, that could be used in normal <CannonCombat>
/// 
/// Well anywho, pretty much nothing else to report.
/// - Karl Sartorisio
/// The Great Deep Blue
/// </summary>

[RequireComponent(typeof(RTSEntity))]
public class TurretCombat : Combat {

    // Private booleans
    private bool TargetSet = false;
    private bool canFire = true;
    private bool m_FollowEnemy = false;
    private bool m_FireAtEnemy = false;

    // Rate of fire
    private float m_FireRate;

    // Position variables
    private Vector3 CurrentPos;
    private Vector3 TargetPos;
        
    // Projectile spawner / turret variables
    private Transform Spawner;
    private Vector3 SpawnerPos;

    // SphereCollider with trigger to detect enemies
    private SphereCollider DangerZone;
    
    // List of targets and priorities
    private List<RTSEntity> targetList = new List<RTSEntity>(); // Normal priority
    private List<RTSEntity> trueTargetList = new List<RTSEntity>(); // High priority

    // Use this for initialization
    void Start()
    {
        // Let's set the combat mode and assign components to their respective variables
        SwitchMode(CombatMode.Defensive);
        m_Parent = GetComponent<RTSEntity>();
        Spawner = m_Parent.transform.GetChild(0);

        // Initialise DangerZone and set its size
        DangerZone = transform.GetComponent<SphereCollider>();
        DangerZone.radius += 100;
    }

    void Update()
    {        
        // Updates positions and firerate
        SpawnerPos = Spawner.transform.position;
        CurrentPos = CurrentLocation;
        CalculateFireRate();

        // Is there a unit listed on top priority list?
        if (trueTargetList.Count >= 1)
        {
            CleanTargetLists();
            Attack(trueTargetList[0]);
        }
        // If not, is there a unit listed on normal priority list?
        else if (trueTargetList.Count <= 0 && targetList.Count >= 1)
        {
            CleanTargetLists();
            Attack(targetList[0]);
            
        }

        // If target is destroyed yet targetset is still true, this stops the program for accessing the enemy endlessly
        if (TargetSet && m_Target == null)
        {
            Stop();
        }
        // Enemy can be found, target is set and we're ready to fire
        else if (TargetSet && m_Target != null && canFire == true) 
        {
            TargetPos = TargetLocation;            
            Attack(m_Target);
        } 
        
    }

    // ##### Assign Details #####
    public override Vector3 CurrentLocation
    {
        get
        {
            return m_Parent.transform.position;
        }
    }

    public override Vector3 TargetLocation
    {
        get
        {
            return m_Target.transform.position;
        }
    }
    
    // Assign weapon details
    public override void AssignDetails(Weapon weapon)
    {
        Damage = weapon.Damage;
        Range = weapon.Range;
        FireRate = weapon.FireRate;
        TurretSpeed = weapon.TurretSpeed;
        isAntiArmor = weapon.isAntiArmor;
        isAntiStructure = weapon.isAntiStructure;
        //Projectile = weapon.Projectile;
    }
       
    // Attack command
    public override void Attack(RTSEntity obj)
    {
        m_Target = obj;
        TargetSet = true;
        if (m_Target)
        {
            if (TargetInLine())
            {
                RotateTowards(TargetPos);
                if (canFire)
                {
                    // Is the target within maximum range?
                    if (TargetInRange())
                    {
                        Fire();

                        if (m_Target == null)
                        {
                            Stop();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    WaitAndFire();
                }
            }
            else
            {
                RotateTowards(TargetPos);
            }            
        }
        else
        {
            Stop();
        }
    }

    // Fire the weapon
    private void Fire()
    {
        // Start firing        
        //gameObject.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play(true);
        Debug.Log("FIRE TURRET, FIRE!");
        
        //LaunchProjectile(Projectile);
        m_Target.TakeDamage(Damage);
        m_Target.AttackingEnemy = m_Parent;
        canFire = false;
        StartCoroutine(WaitAndFire());
    }

    // Stops just whatever is being done
    public override void Stop()
    {
        // Set no target and target to null
        TargetSet = false;
        m_Target = null;
        m_Parent.AttackingEnemy = null;
    }

    // Launches projectile
    private void LaunchProjectile()
    {
        
    }

    // Checks if target is in line of fires
    private bool TargetInLine()
    {
        Vector3 m_SpawnPos = new Vector3(SpawnerPos.x, 0.1f, SpawnerPos.z);
        RaycastHit hit;
        Ray ray = new Ray(m_SpawnPos, Spawner.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == m_Target.GetComponent<BoxCollider>())
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        else
        {            
            return false;
        }
    }

    // Checks for enemies within range
    private void OnTriggerStay(Collider collider)
    {
        GameObject target = collider.gameObject;

        // Is it a unit or a building?
        if (target.GetComponent<RTSEntity>() && target.tag == "Player1")
        {
            RTSEntity m_ent = target.GetComponent<RTSEntity>();
            if (!targetList.Contains(m_ent) && !trueTargetList.Contains(m_ent))
            {
                Debug.Log("Target not found on list " + m_ent);
                // Is it a building?
                if (target.GetComponent<Building>())
                {
                    Debug.Log("It's a building! " + m_ent);
                    // Is it a Naval Yard?
                    if (target.GetComponent<NavalYard>())
                    {
                        // Naval Yard in range, fire at Naval Yard
                        // Add to top priority list
                        PutTopOfTargetList(m_ent);
                    }
                    else if (target.GetComponent<FloatingFortress>())
                    {
                        // No Naval Yard or other buildings in range, fire at Floating Fortress
                        // Add to priority list
                        trueTargetList.Add(m_ent);
                    }
                }
                // Is it a unit?
                if (target.GetComponent<Unit>())
                {
                    Debug.Log("It's a goddamn unit!");
                    targetList.Add(m_ent);
                }
            }
        }
    }

    /*
    // Detects enemies within turret's effective range
    private void DetectEnemies()
    {
        // Are there any objects within range?
        Ray ray = new Ray(CurrentLocation, transform.forward);
        if (Physics.SphereCast(CurrentLocation, 360f, out hit, Range, ~(8 << 14)))
        {
            GameObject target = hit.collider.gameObject;
            RTSEntity m_target = target.GetComponent<RTSEntity>();
            
            if (!targetList.Contains(m_target))
            {
                // Is it a building?
                if (target.GetComponent<Building>() && target.tag == "Player1")
                {
                    // Is it a Naval Yard?
                    if (target.GetComponent<NavalYard>())
                    {
                        // Naval Yard in range, fire at Naval Yard
                        // Add to top priority list
                        PutTopOfTargetList(m_target);
                        return;
                    }
                    else if (target.GetComponent<FloatingFortress>())
                    {
                        // No Naval Yard or other buildings in range, fire at Floating Fortress
                        trueTargetList.Add(m_target);
                        // Add to priority list
                        return;
                    }
                }
                // Is it a unit?
                if (target.GetComponent<Unit>() && target.tag == "Player1")
                {
                    targetList.Add(m_target);
                }
            }
        }
    }*/

    // Updates the top priority list and puts the parameter unit on top
    private void PutTopOfTargetList(RTSEntity unit)
    {        
        List<RTSEntity> placeHolderList = trueTargetList; // Copy true list to placeholder
        trueTargetList.Clear(); // Clear true list
        trueTargetList.Add(unit); // Add the target 

        // Add all units in placeholder to true list
        foreach (RTSEntity ent in placeHolderList)
        {
            trueTargetList.Add(ent);
        }     
    }

    // Cleans lists of units that are not within range anymore
    private void CleanTargetLists()
    {
        // Clean the target list
        foreach (RTSEntity unit in targetList)
        {
            if (Vector3.Distance(unit.transform.position, CurrentLocation) > Range)
            {
                targetList.Remove(unit);
            }
        }

        // Clean the true target list
        foreach (RTSEntity unit in trueTargetList)
        {
            if (Vector3.Distance(unit.transform.position, CurrentLocation) > Range)
            {
                trueTargetList.Remove(unit);
            }
        }
    }

    // Rotate the turret towards the target, else it cannot fire
    private void RotateTowards(Vector3 location)
    {
        Vector3 m_Direction = (location - Spawner.transform.position).normalized;

        Quaternion m_LookRotation = Quaternion.LookRotation(new Vector3(m_Direction.x, m_Direction.y * 0, m_Direction.z));
        
        Spawner.transform.rotation = Quaternion.Slerp(Spawner.transform.rotation, m_LookRotation, TurretSpeed * Time.deltaTime);
    }

    // Calculates rate of fire with 60 divided by shots per minute
    private void CalculateFireRate()
    {        
        m_FireRate = 60 / FireRate;
    }
    
    // Checks if the target is within range
    private bool TargetInRange()
    { 
        TargetPos = TargetLocation;
        float dist = Vector3.Distance(CurrentPos, TargetPos);

        if (dist <= Range)
        {            
            return true;
        }
        else
        {
            return false;
        }
    }

    // Switches between combat modes, this activates booleans that control firing behaviour
    public void SwitchMode(CombatMode mode)
    {
        switch (mode)
        {
            case CombatMode.Passive:
                break;

            case CombatMode.Aggressive:
                break;

            case CombatMode.Defensive:
                m_FollowEnemy = true;
                m_FireAtEnemy = true;
                break;

        }
    }

    // A coroutine that calculates when we can fire again
    IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(m_FireRate);
        canFire = true;
    }

    // Combat mode
    // - Passive doesn't fire back at all
    // - Aggressive fires when enemy is within range
    // - Defensive fires only when attacked
    public enum CombatMode {
        Passive,
        Aggressive,
        Defensive
    } 
  
}
