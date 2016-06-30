using UnityEngine;
using System.Collections;
using System;

public class Item {
	
	//Item Member Variables and Properties-----------------------------------
	public int ID;

	public string Name;
	public float Health;
	public float Armour;
	
	public float Speed = 0;
	public float RotationSpeed = 0;
	public float Acceleration = 0;
	
	public GameObject Prefab;
	
	public int Cost;
	public float BuildTime;
	
	public int TypeIdentifier;
	public int TeamIdentifier;
    public int BuildingIdentifier;
    public int PlayerIdentifier;
    public int UnitType;

	private GameObject m_Button;
	public GameObject Button;

	private Sprite m_ItemImage;
    public Sprite ItemImage;
	
	public Texture2D ItemImageHover
	{
		get;
		private set;
	}
	
	public int SortOrder;
	public int[] RequiredBuildings;
	public GameObject Explosion;
	public Type ObjectType;
	
	//-------------------------------------------------------------
	
	//Item building variables------------------------------------------
	
	//Constructors ---------------------------------------------------
	public Item()
	{
		
	}
	
	public Item(Item item)
	{
		ID = item.ID;
		Name = item.Name;
		Health = item.Health;
		Armour = item.Armour;
		Speed = item.Speed;
		RotationSpeed = item.RotationSpeed;
		Prefab = item.Prefab;
		Cost = item.Cost;
		BuildTime = item.BuildTime;
		TypeIdentifier = item.TypeIdentifier;
		TeamIdentifier = item.TeamIdentifier;
        PlayerIdentifier = item.PlayerIdentifier;
        BuildingIdentifier = item.BuildingIdentifier;
		m_ItemImage = item.ItemImage;
		m_Button = item.Button;
		ItemImageHover = item.ItemImageHover;
		SortOrder = item.SortOrder;
		RequiredBuildings = item.RequiredBuildings;
		Explosion = item.Explosion;
		ObjectType = item.ObjectType;
	}
	//--------------------------------------------------------------------------
	
	public void Initialise()
	{

	}
	
	//Methods------------------------------------------------------
	/*private void CreateHoverImage()
	{
		ItemImageHover = new Texture2D(m_ItemImage.width, m_ItemImage.height, TextureFormat.RGB24, false);
		
		for (int i=0; i<m_ItemImage.width; i++)
		{
			for (int j=0; j<m_ItemImage.height; j++)
			{
				Color pixelColor = ItemImage.GetPixel (i, j);
				pixelColor.r += 0.2f;
				pixelColor.g += 0.2f;
				pixelColor.b += 0.2f;
				ItemImageHover.SetPixel (i, j, pixelColor);
			}
		}
		
		ItemImageHover.Apply ();
	}*/
	
	public void Update(float frameTime)
	{

	}
	
	public Item DeepClone()
	{
		return new Item(this);
	}	
	
}
