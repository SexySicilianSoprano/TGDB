using UnityEngine;
using System.Collections.Generic;

using Ceto.Common.Unity.Utility;

namespace Ceto
{

	public class SplashManager
	{

		Dictionary<string, GameObject> m_splashs;

		GameObject m_root;

		public SplashManager(GameObject[] prefabs)
		{

			m_root = new GameObject();
			m_root.name = "Ceto Splashs";
			m_root.hideFlags = HideFlags.HideAndDontSave;

			m_splashs = new Dictionary<string, GameObject>();

			if(prefabs != null)
			{

                int count = prefabs.Length;
				for(int i = 0; i < count; i++)
				{

					if(prefabs[i] == null) continue;

					ISplash splash = ExtendedFind.GetInterface<ISplash>(prefabs[i]);

					if(splash == null) 
					{
						Debug.Log("<color=yellow>Ceto Warning: A splash prefab did not have a AddSplash component. Prefab not added.</color>");
						continue;
					}

					if(splash.ID == "")
					{
						Debug.Log("<color=yellow>Ceto Warning: A splash prefab did not have a id. Prefab not added.</color>");
						continue;
					}

					if(m_splashs.ContainsKey(splash.ID))
					{
						string name = splash.ID;
						Debug.Log("<color=yellow>Ceto Warning: A splash prefab had a name (" + name + ") that has already been used. Prefab not added.</color>");
						continue;
					}

					m_splashs.Add(splash.ID, prefabs[i]);
				}
			}

		}

		public void Update()
		{

			ISplash[] children = ExtendedFind.GetInterfacesInChildren<ISplash>(m_root);

			if(children == null) return;

            int count = children.Length;
			for(int i = 0; i < count; i++)
			{
				if(children[i] == null) continue;

				if(children[i].Kill)
					GameObject.Destroy(children[i].ThisGameObject);
			}
			
		}

		public bool CreateSplash(string id, Vector3 pos)
		{

			if(!m_splashs.ContainsKey(id)) return false;

			GameObject prefab = m_splashs[id];

			GameObject splash = (GameObject)GameObject.Instantiate(prefab, pos, prefab.transform.rotation);

			if(splash == null) return false;

			splash.hideFlags = HideFlags.HideAndDontSave;

			splash.transform.parent = m_root.transform;

			return true;
		}

	}

}













