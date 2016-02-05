using UnityEngine;
using System.Collections;

public class NewCursor : MonoBehaviour {

	public Texture2D cursor;
	
	// Update is called once per frame
	void OnMouseEnter () {
		Cursor.SetCursor (cursor, Vector2.zero, CursorMode.Auto);
	}

	void OnMouseExit() {
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}
