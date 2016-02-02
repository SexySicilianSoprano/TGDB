using UnityEngine;
using System.Collections;

public class Player {

    public int ID { get; set; }

    public string screenName { get; set; }

    public Color color { get; set; }

    public int controlledLayer { get; set; }

    public string controlledTag { get; set; }

    public void AssignDetails(Player player)
    {
        ID = player.ID;
        screenName = player.screenName;
        color = player.color;
        controlledLayer = player.controlledLayer;
        controlledTag = player.controlledTag;
    }
    
}
