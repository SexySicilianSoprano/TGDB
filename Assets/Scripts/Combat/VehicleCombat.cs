using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(RTSEntity))]
public class VehicleCombat : Combat {

    // ##### Private variables #####
    private bool TargetSet = false;
    private bool canFire = true;
    private bool m_FollowEnemy = false;
    private bool m_FireAtEnemy = false;

    private float m_FireRate;

    private Vector3 CurrentPos;
    private Vector3 TargetPos;
    
    private Transform Spawner;
    private Vector3 SpawnerPos;
        
    // Use this for initialization
    void Start()
    {
        SwitchMode(CombatMode.Defensive);
        m_Parent = GetComponent<RTSEntity>();
        Spawner = m_Parent.transform.GetChild(0);        
    }

    void Update()
    {
        SwitchMode(CombatMode.Defensive);
        SpawnerPos = Spawner.transform.position;
        CurrentPos = CurrentLocation;
        CalculateFireRate();

        if (TargetSet && m_Target == null)
        {
            Stop();
        }
        else if (TargetSet && canFire == true)
        {
            TargetPos = TargetLocation;            
            Attack(m_Target);
        } 

        if (m_Parent.AttackingEnemy)
        {
            m_Target = m_Parent.AttackingEnemy;
            if (m_FireAtEnemy == true)
            {
                Attack(m_Target);
            }
            else if (m_FireAtEnemy == false)
            {

            }
            else
            {
                Debug.LogError("Something went wrong with stances");
            }
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
                        Follow();
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

    private void Fire()
    {
        // Start firing
        
        gameObject.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play(true);
        Debug.DrawLine(SpawnerPos, TargetPos);
        
        //LaunchProjectile(Projectile);
        m_Target.TakeDamage(Damage);
        m_Target.AttackingEnemy = m_Parent;
        canFire = false;
        StartCoroutine(WaitAndFire());
    }

    public override void Stop()
    {
        // Set no target and target to null
        TargetSet = false;
        m_Target = null;
        m_Parent.AttackingEnemy = null;
    }

    public void Follow() {
        // Follow target until in range
        if (m_FollowEnemy)
        {
            GetComponent<Movement>().Follow(m_Target.transform);

            if (TargetInRange())
            { 
                GetComponent<Rigidbody>().velocity = Vector3.zero;
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
        //Vector3 Forward = Spawner.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Ray ray = new Ray(Spawner.transform.position, Spawner.transform.forward);
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
       

    private void RotateTowards(Vector3 location)
    {
        Vector3 m_Direction = (location - Spawner.transform.position).normalized;

        Quaternion m_LookRotation = Quaternion.LookRotation(new Vector3(m_Direction.x, m_Direction.y * 0, m_Direction.z));
        
        Spawner.transform.rotation = Quaternion.Slerp(Spawner.transform.rotation, m_LookRotation, TurretSpeed * Time.deltaTime);
    }

    private void CalculateFireRate() {
        // Calculates rate of fire with 60 divided by shots per minute
        m_FireRate = 60 / FireRate;
    }
    
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

    IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(m_FireRate);
        canFire = true;
    }

    public enum CombatMode {
        Passive,
        Aggressive,
        Defensive
    } 
  
}
