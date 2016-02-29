using UnityEngine;
using System.Collections;

public interface IGameManager{

    Player primaryPlayer();
    Player enemyPlayer();

    void InitialiseGameData();

}
