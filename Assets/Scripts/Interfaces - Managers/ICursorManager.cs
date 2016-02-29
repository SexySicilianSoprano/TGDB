using UnityEngine;
using System.Collections;

public interface ICursorManager 
{
	void UpdateCursorIcon(InteractionState state);
	void HideCursor();
	void ShowCursor();
}
