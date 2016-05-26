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
    public GameObject selectedHealthBar;
    public Image bar;
    public Image selectedBar;
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
        newHealthBar.name = "HPBar("+ name +")";

        healthBar = (RectTransform)newHealthBar.transform;
        bar = newHealthBar.transform.Find("HPBG").transform.Find("HPGreen").GetComponent<Image>();

        objectToFollow = this.gameObject.transform;
        //InvokeRepeating ("decreaseHealth", 0f, 2f);

        RectTransform panelRectTransform = newHealthBar.GetComponent<RectTransform>();

        HideHealthBar();
        /*SizeDif = MyCamera.zoomMinLimit / MyCamera.;

        panelRectTransform.sizeDelta =  panelRectTransform.sizeDelta.y);*/
	}
	
    void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
        healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
        bar.fillAmount = cur_health / max_health;

        if (selectedBar != null)
        {
            selectedBar.fillAmount = cur_health / max_health;
        }
        //Debug.Log (bar.fillAmount);

        max_health = GetComponent<RTSEntity>().m_MaxHealth;
        cur_health = GetComponent<RTSEntity>().m_Health;

        if (GetComponent<Turret>())
        {
            if (cur_health < max_health)
            {
                ShowHealthBar();
            }
        }
    }

    void OnDestroy()
    {
        Destroy(bar);
        Destroy(newHealthBar);
        Destroy(selectedHealthBar);
        Destroy(selectedBar);
    }

    // Show health bar
    public void ShowHealthBar()
    {
        if (bar)
            bar.gameObject.SetActive(true);

        if (healthBar)
            healthBar.gameObject.SetActive(true);
    }

    // Hide healt bar
    public void HideHealthBar()
    {
        if (bar)
            bar.gameObject.SetActive(false);

        if (healthBar)
            healthBar.gameObject.SetActive(false);
    }
    
}
