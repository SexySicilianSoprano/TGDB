using UnityEngine;
using System.Collections.Generic;

#pragma warning disable 219

namespace Ceto
{

    /// <summary>
    /// Base class for the add overlay scripts.
    /// Provides some commonly used functions.
    /// Both a single overlay and a list of overlays
    /// is provided and its then up to the parent class
    /// to decide which one to use by creating a instance.
    /// </summary>
    public abstract class AddWaveOverlayBase : MonoBehaviour
    {

        /// <summary>
        /// A list of all the overlays created.
        /// For components that handle multiple 
        /// overlays.
        /// </summary>
        protected IList<WaveOverlay> m_overlays = new List<WaveOverlay>();

        protected virtual void Start()
        {

        }

        /// <summary>
        /// Each overlay needs to be updated each frame.
        /// TODO - only updates overlays that have changed.
        /// </summary>
        protected virtual void Update()
        {

            if (m_overlays != null)
            {
                //unhide all the overlays on enable.
                var e = m_overlays.GetEnumerator();
                while(e.MoveNext())
                {
                    e.Current.UpdateOverlay();
                }
            }

        }

        /// <summary>
        /// On enable unhide all the overlays.
        /// </summary>
        protected virtual void OnEnable()
        {

            if (m_overlays != null)
            {
                //unhide all the overlays on enable.
                var e = m_overlays.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Hide = false;
                }
            }
        }

        /// <summary>
        /// On disable hide all the overlays.
        /// </summary>
        protected virtual void OnDisable()
        {

            if (m_overlays != null)
            {
                //unhide all the overlays on enable.
                var e = m_overlays.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Hide = true;
                }
            }
        }

        /// <summary>
        /// On destroy kill all the overlays.
        /// </summary>
        protected virtual void OnDestroy()
        {

            if (m_overlays != null)
            {
                //unhide all the overlays on enable.
                var e = m_overlays.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Kill = true;
                }
            }
        }

        /// <summary>
        /// The default curve for the time line.
        /// </summary>
        protected static AnimationCurve DefaultCurve()
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

        /// <summary>
        /// For textures that require their contents to be
        /// sampled check to see if read/write is enabled.
        /// </summary>
        protected void CheckCanSampleTex(Texture tex, string name)
        {

            if (tex == null) return;

            if (!(tex is Texture2D))
            {
                Ocean.LogWarning("Can not query overlays " + name + " if texture is not Texture2D");
                return;
            }

            Texture2D t = tex as Texture2D;

            //Is there a better way to do this?
            try
            {
                Color c = t.GetPixel(0, 0);
            }
            catch
            {
                Ocean.LogWarning("Can not query overlays " + name + " if read/write is not enabled");
            }
        }

    }

}







