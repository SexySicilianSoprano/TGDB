using UnityEngine;
using System.Collections;

namespace Ceto
{

	public interface ISplash 
	{

		string ID { get; }

		bool Kill { get; }

		GameObject ThisGameObject { get; }

	}

}
