using UnityEngine;
using System.Collections;

public static class SetPlayer {

    public static Player Player1 = new Player
    {
        ID = 0,
        screenName = "Mingebag",
        color = new Color(255, 0, 0),
        controlledLayer = 8,
        controlledTag = "Player1"
    };

    public static Player Player2 = new Player
    {
        ID = 1,
        screenName = "TestEnemy",
        color = new Color(0, 0, 255),
        controlledLayer = 9,
        controlledTag = "Player2"
    };
    
}
