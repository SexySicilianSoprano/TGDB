using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// This is the <TurretCombat> script. It's used to give behaviour to enemy towers
/// It is fundamentally same as <CannonCombat>, but modified to fire autonomously (wait, that could be used in normal <CannonCombat>)
/// 
/// Well anywho, pretty much nothing else to report. Apart from autonomous firing, it's pretty much the same.
/// Just commented better.
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// </summary>

[RequireComponent(typeof(RTSEntity))]
public class TurretCombat : Combat {

    // Private booleans
    private bool TargetSet = false; // Is a target set?
    private bool canFire = true; // Are we able to fire?
    private bool m_FollowEnemy = false; // Do we follow a fleeing enemy?
    private bool m_FireAtEnemy = false; // Do we fire at an enemy without command?

    // Rate of fire
    private float m_FireRate;

    // Position variables
    private Vector3 CurrentPos;
    private Vector3 TargetPos;
        
    // Projectile spawner / turret variables
    private Transform Spawner;
    private Vector3 SpawnerPos;

    // SphereCollider with trigger to detect enemies
    private SphereCollider DangerZone
    { get { return GetComponent<SphereCollider>(); } }
    private Projector DangerZoneProjector
    { get { return transform.Find("Projector").GetComponent<Projector>(); } }

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
    }

    void FixedUpdate()
    {        
        // Updates positions and firerate
        SpawnerPos = Spawner.transform.position;
        CurrentPos = CurrentLocation;
        CalculateFireRate();

        // Check if lists need refreshing aka target is destroyed or off the DangerZone
        RefreshTargetLists(m_Target);

        // Is there a unit listed on top priority list?
        if (trueTargetList.Count > 0)
        {
            AttackCommand(trueTargetList[0]);
        }
        // If not, is there a unit listed on normal priority list?
        else if (targetList.Count > 0)
        {            
            AttackCommand(targetList[0]);
        }
        else
        {
            Stop();
        }
    }

   // Host unit location
    public override Vector3 CurrentLocation
    {
        get
        {
            return m_Parent.transform.position;
        }
    }

    // Target unit location
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
        Damage = weapon.Damage * 2;
        Range = weapon.Range;
        FireRate = weapon.FireRate;
        TurretSpeed = weapon.TurretSpeed;
        isAntiArmor = weapon.isAntiArmor;
        isAntiStructure = weapon.isAntiStructure;
        //Projectile = weapon.Projectile;
        DangerZone.radius = weapon.Range;
        DangerZoneProjector.orthographicSize = weapon.Range / 1.7f;
    }
       
    // Attack command
    public override void AttackCommand(RTSEntity obj)
    {
        // Set target
        m_Target = obj;
        TargetSet = true;

        // Does target exist?
        if (m_Target)
        {
            // Update target location
            TargetPos = TargetLocation;

            // Target in line?
            if (TargetInLine())
            {
                // Rotate towards target just in case, doesn't hurt
                RotateTowards(TargetPos);

                // Are we ready to fire?
                if (canFire)
                {
                    // Is the target within maximum range?
                    if (TargetInRange())
                    {
                        // FIRE!
                        Fire();

                        // Check if target is destroyed after the shot
                        if (m_Target == null)
                        {
                            // We can't keep firing against nothing, so let's call it quits
                            Stop();
                            return;
                        }
                    }
                    else
                    {
                        // Target isn't in range, so let's forget about it
                        Stop();
                        return;
                    }
                }
                else
                {
                    // Wait until we can fire again
                    WaitAndFire();
                }
            }
            else
            {
                // Rotate the turret
                RotateTowards(TargetPos);
            }            
        }
        else
        {
            // Just stop, ain't worth shitting around with no target
            Stop();
            return;
        }
    }

    // Fire the weapon
    private void Fire()
    {
        // Play firing      
        Spawner.GetChild(0).GetComponent<ParticleSystem>().Play(true);
        //debug.log("FIRE TURRET, FIRE!");
        
        //LaunchProjectile(Projectile); // Launch projectile
        m_Target.TakeDamage(Damage); // Target takes damage
        m_Target.AttackingEnemy = m_Parent; // Let the target know it's being fired so it can shoot back
        canFire = false; // We can't fire anymore
        StartCoroutine(WaitAndFire()); // Start cooldown routine so we can fire again
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
        // TODO: Launch a projectile, you hobo
    }

    // Checks if target is in line of fires
    private bool TargetInLine()
    {
        Vector3 m_SpawnPos = new Vector3(SpawnerPos.x, 2f, SpawnerPos.z);
        Ray ray = new Ray(m_SpawnPos, Spawner.transform.forward);
        // Raycastin' yo
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            // Did we hit the target's box collider?
            if (hit.collider == m_Target.GetComponent<BoxCollider>())
            {
                // Yup, target on sights
                return true;
            }
            else
            {
                // Apparently not
                return false;
            }
        }

        // We're not hitting anything, try again
        return false;
    }

    // Checks for enemies within range
    private void OnTriggerStay(Collider collider)
    {
        if (collider == GetComponent<SphereCollider>())
        {
            Physics.IgnoreCollision(collider, GetComponent<SphereCollider>());
        }

        GameObject target = collider.gameObject;
        // Is it a unit or a building?
        if (!collider.isTrigger && target.GetComponent<RTSEntity>() && target.tag != m_Parent.tag)
        {
            RTSEntity m_ent = target.GetComponent<RTSEntity>();

            if (!targetList.Contains(m_ent) && !trueTargetList.Contains(m_ent))
            {
                //debug.log("Target not found on list " + m_ent);
                // Is it a building?
                if (target.GetComponent<Building>())
                {
                    //debug.log("It's a building! " + m_ent);
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
                    // Add unit to target list
                    targetList.Add(m_ent);
                }
            }
        }
    }

    // What happens when an unit exits the DangerZone
    private void OnTriggerExit(Collider collider)
    {        
        if (collider == GetComponent<SphereCollider>())
        {
            Physics.IgnoreCollision(collider, GetComponent<SphereCollider>());
        }

        GameObject target = collider.gameObject;

        if (collider == GetComponent<BoxCollider>() && target.GetComponent<RTSEntity>() && target.tag != m_Parent.tag)
        {
            RTSEntity m_ent = target.GetComponent<RTSEntity>();

            // Is the unit a target or a true target?
            if (targetList.Contains(m_ent))
            {
                // Delete the exiting unit from target list
                RefreshTargetLists(m_ent);
            }
            else if (trueTargetList.Contains(m_ent))
            {
                // Delete the exiting unit from true target list
                RefreshTargetLists(m_ent);
            }
        }        
    }
        
    // Updates the top priority list and puts the parameter unit on top
    private void PutTopOfTargetList(RTSEntity unit)
    {
        List<RTSEntity> placeHolderList = new List<RTSEntity>();

        if (trueTargetList.Count > 0)
        {
            // Copy true list to placeholder
            foreach (RTSEntity ent in trueTargetList)
            {
                placeHolderList.Add(ent);
            }             
            trueTargetList.Clear(); // Clear true list
        }

        trueTargetList.Add(unit); // Add the target 

        // Add all units in placeholder to true list
        if (placeHolderList.Count > 0)
        {
            foreach (RTSEntity ent in placeHolderList)
            {
                trueTargetList.Add(ent);
            }
        }
    }

    // Cleans lists of units that are either destroyed or not within range any longer
    private void RefreshTargetLists(RTSEntity target)
    {
        // Let's create a temp list
        List<RTSEntity> tempList = new List<RTSEntity>();

        // Is the unit empty and contained within target list?
        // If no, is it not empty and just not within range?
        if (target == null && targetList.Contains(target) || target != null && !TargetInRange())
        {
            targetList.Remove(target);

            // Clean the target list
            foreach (RTSEntity unit in targetList)
            {
                if (unit)
                {
                    // Add existing units to temp
                    tempList.Add(unit);
                }
            }

            targetList.Clear(); // Empty the target list for re-filling

            foreach (RTSEntity unit in tempList)
            {
                targetList.Add(unit); // Add each unit in tempList back to targetList
            }

            tempList.Clear(); // Finally, clear the temp            
        }

        // Is the unit empty and contained within true target list?
        // If no, is it not empty and just not within range?
        if (target == null && trueTargetList.Contains(target) || target != null && !TargetInRange())
        {
            trueTargetList.Remove(target);

            // Clean the target list
            foreach (RTSEntity unit in trueTargetList)
            {
                if (unit)
                {
                    // Add existing units to temp
                    tempList.Add(unit);
                }
            }

            trueTargetList.Clear(); // Empty the target list for re-filling

            foreach (RTSEntity unit in tempList)
            {
                trueTargetList.Add(unit); // Add each unit in tempList back to targetList
            }

            tempList.Clear(); // Finally, clear the temp
        }
    }

    // Rotate the turret towards the target, else it cannot fire
    private void RotateTowards(Vector3 location)
    {
        // Calculate the direction we want to face
        Vector3 m_Direction = (location - Spawner.transform.position).normalized;

        // Calculate the quaternion lookrotation with vector we just determined
        Quaternion m_LookRotation = Quaternion.LookRotation(new Vector3(m_Direction.x, m_Direction.y * 0, m_Direction.z));
        
        // Use quaternion slerp to rotate the turret towards desired position with set speed
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

        // Is the distance between target and parent smaller than maximum range?
        if (dist <= Range)
        {            
            // Yup, we're good to go
            return true;
        }
        else
        {
            // Nope, it's too far away
            return false;
        }
    }

    // Switches between combat modes, this activates booleans that control firing behaviour
    // Combat mode
    // - Passive doesn't fire back at all
    // - Aggressive fires when enemy is within range
    // - Defensive fires only when attacked
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
