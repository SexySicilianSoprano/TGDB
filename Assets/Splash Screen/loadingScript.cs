﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadingScript : MonoBehaviour {

    public float loadingTime;
    public Image loadingBar;
    public Text percentage;

	void Start () {
        loadingBar.fillAmount = 0;
	}
	
	// Update is called once per frame
	void Update () {
	    if (loadingBar.fillAmount <= 1.0f)
        {
            loadingBar.fillAmount += 1.0f / loadingTime * Time.deltaTime;
        }
        if (loadingBar.fillAmount == 1.0f)
        {
            SceneManager.LoadScene("MainMenu");
        }
        percentage.text = (loadingBar.fillAmount * 100).ToString ("f0");
	}
}
