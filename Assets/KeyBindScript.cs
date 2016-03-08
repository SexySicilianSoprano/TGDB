﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class KeyBindScript : MonoBehaviour {

    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public Text up, down, left, right, jump, camLeft, camRight;

    private GameObject currentKey;

    private Color32 normal = new Color32(255, 255, 255, 255);
    private Color32 selected = new Color32(200, 200, 200, 225);

	// Use this for initialization
	/*void Start ()
    {
        GetKeys();
    }*/

    private void Start()
    {
        keys.Add("Up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "W")));
        keys.Add("Down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", "S")));
        keys.Add("Left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "A")));
        keys.Add("Right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D")));
        keys.Add("Jump", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump", "Space")));
        keys.Add("camLeft", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("camLeft", "Q")));
        keys.Add("camRight", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("camRight", "E")));
        
        /*keys.Add("Up", KeyCode.W);
        keys.Add("Down", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Jump", KeyCode.Space);
        keys.Add("camLeft", KeyCode.Q);
        keys.Add("camRight", KeyCode.E);*/

        up.text = keys["Up"].ToString();
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
        camLeft.text = keys["camLeft"].ToString();
        camRight.text = keys["camRight"].ToString();
    }

    // Update is called once per frame
    void Update () {
	    if (Input.GetKeyDown(keys["Up"]))
        {
            //Do a move action
            Debug.Log("Up");
        }
        if (Input.GetKeyDown(keys["Down"]))
        {
            //Do a move action
            Debug.Log("Down");
        }
        if (Input.GetKeyDown(keys["Left"]))
        {
            //Do a move action
            Debug.Log("Left");
        }
        if (Input.GetKeyDown(keys["Right"]))
        {
            //Do a move action
            Debug.Log("Right");
        }
        if (Input.GetKeyDown(keys["Jump"]))
        {
            //Do a move action
            Debug.Log("Jump");
        }
        if (Input.GetKeyDown(keys["camLeft"]))
        {
            //Do a move action
            Debug.Log("camLeft");
        }
        if (Input.GetKeyDown(keys["camRight"]))
        {
            //Do a move action
            Debug.Log("camRight");
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Tallennettu!");
            SaveKeys();
        }
    }

    void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey.GetComponent<Image>().color = normal;
                currentKey = null;
            }
        }
    }

    public void ChangeKey(GameObject clicked)
    {
        if (currentKey!= null)
        {
            currentKey.GetComponent<Image>().color = normal;
        }

        currentKey = clicked;
        currentKey.GetComponent<Image>().color = selected;
    }

    public void SaveKeys()
    {
        foreach (var key in keys)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }

        PlayerPrefs.Save();
        keys = null;
        //GetKeys();
    }
}
