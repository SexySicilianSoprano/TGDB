using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RTSEntity))]
public class CannonCombat : Combat {

    // ##### Private variables #####
    // Private booleans
    private bool TargetSet = false; // Is a target set?
    private bool canFire = true; // Are we able to fire?
    private bool m_FollowEnemy = false; // Do we follow a fleeing enemy?
    private bool m_FireAtEnemy = false; // Do we fire at an enemy without command?
    private bool isFollowing = false; // Are we currently following the enemy?
    private bool inCombat = false;
    private bool movementOrderGiven { get { return m_Movement.onTheMove; } } // Is movement order given?
 
    // Call this outside combat script to see if the unit is currently in combat
    public override bool isInCombat
    {
        get
        {
            return TargetSet;
        }
    }

    // Rate of fire
    private float m_FireRate;

    // Position variables
    private Vector3 CurrentPos;
    private Vector3 TargetPos;

    // Projectile spawner / turret variables
    private Transform Spawner;
    private Vector3 SpawnerPos;

    // SphereCollider with trigger to detect enemies private SphereCollider DangerZone
    private SphereCollider DangerZone
    { get { return transform.Find("DangerZone").GetComponent<SphereCollider>(); } }

    // List of targets and priorities
    private List<RTSEntity> targetList = new List<RTSEntity>(); // Normal priority
    private List<RTSEntity> trueTargetList = new List<RTSEntity>(); // High priority

    // This unit's movement script
    private Movement m_Movement;
        
    // Use this for initialization
    void Start()
    {
        // Let's set the combat mode and assign components to their respective variables
        SwitchMode(CombatMode.Defensive);
        m_Parent = GetComponent<RTSEntity>();
        Spawner = m_Parent.transform.GetChild(0);
        m_Movement = GetComponent<Movement>();
}

    void FixedUpdate()
    {
        // Updates positions and firerate
        SpawnerPos = Spawner.transform.position;
        CurrentPos = CurrentLocation;
        CalculateFireRate();

        // Check if lists need refreshing aka target is destroyed or off the DangerZone
        RefreshTargetLists(m_Target);

        // Behaviour query
        if (!TargetSet && m_Parent.AttackingEnemy)
        {
            Attack(m_Parent.AttackingEnemy);
        }
        else if (TargetSet && m_Target == null)
        {
            // Target is set, but can't be found, so let's stop
            Stop();
        }
        else if (m_Target && canFire == true)
        {
            // Target is set and found, let's update locations and fire
            TargetPos = TargetLocation;
            Attack(m_Target);
        }
        else
        {
            // Target is not set, we're idle, so let's see if target lists have anything to shoot at
            // Is there a unit listed on top priority list?
            if (trueTargetList.Count > 0)
            {
                Attack(trueTargetList[0]);
            }
            // If not, is there a unit listed on normal priority list?
            else if (targetList.Count > 0)
            {
                Attack(targetList[0]);
            }
            else
            {
                Stop();
            }
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
        Damage = weapon.Damage;
        Range = weapon.Range;
        FireRate = weapon.FireRate;
        TurretSpeed = weapon.TurretSpeed;
        isAntiArmor = weapon.isAntiArmor;
        isAntiStructure = weapon.isAntiStructure;
        //Projectile = weapon.Projectile;
        DangerZone.radius = weapon.Range;
    }

    // Attack with command
    public override void AttackCommand(RTSEntity obj)
    {
        m_FollowEnemy = true;
        //PutTopOfTargetList(obj);
        Attack(obj);
    }

    
    // Attack without command
    public void Attack(RTSEntity obj)
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
                        // Stop movement and fire the guns
                        isFollowing = false;

                        // If we didn't give a move order while in combat
                        if (!movementOrderGiven && !inCombat)
                        {
                            m_Movement.Stop();
                        }

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
                        // Target not in range, follow it!
                        Follow();
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
            // Just stop, ain't worth shitting around with no target, mate
            Stop();
            return;
        }
    }

    private void Fire()
    {
        // Start firing
        gameObject.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play(true);
       
        // Set us in combat
        inCombat = true;

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
        inCombat = false;
        TargetSet = false;
        m_Target = null;
        m_Parent.AttackingEnemy = null;
        isFollowing = false;
    }

    // Follow the target
    public void Follow() {
        // Follow target until in range
        if (m_FollowEnemy)
        {
            if (!isFollowing)
            {
                isFollowing = true;
                m_Movement.Follow(m_Target.transform);
            }

            if (TargetInRange())
            {
                m_Movement.Stop();
            }
        }
    }

    // Launches projectile
    private void LaunchProjectile()
    {
           
    }

    // Checks if target is in line of fires
    private bool TargetInLine()
    {
        Ray ray = new Ray(Spawner.transform.position, Spawner.transform.forward);
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
        }

        // We're not hitting anything, try again
        return false;
    }

    // Checks for enemies within range
    private void OnTriggerEnter(Collider collider)
    {
        GameObject target = collider.gameObject;

        // Ignore danger zones
        if (collider == GetComponent<SphereCollider>())
        {
            Physics.IgnoreCollision(collider, GetComponent<SphereCollider>());
        }

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
                    // Yeah, just a building. Put on low prio list
                    targetList.Add(m_ent);

                }
                // Is it a unit?
                if (target.GetComponent<Unit>())
                {
                    // Put on high prio list
                    trueTargetList.Add(m_ent);
                }
            }
        }
        
    }

    // What happens when an unit exits the DangerZone
    private void OnTriggerExit(Collider collider)
    { 
        // Ignore danger zones
        if (collider == GetComponent<SphereCollider>())
        {
            Physics.IgnoreCollision(collider, GetComponent<SphereCollider>());
        }

        GameObject target = collider.gameObject;
        
        if (collider == GetComponent<BoxCollider>() && target.GetComponent<RTSEntity>())
        {
            if (target.GetComponent<RTSEntity>() && target.tag != m_Parent.tag)
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
                m_FollowEnemy = false;
                m_FireAtEnemy = false;
                break;

            case CombatMode.Aggressive:
                m_FollowEnemy = true;
                m_FireAtEnemy = true;
                break;

            case CombatMode.Defensive:
                m_FollowEnemy = false;
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
    public enum CombatMode
    {
        Passive,
        Aggressive,
        Defensive
    }

}
