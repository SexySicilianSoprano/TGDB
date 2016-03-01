using UnityEngine;
using System.Collections;
using System;

public interface ICursorManager 
{
	void UpdateCursorIcon(InteractionState state);
	void HideCursor();
	void ShowCursor();
}
