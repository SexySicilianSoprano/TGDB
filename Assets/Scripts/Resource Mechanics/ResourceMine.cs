using UnityEngine;
using System.Collections;

public class ResourceMine : MonoBehaviour {

    public float resource;
    public int maxGatherers;

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
