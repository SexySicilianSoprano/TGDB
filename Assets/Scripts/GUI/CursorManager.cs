using UnityEngine;
using System.Collections;
using System;
/*
    This manager controls how the cursor changes depending on UIManager's InteractionState.
    This way we can change the cursor icon whenever we're doing something different.
    
    - Karl Sartorisio
    The Great Deep Blue
*/

public class CursorManager : MonoBehaviour, ICursorManager {

    // Singleton
    public static CursorManager main;

    // Public variables, assign textures from Unity editor
	public Texture2D attack;
	public Texture2D select;
    public Texture2D move;
    public Texture2D normal;

    void Awake()
    {
        main = this;
    }
	
	// Updates the cursor icon depending what InteractionState is used with it
	public void UpdateCursorIcon (InteractionState state)
    {
        switch (state)
        {
            case InteractionState.Attack:
                Cursor.SetCursor(attack, Vector2.zero, CursorMode.Auto);
                break;
            case InteractionState.Select:
                Cursor.SetCursor(select, Vector2.zero, CursorMode.Auto);
                break;
            case InteractionState.Move:
                Cursor.SetCursor(move, Vector2.zero, CursorMode.Auto);
                break;
            case InteractionState.Invalid:
            case InteractionState.Interact:
            case InteractionState.Nothing:
            case InteractionState.Gather:
                Cursor.SetCursor(normal, Vector2.zero, CursorMode.Auto);
                break;
        }
	}

    // Hides the cursor
    public void HideCursor()
    {
        Cursor.visible = false;
    }

    // Shows the cursor
    public void ShowCursor()
    {
        Cursor.visible = true;
    }
}
