using UnityEngine;
using System.Collections;

public abstract class Combat : MonoBehaviour{

    protected RTSEntity m_Parent;
    protected RTSEntity m_Target;

    public float Damage;
    public float Range;
    public float FireRate;
    public float TurretSpeed;

    public bool isRanged;
    public bool isAntiArmor;
    public bool isAntiStructure;

    public abstract Vector3 CurrentLocation { get; }
    
    public abstract Vector3 TargetLocation { get; }

    public abstract void AssignDetails(Weapon weapon);

    public abstract void Attack(RTSEntity obj);

    public abstract void Stop();

}
