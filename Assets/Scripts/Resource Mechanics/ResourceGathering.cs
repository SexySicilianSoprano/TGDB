using UnityEngine;
using System.Collections;

public class ResourceGathering : MonoBehaviour {

    // ##### Private variables #####
    // Private booleans
    private bool targetSet = false; // Do we have a mine?
    private bool readyToGather = true; // Are we ready to gather?
    private bool onMyWay = false;

    // Private numeric variables
    private float gatheringRate; // How much time between batches?
    private float maxAmount; // How much do we gather per batch?

    // Targeted mine
    private ResourceMine targetMine; // The targeted mine
    private Vector3 minePosition;

    // Manager accessors
    private Manager m_Manager
    {
        get { return GameObject.Find("Manager").GetComponent<Manager>(); }
    }

	// Use this for initialization
	void Start ()
    {
        SetValues();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // Behaviour query
        if (targetSet && targetMine == null)
        {
            Stop();
        }
        else if (targetSet && readyToGather)
        {
            Gather(targetMine);
        }
	}

    // Gathering command
    public void Gather(ResourceMine mine)
    {
        targetMine = mine;
        targetSet = true;

        // Do we have a target mine?
        if (targetMine)
        {
            minePosition = targetMine.transform.position;
            // Are we close enough to gather?
            if (CloseEnough(minePosition))
            {
                // Make sure we stay in place while gathering
                GetComponent<BoatMovement>().stayInPlace = true;

                if (onMyWay)
                {
                    GetComponent<BoatMovement>().Stop();
                    onMyWay = false;
                }

                if (readyToGather)
                {
                    // Gather!
                    TakeResources();
                }

                return;
            }
            else
            {
                // Give a move order towards the mine
                if (!onMyWay)
                GoTowardsTarget(minePosition);
                return;
            }
        }
        else
        {
            // Just stop, ain't gotta fool around no mo
            Stop();
            return;
        }
    }

    // Take resources from the mine
    public void TakeResources()
    {
        // Take what is yours and send it to Manager to be used
        float gathered = targetMine.TakeResources(maxAmount);
        m_Manager.AddResource(gathered);

        // Let us wait till we can gather again
        readyToGather = false;
        StartCoroutine(WaitAndGather());
        
    }

    public void Stop()
    {
        targetMine = null;
        targetSet = false;
        onMyWay = false;
        GetComponent<BoatMovement>().stayInPlace = false;
    }

    public void SetValues()
    {
        gatheringRate = 5;
        maxAmount = 100;
    }

    private void GoTowardsTarget(Vector3 target)
    {
        onMyWay = true;
        GetComponent<Movement>().MoveTo(target);
    }

    private bool CloseEnough(Vector3 target)
    {
        if (Vector3.Distance(target, transform.position) <= 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Waits for set amount of seconds to allow gathering again
    IEnumerator WaitAndGather()
    {
        yield return new WaitForSeconds(gatheringRate);
        readyToGather = true; 
    }
}
