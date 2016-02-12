using UnityEngine;
using System.Collections;

public interface ICursorManager 
{
	void UpdateCursorIcon(InteractionState interactionState);
	void HideCursor();
	void ShowCursor();
}
