using UnityEngine;
using System;
using System.Collections.Generic;

using Ceto.Common.Unity.Utility;

namespace Ceto
{
	/// <summary>
	/// Manages the overlay system.
	/// Responsible for rendering the overlays, 
	/// removing killed overlays and handling 
	/// querys in the overlays.
	/// 
	/// NOTE - currently rendered as individual quads. 
	/// This is slow and a batching system will be 
	/// implemented at some point.
	/// </summary>
	public class OverlayManager 
	{

		/// <summary>
		/// Does the manger have a overlay that renders heights.
		/// </summary>
		public bool HasHeightOverlay { get; private set; }

		/// <summary>
		/// Does the manger have a overlay that renders normal.
		/// </summary>
		public bool HasNormalOverlay { get; private set; }

		/// <summary>
		/// Does the manger have a overlay that renders foam.
		/// </summary>
		public bool HasFoamOverlay { get; private set; }

		/// <summary>
		/// Does the manger have a overlay that renders clip.
		/// </summary>
		public bool HasClipOverlay { get; private set; }

        /// <summary>
        /// The maximum amount of displacement on y axis
        /// from any of the overlays. 
        /// </summary>
        public float MaxDisplacement { get; private set; }

        /// <summary>
        /// The overlay material.
        /// </summary>
        Material m_overlayMat;

		/// <summary>
		/// The overlays.
		/// </summary>
		LinkedList<WaveOverlay> m_waveOverlays;

		/// <summary>
		/// All the overlays that can be queried.
		/// </summary>
		LinkedList<WaveOverlay> m_queryableOverlays;

		/// <summary>
		/// The m_containing that contain the query pos.
		/// </summary>
		LinkedList<QueryableOverlayResult> m_containingOverlays;

		/// <summary>
		/// A texture that when sampled from
		/// as a normal will give 0.
		/// </summary>
		Texture2D m_blankNormal;

		bool m_beenCleared;

        /// <summary>
        /// Temp arrays to hold overlays when they 
        /// get separated out before rendering.
        /// </summary>
        LinkedList<WaveOverlay> m_heightOverlays;
        LinkedList<WaveOverlay> m_normalOverlays;
        LinkedList<WaveOverlay> m_foamOverlays;
        LinkedList<WaveOverlay> m_clipOverlays;

        public OverlayManager( Material mat)
		{

            MaxDisplacement = 1.0f;

			m_overlayMat = mat;
			m_waveOverlays = new LinkedList<WaveOverlay>();
			m_queryableOverlays = new LinkedList<WaveOverlay>();
			m_containingOverlays = new LinkedList<QueryableOverlayResult>();

            m_heightOverlays = new LinkedList<WaveOverlay>();
            m_normalOverlays = new LinkedList<WaveOverlay>();
            m_foamOverlays = new LinkedList<WaveOverlay>();
            m_clipOverlays = new LinkedList<WaveOverlay>();

            m_blankNormal = new Texture2D(1,1, TextureFormat.ARGB32, false, true);
			m_blankNormal.SetPixel(0,0, new Color(0.5f,0.5f,1.0f,0.5f));
            m_blankNormal.hideFlags = HideFlags.HideAndDontSave;
            m_blankNormal.name = Ocean.InstanceName + " Blank Normal Texture";
            m_blankNormal.Apply();

		}

        /// <summary>
        /// Release resources.
        /// </summary>
        public void Release()
        {

            UnityEngine.Object.DestroyImmediate(m_blankNormal);

        }

		/// <summary>
		/// Updates what type of overlays manager has,
		/// removes any overlays marked as kill
		/// and sorts the overlays that can be queryed.
		/// </summary>
		public void Update()
		{

			HasHeightOverlay = false;
			HasNormalOverlay = false;
			HasFoamOverlay = false;
			HasClipOverlay = false;

            MaxDisplacement = 1.0f;

            m_queryableOverlays.Clear();

			LinkedList<WaveOverlay> remove = new LinkedList<WaveOverlay>();

            var e1 = m_waveOverlays.GetEnumerator();
			while(e1.MoveNext())
			{
                WaveOverlay overlay = e1.Current;

				if(overlay.Kill)
				{
					remove.AddFirst(overlay);
				}
				else if(!overlay.Hide)
				{
					bool canQuery = false;

					if(overlay.HeightTex.IsDrawable)
					{
						HasHeightOverlay = true;
						canQuery = true;

                        if (overlay.HeightTex.alpha > MaxDisplacement)
                            MaxDisplacement = overlay.HeightTex.alpha;
                    }

					if(overlay.NormalTex.IsDrawable)
						HasNormalOverlay = true;

					if(overlay.FoamTex.IsDrawable)
						HasFoamOverlay = true;

					if(overlay.ClipTex.IsDrawable)
					{
						HasClipOverlay = true;
						canQuery = true;
					}

					//if overlay has a drawable height or clip texture
					//then this overlay can be queried.
					if(canQuery)
						m_queryableOverlays.AddFirst(overlay);
				}
			}

            MaxDisplacement = Mathf.Min(MaxDisplacement, Ocean.MAX_OVERLAY_WAVE_HEIGHT);

            var e2 = remove.GetEnumerator();
            while (e2.MoveNext())
            {
				m_waveOverlays.Remove(e2.Current);
			}

		}

		/// <summary>
		/// Add the specified overlay.
		/// </summary>
		public void Add(WaveOverlay overlay)
		{
			if(overlay.Kill == true)
				return;

			m_waveOverlays.AddLast(overlay);
		}

		/// <summary>
		/// Remove the specified overlay.
		/// </summary>
		public void Remove(WaveOverlay overlay)
		{
			m_waveOverlays.Remove(overlay);
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			m_waveOverlays.Clear();
		}

		/// <summary>
		/// Returns true if any overlay in manager contains the xz pos.
		/// </summary>
		public bool QueryableContains(float x, float z, bool overrideIqnoreQuerys)
		{

            var e = m_queryableOverlays.GetEnumerator();
			while(e.MoveNext())
			{
                WaveOverlay overlay = e.Current;

				if(overlay.Hide) continue;

				bool b1 = (overrideIqnoreQuerys || !overlay.HeightTex.ignoreQuerys) && overlay.HeightTex.IsDrawable;
				bool b2 = (overrideIqnoreQuerys || !overlay.ClipTex.ignoreQuerys) && overlay.ClipTex.IsDrawable;
				
				if(!b1 && !b2) continue;

				if(overlay.Contains(x, z)) return true;
			}
			
			return false;
		}

		/// <summary>
		/// Gets all the query-able overlays that contain xz pos.
		/// </summary>
		public void GetQueryableContaining(float x, float z, bool overrideIqnoreQuerys, bool clipOnly)
		{

			m_containingOverlays.Clear();

            var e = m_queryableOverlays.GetEnumerator();
            while (e.MoveNext())
            {
                WaveOverlay overlay = e.Current;

                if (overlay.Hide) continue;

				bool b1 = !clipOnly && (overrideIqnoreQuerys || !overlay.HeightTex.ignoreQuerys) && overlay.HeightTex.IsDrawable;
				bool b2 = (overrideIqnoreQuerys || !overlay.ClipTex.ignoreQuerys) && overlay.ClipTex.IsDrawable;
				
				if(!b1 && !b2) continue;

				float u, v;
				if(overlay.Contains(x, z, out u, out v))
				{
					QueryableOverlayResult result;
					result.overlay = overlay;
					result.u = u;
					result.v = v;

					m_containingOverlays.AddFirst(result);
				}
				
			}
		}

		/// <summary>
		/// Queries the waves.
		/// </summary>
		public void QueryWaves(WaveQuery query)
		{
			
			if(m_queryableOverlays.Count == 0) return;
			if(!query.sampleOverlay) return;

            bool clipOnly = query.mode == QUERY_MODE.CLIP_TEST;

            float x = query.posX;
            float z = query.posZ;

			//Find all the overlays that have a affect on the wave height at this position
			//This will be overlays with a height tex, a height mask or a clip texture
			GetQueryableContaining(x, z, query.overrideIgnoreQuerys, clipOnly);

			float clipSum = 0.0f;
			float heightSum = 0.0f;
			float maskSum = 0.0f;

			OverlayClipTexture clipTex = null;
			OverlayHeightTexture heightTex = null;

            var e = m_containingOverlays.GetEnumerator();
			while(e.MoveNext())
			{

                QueryableOverlayResult result = e.Current;

                //If enable read/write is not enabled on tex it will throw a exception.
                //Catch and ignore.
                try
				{
					clipTex = result.overlay.ClipTex;
					heightTex = result.overlay.HeightTex;

					//If overlay has a clip tex sample it.
					if(clipTex.IsDrawable && clipTex.tex is Texture2D)
					{
						float clip = (clipTex.tex as Texture2D).GetPixelBilinear(result.u, result.v).a;
						clipSum += clip * Mathf.Max(0.0f, clipTex.alpha);
					}

					//If overlay has a height or mask tex sample it.
					if(!clipOnly && heightTex.IsDrawable)
					{

						float alpha = heightTex.alpha;
						float maskAlpha = Mathf.Max(0.0f, heightTex.maskAlpha);

						float height = 0.0f;
						float mask = 0.0f;

						if(heightTex.tex != null && heightTex.tex is Texture2D)
						{
							height = (heightTex.tex as Texture2D).GetPixelBilinear(result.u, result.v).a;
						}

						if(heightTex.mask != null && heightTex.mask is Texture2D)
						{
							mask = (heightTex.mask as Texture2D).GetPixelBilinear(result.u, result.v).a;
							mask = Mathf.Clamp01(mask * maskAlpha);
						}

						//Apply the height and mask depending on mask mode.
						if(heightTex.maskMode == OVERLAY_MASK_MODE.WAVES)
						{
							height *= alpha;
						}
						else if(heightTex.maskMode == OVERLAY_MASK_MODE.OVERLAY)
						{
							height *= alpha * mask;
							mask = 0;
						}
						else if(heightTex.maskMode == OVERLAY_MASK_MODE.WAVES_AND_OVERLAY)
						{
							height *= alpha * (1.0f-mask);
						}
						else if(heightTex.maskMode == OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND)
						{
							height *= alpha * mask;
						}

						heightSum += height;
						maskSum += mask;
					}

				}
				catch {}
			}

			clipSum = Mathf.Clamp01(clipSum);

			if(0.5f - clipSum < 0.0f)
			{
				query.result.isClipped = true;
			}

			maskSum = 1.0f - Mathf.Clamp01(maskSum);

			query.result.height *= maskSum;
			query.result.displacementX *= maskSum;
			query.result.displacementZ *= maskSum;

			query.result.height += heightSum;

		}

		/// <summary>
		/// Renders the wave overlays for this camera.
		/// </summary>
		public void RenderWaveOverlays(Camera cam, WaveOverlayData data)
		{

			if(!m_beenCleared)
			{
				RTUtility.ClearColor(data.height, new Color(0,0,0,0));
				RTUtility.ClearColor(data.normal, new Color(0,0,0,0));
				RTUtility.ClearColor(data.foam, new Color(0,0,0,0));
				RTUtility.ClearColor(data.clip, new Color(0,0,0,0));

				m_beenCleared = true;
			}

			if(m_waveOverlays.Count == 0) return;
			
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.current);

			m_heightOverlays.Clear();
            m_normalOverlays.Clear();
            m_foamOverlays.Clear();
            m_clipOverlays.Clear();

            var e = m_waveOverlays.GetEnumerator();
            while(e.MoveNext())
			{

                WaveOverlay overlay = e.Current;

                if (!overlay.Hide && GeometryUtility.TestPlanesAABB(planes, overlay.BoundingBox))
				{
					if(overlay.HeightTex.IsDrawable)
					{
                        m_heightOverlays.AddLast(overlay);
					}
					
					if(overlay.NormalTex.IsDrawable)
					{
                        m_normalOverlays.AddLast(overlay);
					}
					
					if(overlay.FoamTex.IsDrawable)
					{
                        m_foamOverlays.AddLast(overlay);
					}

					if(overlay.ClipTex.IsDrawable)
					{
                        m_clipOverlays.AddLast(overlay);
					}
					
				}
				
			}

			RenderHeightOverlays(m_heightOverlays, data.height);
			RenderNormalOverlays(m_normalOverlays, data.normal);
			RenderFoamOverlays(m_foamOverlays, data.foam);
			RenderClipOverlays(m_clipOverlays, data.clip);

			m_beenCleared = false;

		}

		void RenderHeightOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{

			if(target == null) return;

            var e = overlays.GetEnumerator();
            while (e.MoveNext())
            {

                WaveOverlay overlay = e.Current;

                m_overlayMat.SetFloat("Ceto_Overlay_Alpha", overlay.HeightTex.alpha);
				m_overlayMat.SetFloat("Ceto_Overlay_MaskAlpha", Mathf.Max(0.0f, overlay.HeightTex.maskAlpha));
				m_overlayMat.SetTexture("Ceto_Overlay_Height", (overlay.HeightTex.tex != null) ? overlay.HeightTex.tex : Texture2D.blackTexture);
				m_overlayMat.SetTexture("Ceto_Overlay_HeightMask", (overlay.HeightTex.mask != null) ? overlay.HeightTex.mask : Texture2D.blackTexture);
				m_overlayMat.SetFloat("Ceto_Overlay_MaskMode", (float)overlay.HeightTex.maskMode);

				Blit(overlay.Corners, overlay.HeightTex.scaleUV, overlay.HeightTex.offsetUV, target, m_overlayMat, (int)OVERLAY_PASS.HEIGHT);

			}

		}

		void RenderNormalOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{

			if(target == null) return;

            var e = overlays.GetEnumerator();
            while (e.MoveNext())
            {

                WaveOverlay overlay = e.Current;

                m_overlayMat.SetFloat("Ceto_Overlay_Alpha", Mathf.Max(0.0f, overlay.NormalTex.alpha));
				m_overlayMat.SetFloat("Ceto_Overlay_MaskAlpha", Mathf.Max(0.0f, overlay.NormalTex.maskAlpha));
				m_overlayMat.SetTexture("Ceto_Overlay_Normal", (overlay.NormalTex.tex != null) ? overlay.NormalTex.tex : m_blankNormal);
				m_overlayMat.SetTexture("Ceto_Overlay_NormalMask", (overlay.NormalTex.mask != null) ? overlay.NormalTex.mask : Texture2D.blackTexture);
				m_overlayMat.SetFloat("Ceto_Overlay_MaskMode", (float)overlay.NormalTex.maskMode);

				Blit(overlay.Corners, overlay.NormalTex.scaleUV, overlay.NormalTex.offsetUV, target, m_overlayMat, (int)OVERLAY_PASS.NORMAL);
			}
		}

		void RenderFoamOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{

			if(target == null) return;

            var e = overlays.GetEnumerator();
            while (e.MoveNext())
            {

                WaveOverlay overlay = e.Current;

                m_overlayMat.SetFloat("Ceto_Overlay_Alpha", Mathf.Max(0.0f, overlay.FoamTex.alpha));
				m_overlayMat.SetFloat("Ceto_Overlay_MaskAlpha", Mathf.Max(0.0f, overlay.FoamTex.maskAlpha));
				m_overlayMat.SetTexture("Ceto_Overlay_Foam", (overlay.FoamTex.tex != null) ? overlay.FoamTex.tex : Texture2D.blackTexture);
				m_overlayMat.SetTexture("Ceto_Overlay_FoamMask", (overlay.FoamTex.mask != null) ? overlay.FoamTex.mask : Texture2D.blackTexture);
				m_overlayMat.SetFloat("Ceto_Overlay_MaskMode", (float)overlay.FoamTex.maskMode);
				
				Blit(overlay.Corners, overlay.FoamTex.scaleUV, overlay.FoamTex.offsetUV, target, m_overlayMat, (int)OVERLAY_PASS.FOAM);
			}
		}

		void RenderClipOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{
			if(target == null) return;

            var e = overlays.GetEnumerator();
            while (e.MoveNext())
            {

                WaveOverlay overlay = e.Current;

                m_overlayMat.SetFloat("Ceto_Overlay_Alpha", Mathf.Max(0.0f, overlay.ClipTex.alpha));
				m_overlayMat.SetTexture("Ceto_Overlay_Clip", (overlay.ClipTex.tex != null) ? overlay.ClipTex.tex : Texture2D.blackTexture);
				
				Blit(overlay.Corners, overlay.ClipTex.scaleUV, overlay.ClipTex.offsetUV, target, m_overlayMat, (int)OVERLAY_PASS.CLIP);
			}
		}

		void Blit(Vector4[] corners, Vector2 scale, Vector2 offset, RenderTexture des, Material mat, int pass)
		{

			Graphics.SetRenderTarget(des);
			
			GL.PushMatrix();
			GL.LoadOrtho();
			
			mat.SetPass(pass);

			GL.Begin(GL.QUADS);

			GL.MultiTexCoord2(0, offset.x, offset.y); 
			GL.MultiTexCoord2(1, 0.0f, 0.0f); 
			GL.Vertex(corners[0]);

			GL.MultiTexCoord2(0, offset.x + 1.0f * scale.x, offset.y);
			GL.MultiTexCoord2(1, 1.0f, 0.0f); 
			GL.Vertex(corners[1]);

			GL.MultiTexCoord2(0, offset.x + 1.0f * scale.x, offset.y + 1.0f * scale.y); 
			GL.MultiTexCoord2(1, 1.0f, 1.0f); 
			GL.Vertex(corners[2]);

			GL.MultiTexCoord2(0, offset.x, offset.y + 1.0f * scale.y);
			GL.MultiTexCoord2(1, 0.0f, 1.0f); 
			GL.Vertex(corners[3]);
			GL.End();
			
			GL.PopMatrix();

		}


	}

}














