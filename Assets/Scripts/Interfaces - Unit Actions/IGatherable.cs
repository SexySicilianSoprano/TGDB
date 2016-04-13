using UnityEngine;
using System.Collections;

public interface IGatherable
{
    void Gather (ResourceMine mine);

    void Stop();
}
