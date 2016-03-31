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
    public GameObject newHealthBar;
    public Image bar;
    public float max_health;
    public float cur_health;
    public MyCamera MyCamera;

    public float SizeDif;

	// Use this for initialization
	void Start ()
    {
        //Set variables to components
        cur_health = GetComponent<RTSEntity>().m_Health;
        max_health = GetComponent<RTSEntity>().m_MaxHealth;
        canvasRectT = GameObject.Find("Healthbars").GetComponent<RectTransform>();
        objectToFollow = transform;
        newHealthBar = Instantiate(imgPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        newHealthBar.transform.SetParent(canvasRectT);
        newHealthBar.transform.position = new Vector3 (0, 0, 0);
        healthBar = (RectTransform)newHealthBar.transform;
        bar = newHealthBar.transform.Find("HPBG").transform.Find("HPGreen").GetComponent<Image>();
        objectToFollow = this.gameObject.transform;
        //InvokeRepeating ("decreaseHealth", 0f, 2f);

        RectTransform panelRectTransform = newHealthBar.GetComponent<RectTransform>();

        /*SizeDif = MyCamera.zoomMinLimit / MyCamera.;

        panelRectTransform.sizeDelta =  panelRectTransform.sizeDelta.y);*/
	}
	
    void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
        healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
        bar.fillAmount = cur_health / max_health;
        //Debug.Log (bar.fillAmount);

        max_health = GetComponent<RTSEntity>().m_MaxHealth;
        cur_health = GetComponent<RTSEntity>().m_Health;

        if (gameObject.layer == 9) 
        {
            //bar.gameObject.SetActive(false);
            if (GetComponent<RTSEntity>().m_Health < GetComponent<RTSEntity>().m_MaxHealth)
            {
                bar.gameObject.SetActive(true);
            }
        }
    }

    void OnDestroy()
    {
        Destroy(bar);
        Destroy(newHealthBar);
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
