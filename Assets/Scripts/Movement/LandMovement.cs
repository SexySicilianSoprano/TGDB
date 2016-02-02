using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LandMovement : Movement {

    //protected GetPathThread m_GetPathThread;

    private List<Vector3> m_Path;
    private object m_PathLock = new object();
    private object m_PathChangedLock = new object();

    private bool m_PathChanged = false;

    public event PathChangedDelegate PathChangedEvent;

    //This variable needs to be locked as it can be accessed from multiple threads
    protected List<Vector3> Path
    {
        get
        {
            List<Vector3> tempValue;
            lock (m_PathLock)
            {
                tempValue = m_Path;
            }
            return tempValue;
        }
        set
        {
            lock (m_PathLock)
            {
                m_Path = value;
            }

            //Set path changed to true, this is so the UI thread can pick up a change and carry on execution
            PathChanged = true;
        }
    }

    //This variable needs to be locked as it can be accessed from multiple threads
    private bool PathChanged
    {
        get
        {
            bool tempValue;
            lock (m_PathChangedLock)
            {
                tempValue = m_PathChanged;
            }
            return tempValue;
        }
        set

        {
            lock (m_PathChangedLock)
            {
                m_PathChanged = value;
            }
        }
    }


    public void SetPath(List<Vector3> path)
    {
        Path = path;
    }

    public delegate void PathChangedDelegate();

    protected void Update() { }
}
