using UnityEngine;
using System.Collections;

public class ResolverConfig : MonoBehaviour {

	void Awake () 
	{
		ManagerResolver.Register<ISelectedManager>(SelectedManager.main);
		ManagerResolver.Register<IUIManager>(UIManager.main);
		ManagerResolver.Register<IManager>(Manager.main);
		ManagerResolver.Register<ICursorManager>(CursorManager.main);
        ManagerResolver.Register<IGameManager>(GameManager.main);
	}
}
