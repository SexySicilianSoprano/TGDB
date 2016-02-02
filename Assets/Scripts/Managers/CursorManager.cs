using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour {

    //Singleton
    public static CursorManager main;

    //Variables
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public void UpdateCursor(InteractionState state)
    {
        switch (state)
        {
            case InteractionState.Attack:
                
                break;
            case InteractionState.Move:
                break;
            case InteractionState.Nothing:
                cursorTexture = Resources.Load("GUI/cursor2") as Texture2D;
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                break;

        }
    }

}
