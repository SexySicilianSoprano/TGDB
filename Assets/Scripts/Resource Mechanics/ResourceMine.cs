using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// Attach this component to pretty much whatever and you'll get yourself a nice mine for resources
/// Units with Resource Gathering component can gather resources. By default, this component's values
/// are 0, so upon creation you need to assign them.
/// 
/// - Karl Sartorisio
/// The Great Deep Blue
/// 
/// </summary>

public class ResourceMine : MonoBehaviour {

    // The amount of resources this mine holds
    public float resource;

    // How many gatheres can there be at once?
    public int maxGatherers;

    // Called in Resource Gathering script, returns either the amout requested, less than that or zero if no resources
    public float TakeResources(float amount)
    {
        if (resource > 0)
        {
            if (resource >= amount)
            {
                resource -= amount;
                return amount;
            }
            else
            {
                float newAmount = resource;
                resource = 0;
                return newAmount;
            }
        }
        else
        {
            return 0;
        }
    }
}
