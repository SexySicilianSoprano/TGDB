using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ItemDB {
		
	public static List<Item> AllItems = new List<Item>();

    // ##### STEAM HOUSE BUILDINGS #####
    
    public static Item Scout = new Item
    {
        ID = 0,
        TypeIdentifier = Const.TYPE_Ship,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        Name = "Scout",
        Health = 40.0f,
        Armour = 1.0f,
        Speed = 40.0f,
        RotationSpeed = 4.0f,
        Acceleration = 5.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_3", typeof(GameObject)) as GameObject,
        Prefab = Resources.Load("Models/Units/SteamHouse/Scout/Scout", typeof(GameObject)) as GameObject,
		ItemImage = Resources.Load("Item Images/SteamHouse/ScoutBoat_Icon", typeof(Sprite)) as Sprite,
        SortOrder = 0,
        RequiredBuildings = new int[] { 1 },
        Cost = 200,
        BuildTime = 3.0f,
    };
     
    public static Item Destroyer = new Item
    {
        ID = 1,
        TypeIdentifier = Const.TYPE_Ship,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        Name = "Destroyer",
        Health = 100.0f,
        Armour = 3.0f,
        Speed = 30.0f,
        RotationSpeed = 1.5f,
        Acceleration = 1.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_3", typeof(GameObject)) as GameObject,
        Prefab = Resources.Load("Models/Units/SteamHouse/Destroyer/Destroyer", typeof(GameObject)) as GameObject,
		ItemImage = Resources.Load("Item Images/SteamHouse/Destroyer_Icon", typeof(Sprite)) as Sprite,
        SortOrder = 1,
        RequiredBuildings = new int[] { 1 },
        Cost = 450,
        BuildTime = 5.0f,
    };

    public static Item FishingBoat = new Item
    {
        ID = 2,
        TypeIdentifier = Const.TYPE_Ship,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        Name = "FishingBoat",
        Health = 100.0f,
        Armour = 3.0f,
        Speed = 20.0f,
        RotationSpeed = 2.5f,
        Acceleration = 3.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_3", typeof(GameObject)) as GameObject,
        Prefab = Resources.Load("Models/Units/SteamHouse/FishingBoat/FishingBoat", typeof(GameObject)) as GameObject,
        ItemImage = Resources.Load("Item Images/SteamHouse/FishingBoat_Icon", typeof(Sprite)) as Sprite,
        SortOrder = 1,
        RequiredBuildings = new int[] { 1 },
        Cost = 675,
        BuildTime = 7.0f,
    };

    public static Item Dreadnought = new Item
    {
        ID = 3,
        TypeIdentifier = Const.TYPE_Ship,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        Name = "Dreadnought",
        Health = 100.0f,
        Armour = 3.0f,
        Speed = 20.0f,
        RotationSpeed = 1.5f,
        Acceleration = 1.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_3", typeof(GameObject)) as GameObject,
        Prefab = Resources.Load("Models/Units/SteamHouse/Dreadnought/Dreadnought", typeof(GameObject)) as GameObject,
        ItemImage = Resources.Load("Item Images/SteamHouse/Dreadnought_Icon", typeof(Sprite)) as Sprite,
        SortOrder = 1,
        RequiredBuildings = new int[] { 1 },
        Cost = 300,
        BuildTime = 5.0f,
    };


    // ##### STEAM HOUSE BUILDINGS #####    

    public static Item FloatingFortress = new Item
	{
		ID = 4,
		TypeIdentifier = Const.TYPE_Building,
		TeamIdentifier = Const.TEAM_STEAMHOUSE,
        BuildingIdentifier = Const.BUILDING_FloatingFortress,
        Name = "Floating Fortress",
		Health = 500.0f,
		Armour = 10.0f,
		Explosion = Resources.Load("Effects/Prefabs/Explosion_4", typeof(GameObject)) as GameObject,
		Prefab = Resources.Load ("Models/Buildings/SteamHouse/Floating Fortress/FloatingFortress", typeof(GameObject)) as GameObject,
		SortOrder = 100,
		RequiredBuildings = new int[] { 7, 6, 100 },
		Cost = 700,
		BuildTime = 10.0f,
		ObjectType = typeof(FloatingFortress),
	};
	
	public static Item NavalYard = new Item
	{
		ID = 5,
		TypeIdentifier = Const.TYPE_Building,
		TeamIdentifier = Const.TEAM_STEAMHOUSE,
        BuildingIdentifier = Const.BUILDING_NavalYard,
		Name = "Naval Yard",
		Health = 100.0f,
		Armour = 3.0f,
		Explosion = Resources.Load("Effects/Prefabs/Explosion_4") as GameObject,
		Prefab = Resources.Load ("Models/Buildings/SteamHouse/Naval Yard/NavalYard", typeof(GameObject)) as GameObject,
        ItemImage = Resources.Load ("Item Images/SteamHouse/NavalYard_Icon", typeof(Sprite)) as Sprite,
		SortOrder = 0,
		RequiredBuildings = new int[] { 0 },
		Cost = 700,
		BuildTime = 5.0f,
		ObjectType = typeof(NavalYard),
	};

    public static Item Refinery = new Item
    {
        ID = 6,
        TypeIdentifier = Const.TYPE_Building,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        BuildingIdentifier = Const.BUILDING_Refinery,
        Name = "Refinery",
        Health = 100.0f,
        Armour = 3.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_4") as GameObject,
        Prefab = Resources.Load("Models/Buildings/SteamHouse/Refinery/Refinery", typeof(GameObject)) as GameObject,
        ItemImage = Resources.Load("Item Images/SteamHouse/Refinery_Icon", typeof(Sprite)) as Sprite,
        SortOrder = 0,
        RequiredBuildings = new int[] { 0 },
        Cost = 700,
        BuildTime = 5.0f,
        ObjectType = typeof(Refinery),
    };
     
    public static Item Laboratory = new Item
    {
        ID = 7,
        TypeIdentifier = Const.TYPE_Building,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        BuildingIdentifier = Const.BUILDING_Laboratory,
        Name = "Laboratory",
        Health = 100.0f,
        Armour = 3.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_4") as GameObject,
        Prefab = Resources.Load("Models/Buildings/SteamHouse/Laboratory/Laboratory", typeof(GameObject)) as GameObject,
        ItemImage = Resources.Load("Item Images/SteamHouse/Laboratory_Icon", typeof(Sprite)) as Sprite,
        SortOrder = 0,
        RequiredBuildings = new int[] { 0 },
        Cost = 700,
        BuildTime = 5.0f,
        ObjectType = typeof(Laboratory),
    };

    // TEST TURRET
    public static Item Turret = new Item
    {
        ID = 100,
        TypeIdentifier = Const.TYPE_Building,
        TeamIdentifier = Const.TEAM_STEAMHOUSE,
        BuildingIdentifier = Const.BUILDING_TURRET,
        Name = "Defense Turret",
        Health = 200f,
        Armour = 3.0f,
        Explosion = Resources.Load("Effects/Prefabs/Explosion_4") as GameObject,
        Prefab = Resources.Load("Models/Buildings/SteamHouse/Turret/Turret", typeof(GameObject)) as GameObject,
        ObjectType = typeof(Turret),
    };

    // ### Functions ###

    // Initialise items
    public static void Initialise()
	{
        // House of Gears items
        InitialiseItem (Scout);
        InitialiseItem (Destroyer);
		InitialiseItem (FishingBoat);
		InitialiseItem (Dreadnought);
        InitialiseItem (FloatingFortress);
		InitialiseItem (NavalYard);
        InitialiseItem (Refinery);
        InitialiseItem (Laboratory);
        InitialiseItem (Turret);
	}
	
    // Initialises item and adds it to AllItems
	private static void InitialiseItem(Item item)
	{
		item.Initialise ();
		AllItems.Add (item);
	}   
    
	public static List<Item> GetAvailableItems(int ID, List<Building> CurrentBuildings)
	{
		List<Item> valueToReturn = AllItems.FindAll(x => 
		{
			if (x.RequiredBuildings.Length == 1)
			{
				if (x.RequiredBuildings[0] == ID)
				{
					return true;
				}
			}
			else
			{
				bool otherBuildingsPresent = true;
				
				//Does this item require the added building ID?
				if (x.RequiredBuildings.Contains (ID))
				{
					//If so do we have the other required ID's?
					foreach (int id in x.RequiredBuildings)
					{
						if (id != ID && CurrentBuildings.FirstOrDefault(building => building.ID == id) == null)
						{
							otherBuildingsPresent = false;
							break;
						}
					}
				}
				else
				{
					otherBuildingsPresent = false;
				}	
                			
				return otherBuildingsPresent;
			}
						
			return false;
		});
		
		return valueToReturn;
	}
}
