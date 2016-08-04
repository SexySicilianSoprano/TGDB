using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 
/// ORIGINAL NOTES ----
/// Class that handle Inputs.
/// This class use custom inputs configured at Unity inspector.
/// </summary>
/// ######################################################
/// Author: Luigi Garcia
/// - Email: mr.garcialuigi@gmail.com
/// - Linkedin: http://br.linkedin.com/in/garcialuigi
/// - Github:  https://github.com/garcialuigi
/// - Facebook: https://www.facebook.com/mr.garcialuigi
/// ######################################################
/// ---- END OF ORIGINAL NOTES
/// 
/// 
/// 
/// As a side note, I've taken liberty to edit this code heavily, by removing inspector keybinds and merging keybindscript into this script. Much better :) All this code needs is pref implementing.
/// - Laura V.
public class InputManager : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public List<string> ReservedKeys = new List<string>();

    public Text up, down, left, right, jump, camLeft, camRight, zoomIn, zoomOut;

    private GameObject currentKey;

    private Color32 normal = new Color32(255, 255, 255, 255);
    private Color32 selected = new Color32(131, 200, 218, 225);


    //This is old part of the code

    // this must be configured by inspector

    /*public KeyCode upArrow;
	public KeyCode downArrow;
	public KeyCode leftArrow;
	public KeyCode rightArrow;
	public KeyCode rotateAroundLeft;
	public KeyCode rotateAroundRight;
	public KeyCode zoomIn;
	public KeyCode zoomOut;
	public KeyCode jumpBackToPlayer;*/

    public static InputManager instance; // instance reference
	private Vector2 panAxis = Vector2.zero;
	
	void Awake()
	{
		instance = this; // instance reference
	}

    void Start()
    {
        DefaultKeys();
    }

    //Supposed to set key values back to default ones, but still need to figure out how.
    public void DefaultKeys()
    {
        keys.Add("Up", KeyCode.W);
        keys.Add("Down", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Jump", KeyCode.Space);
        keys.Add("camLeft", KeyCode.Q);
        keys.Add("camRight", KeyCode.E);
        keys.Add("zoomIn", KeyCode.R);
        keys.Add("zoomOut", KeyCode.F);

        up.text = keys["Up"].ToString();
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
        camLeft.text = keys["camLeft"].ToString();
        camRight.text = keys["camRight"].ToString();
        zoomIn.text = keys["zoomIn"].ToString();
        zoomOut.text = keys["zoomOut"].ToString();

    }

    void Update()
	{
		UpdatePanAxis();
	}
	
	private void UpdatePanAxis()
	{
		panAxis = Vector2.zero;

        if (Input.GetKey(keys["Up"]))
        {
            //Do a move action
            //Debug.Log("Up");
            panAxis.y = 1;
        }
		else if (Input.GetKey(keys["Down"]))
        {
            //Do a move action
            //Debug.Log("Down");
            panAxis.y = -1;
		}
        if (Input.GetKey(keys["Right"]))
        {
            //Do a move action
            //Debug.Log("Right");
            panAxis.x = 1;
        }
        else if (Input.GetKey(keys["Left"]))
        {
            //Do a move action
            //Debug.Log("Left");
            panAxis.x = -1;
        }
	}
	
	public Vector2 GetPanAxis()
	{
		return panAxis;
	}
	
	public bool GetRotateAroundLeft()
	{
        //Debug.Log("CamLeft");
		return Input.GetKey(keys["camLeft"]);
	}
	
	public bool GetRotateAroundRight()
	{
        //Debug.Log("CamRight");
		return Input.GetKey(keys["camRight"]);
	}
	
	public float GetZoomInputAxis()
	{
		float value = 0;
		
		if (Input.GetKey(keys["zoomOut"]))
		{
			value = -0.3f;
		}
		else if (Input.GetKey(keys["zoomIn"]))
		{
			value = 0.3f;
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
            value = -1;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
            value = 1;
		}
		
		return value;
	}
	
	public bool GetJumpBackToPlayer()
	{
		return Input.GetKey(keys["Jump"]);
	}

    public void ReservedKeysList()
    {
        ReservedKeys.Clear();
        ReservedKeys.Add(up.text);
        ReservedKeys.Add(down.text);
        ReservedKeys.Add(left.text);
        ReservedKeys.Add(right.text);
        ReservedKeys.Add(jump.text);
        ReservedKeys.Add(camLeft.text);
        ReservedKeys.Add(camRight.text);
    }

    void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (CheckForAvailability(e.keyCode.ToString()) || currentKey.transform.Find("Text").GetComponent<Text>().text == e.keyCode.ToString())
                {
                    keys[currentKey.name] = e.keyCode;
                    currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                    currentKey.GetComponent<Image>().color = normal;
                    currentKey = null;

                    ReservedKeysList();
                }

                else
                {
                    currentKey.GetComponent<Image>().color = normal;
                    currentKey = null;
                }

            }

            if (e.isMouse)
            {
                switch (e.button)
                {
                    case 0:
                        if (CheckForAvailability(KeyCode.Mouse0.ToString()))
                        {
                            currentKey.transform.Find("Text").GetComponent<Text>().text = "Mouse0";
                            keys[currentKey.name] = KeyCode.Mouse0;
                        }
                        break;

                    case 1:
                        if (CheckForAvailability(KeyCode.Mouse1.ToString()))
                        {
                            currentKey.transform.Find("Text").GetComponent<Text>().text = "Mouse1";
                            keys[currentKey.name] = KeyCode.Mouse1;
                        }
                        break;
                    case 2:
                        if (CheckForAvailability(KeyCode.Mouse2.ToString()))
                        {
                            currentKey.transform.Find("Text").GetComponent<Text>().text = "Mouse2";
                            keys[currentKey.name] = KeyCode.Mouse2;
                        }
                        break;
                }

                ReservedKeysList();

                currentKey.GetComponent<Image>().color = normal;
                currentKey = null;
            }

        }
    }

    public void ChangeKey(GameObject clicked)
    {
        if (currentKey != null)
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

    public bool CheckForAvailability(string key)
    {
        foreach (string s in ReservedKeys)
        {
            if (key == s)
            {
                return false;
            }
        }

        return true;
    }

}