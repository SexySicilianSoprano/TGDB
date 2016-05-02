using UnityEngine;
using System.Collections;

/// <summary>
/// --- LAUNCHER COMBAT ---
/// 
/// This is a weapon script that is designed to fire projectiles in an arc.
/// The projectiles themselves are designed to do the damage instead of this script.
/// It's nice to have nice things for once. Nice.
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// </summary>

[RequireComponent(typeof(RTSEntity))]
public class LauncherCombat : Combat
{

    // ##### Private variables #####
    private bool TargetSet = false;
    private bool canFire = true;
    private bool m_FollowEnemy = true;
    private bool m_FireAtEnemy = false;

    // Rate of fire
    private float m_FireRate;

    // Minimum range of fire
    private float m_minimumRange = 15;

    // Position variables
    private Vector3 CurrentPos;
    private Vector3 TargetPos;

    // Projectile spawner / turret variables
    private Transform Spawner;
    private Vector3 SpawnerPos;

    // Movement script
    private Movement m_Movement;

    // Use this for initialization
    void Start()
    {
        // Let's set the combat mode and assign components to their respective variables
        SwitchMode(CombatMode.Defensive);
        m_Parent = GetComponent<RTSEntity>();
        m_Movement = GetComponent<Movement>();
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

    // Attack with command
    public override void AttackCommand(RTSEntity obj)
    {
        m_FollowEnemy = true;
        Attack(obj);
    }

    public void Attack(RTSEntity obj)
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
                        m_Movement.Stop();
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

        LaunchProjectile();
        //m_Target.TakeDamage(Damage);
        if (!m_Target.AttackingEnemy)
        {
            m_Target.AttackingEnemy = m_Parent;
        }
        canFire = false;
        StartCoroutine(WaitAndFire());
    }

    public override void Stop()
    {
        // Set no target and target to null
        TargetSet = false;
        m_Target = null;
        m_Parent.AttackingEnemy = null;
        //GetComponent<Movement>().Stop();
    }

    public void Follow()
    {
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
        // Create and instantiate a projectile
        GameObject projectile = Resources.Load("Projectiles/CannonBall", typeof (GameObject)) as GameObject;
        GameObject newProjectile = Instantiate(projectile, SpawnerPos, Quaternion.identity) as GameObject;

        // Setting up vectors
        Vector3 targetPosition = TargetLocation; // Target position assigned to temporary variable
        Vector3 dir = targetPosition - SpawnerPos; // Direction from Spawner to Target

        // Let's calculate firing properties
        float height = dir.y; // Get height difference
        dir.y = 0; // Retain only the horizontal distance
        float dist = dir.magnitude; // Get horizontal distance
        dir.y = dist; // Set elevation to 45 degrees
        dist += height; // Correct for different heights
        float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude); // Add gravity to velocity
        Vector3 newVel = vel * dir.normalized; // Add velocity to direction

        // Give your nice new bombshells some orders
        newProjectile.GetComponent<Rigidbody>().velocity = newVel;
        newProjectile.GetComponent<ProjectileBehaviour>().targetPosition = targetPosition;
        newProjectile.GetComponent<ProjectileBehaviour>().parent = m_Parent;
    }

    // Checks if target is in line of fires
    private bool TargetInLine()
    {
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

    private void CalculateFireRate()
    {
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

    public enum CombatMode
    {
        Passive,
        Aggressive,
        Defensive
    }

}
