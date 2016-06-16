using UnityEngine;
using System.Collections;

/// <summary>
///  
/// Basic AI behaviour script
/// 
/// 
/// </summary>

public class BasicAI : AICore {

    // Behavior timer variables
    public float taskTimerInterval;
    private float timer = 0;

    // Use this for initialization
	void Start ()
    {
            
	}
	
	// Update is called once per frame
	void Update ()
    { 
        // Advance timer
        timer = AdvanceTimer(timer);

        // When timer reaches interval time, AI will do what it is supposed to do
        if (timer >= taskTimerInterval)
        {
            // Restart timer
            timer = 0;
            switch (currentThreshold)
            {
                case Threshold.StandBy:
                    StandByBehavior();
                    break;
                case Threshold.Low:
                    LowThreatBehaviour();
                    break;
                case Threshold.High:
                    HighThreatBehaviour();
                    break;
                case Threshold.BaseUA:
                    BaseUABehaviour();
                    break;
            }
        }
    }

    // StandBy behaviour, doesn't do moves and only builds if necessary. 25% resources in use.
    private void StandByBehavior() { }

    // Low threat behaviour, only does defensive moves and builds if necessary, 50% resources in use.
    private void LowThreatBehaviour() { }

    // High threat behaviour, does both defensive and attack moves, builds if necessary. 75% resources in use.
    private void HighThreatBehaviour() { }

    // Highest threat behaviour, furiously defends the base building with everything they've got. 100% resources in use.
    private void BaseUABehaviour() { }

    // Offensive move
    private void AttackMove(Squad squad, Vector3 target)
    {

    }

    // Defensive move
    private void DefensiveMove(Squad squad, Vector3 target)
    {

    }

    // Find closest turret to enemy and return a defend move position in defend move scenario
    private Vector3 GetClosestTurretPosition()
    {
        Vector3 target = new Vector3();
        return target;
    }

    // Find enemy attack move position in attack move scenario
    private Vector3 GetEnemyBaseAttackPosition()
    {
        Vector3 target = new Vector3();
        return target;
    }

    // Find the defend move position in BaseUA scenario
    private Vector3 GetOwnBaseDefendPosition()
    {
        Vector3 target = new Vector3();
        return target;
    }

    // Building gets destroyed, what do we do?
    public void BuildingDestroyed()
    {

    }

    // Advance behavior timer
    private float AdvanceTimer(float seconds)
    {
        float newTime = seconds;
        newTime += Time.deltaTime;
        return newTime;
    }
}
