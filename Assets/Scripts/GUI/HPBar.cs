﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBar : MonoBehaviour {

    public RectTransform canvasRectT;
    public RectTransform healthBar;
    public Transform objectToFollow;
    public Image Bar;
    public float max_health = 100f;
    public float cur_health = 0f;

	// Use this for initialization
	void Start () {
        cur_health = max_health;
        InvokeRepeating ("decreaseHealth", 0f, 2f);
	}
	
    void Update()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);
        healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
    }

	// Update is called once per frame
	void decreaseHealth () {
        cur_health -= 5f;
        float calc_health = cur_health / max_health;
        SetHealth(calc_health);
	}

    void SetHealth (float myHealth)
    {
        Bar.fillAmount = myHealth;
    }
}
