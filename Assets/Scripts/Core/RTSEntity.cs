using UnityEngine;
using System.Collections;

public abstract class RTSEntity : MonoBehaviour {

	//The base class for all playable objects
	
	public string Name
	{
		get;
		private set;
	}
	
	public int ID
	{
		get;
		private set;
	}
	
	public int UniqueID
	{
		get;
		private set;
	}
	
	public int TeamIdentifier
	{
        get;
        private set;
	}
    
    public int playerLayer
    {
        get
        {
            return gameObject.layer;
        }
    }
    public string playerTag
    {
        get
        {
            return gameObject.tag;
        }
    }

    public Color color
    {
        get;
        private set;
    }

    public Weapon Weapon {
        get;
        private set;
    }

    public GameObject Explosion
    {
        get;
        private set;
    }

    public RTSEntity AttackingEnemy;

    // Health details
    public float m_Health;
	public float m_MaxHealth;	

    // Action voids
	public abstract void SetSelected();
	public abstract void SetDeselected();
	public abstract void AssignToGroup(int groupNumber);
	public abstract void RemoveFromGroup();
	public abstract void ChangeTeams(int team);

    // Weapon details
    private float m_Damage;
    private float m_Range;
    private float m_FireRate;
	
	public float GetHealthRatio()
	{
		return m_Health/m_MaxHealth;
	}
	
    	
	protected void AssignDetails(Item item)
	{
		Name = item.Name;
		ID = item.ID;
		TeamIdentifier = item.TeamIdentifier;
		m_MaxHealth = item.Health;
		m_Health = m_MaxHealth;
        Explosion = item.Explosion;
	}

   	public void TakeDamage(float damage)
	{
        
        
	}
     
    protected void OnDestroy() {
        
    }
    
}
