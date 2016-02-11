using UnityEngine;
using System.Collections;

public class NewCursor : MonoBehaviour {

	public Texture2D attack;
	public Texture2D select;
	
	// Update is called once per frame
	void OnMouseEnter () {
		if (gameObject.tag == "Player2") {
			Cursor.SetCursor (attack, Vector2.zero, CursorMode.Auto);
		}
		else if (gameObject.tag == "Player1") {
			Cursor.SetCursor (select, Vector2.zero, CursorMode.Auto);
		}
	}

	void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}
