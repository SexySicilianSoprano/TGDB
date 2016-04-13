using UnityEngine;
using System.Collections;

public static class Const {

	public const int TEAM_STEAMHOUSE = 0;

    public const int PLAYER_1 = 0;
    public const int PLAYER_2 = 1;

    public const int ORDER_STOP = 0;
	public const int ORDER_MOVE_TO = 1;
	public const int ORDER_ATTACK = 2;
	public const int ORDER_DEPLOY = 3;
    public const int ORDER_GATHER = 4;
	
	public const int BUILDING_FloatingFortress = 0;
    public const int BUILDING_NavalYard = 1;
    public const int BUILDING_Refinery = 2; 
    public const int BUILDING_Laboratory = 3;
    public const int BUILDING_TURRET = 4;

    public const int TYPE_Building = 0;
	public const int TYPE_Ship = 1;
	public const int TYPE_Science = 2;
	
	public const int MAINTENANCE_Nothing = 0;
	public const int MAINTENANCE_Sell = 1;
	public const int MAINTENANCE_Fix = 2;
	public const int MAINTENANCE_Disable = 3;
	
	public const int TILE_Open = 1;
	public const int TILE_Closed = 2;
	public const int TILE_Unvisited = 3;
	public const int TILE_Blocked = 4;
	public const int TILE_OnRoute = 6;
	
	public const int LAYEREDTILE_NotLayered = 0;
	public const int LAYEREDTILE_Bridge = 1;
	public const int LAYEREDTILE_Tunnel = 2;
	
	public const float ASTAR_Costinc = 10.0f;
	public const float ASTAR_CostDinc = 14.0f;
	
	public const int BLOCKINGLEVEL_Normal = 0;
	public const int BLOCKINGLEVEL_Occupied = 1;
	public const int BLOCKINGLEVEL_OccupiedStatic = 2;
	
	public const int DIRECTION_Right = 0;
	public const int DIRECTION_Down = 1;
	public const int DIRECTION_Left = 2;
	public const int DIRECTION_Up = 3;
}
