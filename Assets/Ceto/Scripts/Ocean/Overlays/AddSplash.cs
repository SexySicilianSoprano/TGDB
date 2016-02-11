using UnityEngine;
using System.Collections;

namespace Ceto
{

	//[AddComponentMenu("Ceto/AddSplash")]
	[DisallowMultipleComponent]
	public class AddSplash : MonoBehaviour, ISplash
	{

		public string ID { get { return name; } }

		public bool Kill { get { return m_kill; } }
		bool m_kill;

		public GameObject ThisGameObject { get { return gameObject; } }

		public OverlayFoamTexture foamTexture;

		public AnimationCurve timeLine = DefaultCurve();

		public float size = 10.0f;

		public float rotaion = 0.0f;

		public float duration = 10.0f;
		
		[Range(0.0f, 1.0f)]
		public float alpha = 0.8f;
		
		WaveOverlay m_overlay;

		void Start () 
		{

			Vector3 halfSize = new Vector2(size * 0.5f, size * 0.5f);
			
			m_overlay = new WaveOverlay(transform.position, rotaion, halfSize, duration);
	
			if(Ocean.Instance != null)
				Ocean.Instance.OverlayManager.Add(m_overlay);
			
		}

		static AnimationCurve DefaultCurve()
		{
			
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0.0f, 0.0f),
				new Keyframe(0.012f, 0.98f),
				new Keyframe(0.026f, 1.0f),
				new Keyframe(1.0f, 0.0f)
			};
			
			return new AnimationCurve(keys);
		}
		
		void Update () 
		{
			if(m_overlay.Age >= m_overlay.Duration)
			{
				m_overlay.Kill = true;
			}
			else
			{

				m_overlay.FoamTex = foamTexture;
				m_overlay.Position = transform.position;
				m_overlay.HalfSize = new Vector2(size * 0.5f, size * 0.5f);

				float a = timeLine.Evaluate(m_overlay.NormalizedAge);
				m_overlay.FoamTex.alpha = a * alpha;

				m_overlay.UpdateOverlay();
			}
			
		}
		
		void OnEnable()
		{
			if(m_overlay != null)
				m_overlay.Hide = false;
		}
		
		void OnDisable()
		{
			if(m_overlay != null)
				m_overlay.Hide = true;
		}

		void OnDestroy()
		{

			if(m_overlay != null)
				m_overlay.Kill = true;

			m_kill = true;

		}
		
		void OnDrawGizmos() 
		{
			if(!enabled) return;

			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
		}
	}
	
}







