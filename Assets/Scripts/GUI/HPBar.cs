using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// HPBar. This code determines how the HP Bar above game objects behave. Still need some logic work, although the basics are there!
/// </summary>

[RequireComponent(typeof(RTSEntity))]
public class HPBar : MonoBehaviour {

    public RectTransform canvasRectT;
    public RectTransform healthBar;
    public Transform objectToFollow;
    public GameObject imgPrefab;
    public Image bar;
    public float max_health;
    public float cur_health;

	// Use this for initialization
	void Start () {
        cur_health = GetComponent<RTSEntity>().m_Health;
        max_health = GetComponent<RTSEntity>().m_MaxHealth;
        GameObject newHealthBar = Instantiate(imgPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
        newHealthBar.transform.SetParent(canvasRectT);
        newHealthBar.transform.position = new Vector3 (0, 0, 0);
        healthBar = (RectTransform)newHealthBar.transform;
        bar = newHealthBar.gameObject.GetComponent<Image>();
        objectToFollow = this.gameObject.transform;
        //InvokeRepeating ("decreaseHealth", 0f, 2f);
	}
	
    void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
        healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
        bar.fillAmount = GetComponent<RTSEntity>().GetHealthRatio();
        
        if (gameObject.layer == 9) 
        {
            bar.gameObject.SetActive(false);
            if (GetComponent<RTSEntity>().m_Health < GetComponent<RTSEntity>().m_MaxHealth)
            {
                bar.gameObject.SetActive(true);
            }
        }
    }

	/*void decreaseHealth () {
        cur_health -= 5f;
        float calc_health = cur_health / max_health;
        SetHealth(calc_health);
	}

    void SetHealth (float myHealth)
    {
        bar.fillAmount = myHealth;
    }*/
}
