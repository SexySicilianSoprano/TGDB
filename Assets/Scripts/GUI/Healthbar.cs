using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(RTSEntity))]
public class Healthbar : MonoBehaviour
{

    public RectTransform canvasRectT;
    public RectTransform healthBar;
    public Slider healthBarSlider;
    public Transform objectToFollow;
    public GameObject ImagePrefab;
    public float currentHealth;
    public float maxHealth;

    void Start()
    {
        currentHealth = GetComponent<RTSEntity>().m_Health;
        maxHealth = GetComponent<RTSEntity>().m_MaxHealth;
        GameObject newHealthSlider = Instantiate(ImagePrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
        newHealthSlider.transform.SetParent(canvasRectT, true);
        newHealthSlider.transform.position = new Vector3(0, 0, 0);
        healthBar = (RectTransform)newHealthSlider.transform;
        healthBarSlider = newHealthSlider.gameObject.GetComponent<Slider>();
        objectToFollow = this.gameObject.transform;
    }

    void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
        healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
        healthBarSlider.value = GetComponent<RTSEntity>().m_Health;
        healthBarSlider.maxValue = GetComponent<RTSEntity>().m_MaxHealth;

        if (gameObject.layer == 9)
        {
            healthBarSlider.gameObject.SetActive(false);
            if (GetComponent<RTSEntity>().m_Health < GetComponent<RTSEntity>().m_MaxHealth)
            {
                healthBarSlider.gameObject.SetActive(true);
            }
        }
    }

}