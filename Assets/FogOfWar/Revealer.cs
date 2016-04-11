using UnityEngine;
using System.Collections;

public class Revealer : MonoBehaviour {

    public int radius;

	// Use this for initialization
	void Start () {
        FogOfWarManager.Instance.RegisterRevealer(this);
	}
}
