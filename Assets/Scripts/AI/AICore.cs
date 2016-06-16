using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 
/// AICore
///  
/// This is the core component for AI scripts to work with. It holds lists, variables and other functions
/// every AI must use to have a basis for behaviour.
/// 
/// - Karl Sartorisio
/// 
/// </summary>

public class AICore : MonoBehaviour {

    // Internal threat count
    protected int internalThreat;

    // The base building, AI must have one known to protect it
    protected RTSEntity baseBuilding;
    protected RTSEntity enemyBase;

    // Building lists
    protected List<Building> totalBuildings = new List<Building>(); // Total count of buildings
    protected List<Building> turrets = new List<Building>(); // Count of active turrets
    protected List<Building> navalYards = new List<Building>(); // Count of Naval Yards
    
    // Unit lists
    protected List<Unit> totalUnits = new List<Unit>(); // Total count of units deployed
    protected List<Unit> availableUnits = new List<Unit>(); // Units that are not in squads

    // Squad lists
    protected List<Squad> totalSquads = new List<Squad>(); // Total count of squads
    protected List<Squad> killSquads = new List<Squad>(); // Count of kill squads
    protected List<Squad> defendSquads = new List<Squad>(); // Count of defensive squads
    protected List<Squad> patrolSquads = new List<Squad>(); // Count of patrol squads

    // Threat thresholds
    protected Threshold currentThreshold = Threshold.StandBy;
    public int lowThresholdLimit;
    public int highThresholdLimit;
    
	// Use this for initialization
	void Start ()
    {
        // If base building is not set, find one
        if (!baseBuilding)
        {
            RTSEntity[] getBases = FindObjectsOfType<FloatingFortress>();

            foreach (RTSEntity basebuild in getBases)
            {
                if (basebuild.tag == "Player2")
                {
                    baseBuilding = basebuild;
                    break;
                }
            }
        }

        // if enemy base is not set, find one
        if (!enemyBase)
        {
            RTSEntity[] getEnemyBase = FindObjectsOfType<FloatingFortress>();

            foreach (RTSEntity basebuild in getEnemyBase)
            {
                if (basebuild.tag == "Player1")
                {
                    enemyBase = basebuild;
                    break;
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // Calculates threshold based on threat and comparing it to thresholds.
    private void CalculateThreshold()
    {
        if (internalThreat >= lowThresholdLimit)
        {
            if (internalThreat >= highThresholdLimit)
            {
                if (baseBuilding.AttackingEnemy)
                {
                    currentThreshold = Threshold.BaseUA;
                }

                currentThreshold = Threshold.High;
                return;
            }

            currentThreshold = Threshold.Low;
            return;
        }
        else
        {
            currentThreshold = Threshold.StandBy;
            return;
        }
    }

    // Set threat level
    public void GetThreat(int value)
    {
        //Debug.Log("AICore Get Threat called, value: " + value);
        internalThreat = value;
        CalculateThreshold();
        //Debug.Log("Current threshold: " + currentThreshold);
    }

    
}

// Current threshold setter, will be used to determine AI actions
// Descriptions:
// - StandBy: No moves, build few units just in case
// - Low: Defensive moves, build a bit more units
// - High: Both defensive and attack moves, build more units
// - BaseUA: Base under attack, highest security mode, throw in as many units as possible 
public enum Threshold
{
    StandBy = 0,
    Low = 1,
    High = 2,
    BaseUA = 3
}
