using System;
using System.Collections.Generic;
using UnityEngine;

using Ceto.Common.Unity.Utility;

namespace Ceto
{

    /// <summary>
    /// Class to provide reflections using the planar reflection method.
    /// 
    /// Note - since this method presumes that the wave plane is flat having
    /// rough wave conditions can cause reflection artefacts. You can use the 
    /// reflection roughness to blur out the reflections to hide these artefacts.
    /// You wont have sharp reflections but thats the best that can be done with this method.
    /// 
    /// </summary>
	[AddComponentMenu("Ceto/Components/PlanarReflection")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Ocean))]
	public class PlanarReflection : ReflectionBase
    {

		const float CLIP_PLANE_OFFSET = 0.07f;
		public const float MAX_REFLECTION_INTENSITY = 2.0f;
		public const float MAX_REFLECTION_DISORTION = 4.0f;

        /// <summary>
        /// The layers that will be rendered for the reflections.
        /// </summary>
		public LayerMask reflectionMask = 0;

        /// <summary>
        /// The resolution of the reflection texture relative to the screen.
        /// </summary>
        public REFLECTION_RESOLUTION reflectionResolution = REFLECTION_RESOLUTION.HALF;

        /// <summary>
        /// If pixels lights are to be used in the reflections.
        /// Disable for increased performance.
        /// </summary>
		public bool pixelLightsInReflection = false;
        
        /// <summary>
        /// Should the reflection camera have fog enabled 
        /// when rendering.
        /// </summary>
        public bool fogInReflection = false;

        /// <summary>
        /// Should the skybox be reflected.
        /// </summary>
        public bool skyboxInReflection = true;

        /// <summary>
        /// The blur mode. Down sampling is faster but will lose resolution.
        /// </summary>
        public ImageBlur.BLUR_MODE blurMode = ImageBlur.BLUR_MODE.OFF;
		
		/// Blur iterations - larger number means more blur.
		[Range(0, 4)]
		public int blurIterations = 1;
		
		/// Blur spread for each iteration. Lower values
		/// give better looking blur, but require more iterations to
		/// get large blurs. Value is usually between 0.5 and 1.0.
		[Range(0.5f, 1.0f)]
		/*public*/ float blurSpread = 0.6f;

        /// <summary>
        /// Tints the reflection color.
        /// </summary>
		public Color reflectionTint = Color.white;

        /// <summary>
        /// Adjusts the reflection intensity.
        /// </summary>
		[Range(0.0f, MAX_REFLECTION_INTENSITY)]
		public float reflectionIntensity = 0.6f;

        /// <summary>
        /// Distorts the reflections based on the wave normal.
        /// </summary>
		[Range(0.0f, MAX_REFLECTION_DISORTION)]
        public float reflectionDistortion = 0.5f;
		
        /// <summary>
        /// The anisotropic filter value for the reflection texture.
        /// </summary>
		public int ansio = 2;

        /// <summary>
        /// If a custom reflection method is provide this 
        /// is the game object passed. Its just a empty
        /// gameobject where the transform contains the
        /// reflections plane position.
        /// </summary>
		GameObject m_dummy;

		/// <summary>
		/// The used to blur the reflections.
		/// </summary>
		ImageBlur m_imageBlur;

		/// <summary>
		/// The blur shader.
		/// </summary>
		[HideInInspector]
		public Shader blurShader;

        void Start()
        {

            try
            {
                m_imageBlur = new ImageBlur(blurShader);
            }
            catch (Exception e)
            {
                Ocean.LogError(e.ToString());
                WasError = true;
                enabled = false;
            }

        }

        protected override void OnEnable()
		{
			base.OnEnable();
			
			Shader.EnableKeyword("CETO_REFLECTION_ON");
			
		}
		
		protected override void OnDisable()
		{

			base.OnDisable();

			Shader.DisableKeyword("CETO_REFLECTION_ON");
			
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            try
            {

                if(m_dummy != null)
                    DestroyImmediate(m_dummy);

            }
			catch(Exception e)
			{
                Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}

}

        void Update()
		{

			reflectionIntensity = Mathf.Max(0.0f, reflectionIntensity);

			Shader.SetGlobalVector("Ceto_ReflectionTint", reflectionTint * reflectionIntensity); 
			Shader.SetGlobalFloat("Ceto_ReflectionDistortion", reflectionDistortion * 0.05f); 

		}

        /// <summary>
        /// Render the reflections for this objects position
        /// and the current camera.
        /// </summary>
		public override void RenderReflection(GameObject go)
		{

            try
            {

                if (!enabled) return;

                Camera cam = Camera.current;
                if (cam == null) return;

                CameraData data = m_ocean.FindCameraData(cam);

                //Create the data needed if not already created.
                if (data.reflection == null)
                    data.reflection = new ReflectionData();

                if (data.reflection.updated) return;

                RenderTexture reflections = null;

                if (data.reflection.cam != null)
                {
                    DisableFog disableFog = data.reflection.cam.GetComponent<DisableFog>();
                    if(disableFog != null) disableFog.enabled = !fogInReflection;
                }
				
				if(RenderReflectionCustom != null)
				{
                    //If using a custom method

                    //Destroy the camera if already created as its no longer needed.
                    if (data.reflection.cam != null)
                    {
                        RTUtility.ReleaseAndDestroy(data.reflection.cam.targetTexture);
                        data.reflection.cam.targetTexture = null;

                        Destroy(data.reflection.cam.gameObject);
                        Destroy(data.reflection.cam);
                        data.reflection.cam = null;
                    }

                    CreateRenderTarget(data.reflection, cam.name, cam.pixelWidth, cam.pixelHeight, cam.hdr);

                    //Create the dummy object if null
                    if (m_dummy == null)
					{
						m_dummy = new GameObject(Ocean.InstanceName + " Reflection Dummy Gameobject");
						m_dummy.hideFlags = HideFlags.HideAndDontSave;
					}
					
                    //Set the position of the reflection plane.
					m_dummy.transform.position = new Vector3(0.0f, m_ocean.level, 0.0f);
					reflections = RenderReflectionCustom(m_dummy);
				}
				else
				{
                    //Else use normal method.

                    CreateReflectionCameraFor(cam, data.reflection);
                    CreateRenderTarget(data.reflection, cam.name, cam.pixelWidth, cam.pixelHeight, cam.hdr);

                    NotifyOnEvent.Disable = true;
		            RenderReflectionFor(cam, data.reflection.cam);
					NotifyOnEvent.Disable = false;

					reflections = data.reflection.cam.targetTexture;
				}

                //The reflections texture should now contain the rendered 
                //reflections for the current cameras view.
				if(reflections != null)
				{
                    //Blit into another texture to take a copy.
					Graphics.Blit(reflections, data.reflection.tex);

					m_imageBlur.BlurIterations = blurIterations;
					m_imageBlur.BlurMode = blurMode;
					m_imageBlur.BlurSpread = blurSpread;
					m_imageBlur.Blur(data.reflection.tex);

					Shader.SetGlobalTexture(Ocean.REFLECTION_TEXTURE_NAME, data.reflection.tex);
				}

	            data.reflection.updated = true;

			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}

		}

        /// <summary>
        /// Create the reflection camera for this camera.
        /// </summary>
        void CreateReflectionCameraFor(Camera cam, ReflectionData data)
        {

            if (data.cam != null) return;

			GameObject go = new GameObject(Ocean.InstanceName + " Reflection Camera: " + cam.name);
			go.hideFlags = HideFlags.HideAndDontSave;

			DisableFog disableFog = go.AddComponent<DisableFog>();
            disableFog.enabled = !fogInReflection;

            //go.AddComponent<DisableShadows>();

            data.cam = go.AddComponent<Camera>();

            data.cam.CopyFrom(cam);
            data.cam.depthTextureMode = DepthTextureMode.None;
            data.cam.renderingPath = RenderingPath.Forward;
            data.cam.enabled = false;
            data.cam.hdr = cam.hdr;
            data.cam.targetTexture = null;

        }

        /// <summary>
        /// Create the render targets for the reflection camera and the reflection texture.
        /// </summary>
		void CreateRenderTarget(ReflectionData data, string camName, int width, int height, bool isHdr)
        {

            int scale = ResolutionToNumber(reflectionResolution);
            width /= scale;
            height /= scale;

            //If the texture has been created and settings have not changed
            //just update the ansio settings and return.
			if (data.tex != null && data.tex.width == width && data.tex.height == height)
            {
				data.tex.anisoLevel = ansio;
                return;
            }

            //Settings have changed or textures not created.

            if (data.tex != null)
                RTUtility.ReleaseAndDestroy(data.tex);

            RenderTextureFormat format;
            if (isHdr || QualitySettings.activeColorSpace == ColorSpace.Linear)
                format = RenderTextureFormat.ARGBHalf;
            else
                format = RenderTextureFormat.ARGB32;

            //This is the actual texture that will be sampled from for the reflections
            data.tex = new RenderTexture(width, height, 0, format, RenderTextureReadWrite.Default);
            data.tex.filterMode = FilterMode.Bilinear;
            data.tex.wrapMode = TextureWrapMode.Clamp;
            data.tex.useMipMap = false;
            data.tex.anisoLevel = ansio;
            data.tex.hideFlags = HideFlags.HideAndDontSave;
            data.tex.name = Ocean.InstanceName + " Reflection Texture: " + camName;

            //This is the camera that will render the reflections.
            //Maybe null if a custom reflection method is being used.
            if (data.cam != null)
            {

                if (data.cam.targetTexture != null)
                    RTUtility.ReleaseAndDestroy(data.cam.targetTexture);

                data.cam.targetTexture = new RenderTexture(width, height, 16, format, RenderTextureReadWrite.Default);
                data.cam.targetTexture.filterMode = FilterMode.Bilinear;
                data.cam.targetTexture.wrapMode = TextureWrapMode.Clamp;
                data.cam.targetTexture.useMipMap = false;
                data.cam.targetTexture.anisoLevel = 0;
                data.cam.targetTexture.hideFlags = HideFlags.HideAndDontSave;
                data.cam.targetTexture.name = Ocean.InstanceName + " Reflection Render Target: " + camName;
            }



        }

        /// <summary>
        /// Convert the setting enum to a meaning full number.
        /// </summary>
        int ResolutionToNumber(REFLECTION_RESOLUTION resolution)
        {

            switch (resolution)
            {
                case REFLECTION_RESOLUTION.FULL:
                    return 1;

                case REFLECTION_RESOLUTION.HALF:
                    return 2;

                case REFLECTION_RESOLUTION.QUARTER:
                    return 4;


                default:
                    return 2;
            }

        }

        /// <summary>
        /// Update the reflection cameras sky box to 
        /// match the current cameras sky box.
        /// </summary>
		void UpdateSkyBox(Camera cam, Camera reflectCamera)
		{

            reflectCamera.backgroundColor = m_ocean.defaultSkyColor;
            reflectCamera.clearFlags = (skyboxInReflection) ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;

            /*
            reflectCamera.backgroundColor = m_ocean.defaultSkyColor;
            reflectCamera.clearFlags = CameraClearFlags.SolidColor;

            if (skyboxInReflection && cam.clearFlags == CameraClearFlags.Skybox)
            {
                reflectCamera.clearFlags = CameraClearFlags.Skybox;

                Skybox skybox = cam.gameObject.GetComponent<Skybox>();

                if (skybox)
                {
                    Skybox sb = reflectCamera.gameObject.GetComponent<Skybox>();

                    if (!sb)
                        sb = reflectCamera.gameObject.AddComponent<Skybox>();

                    sb.material = skybox.material;
                }
            }
            */
        }

        /// <summary>
        /// Render the reflections.
        /// </summary>
        void RenderReflectionFor(Camera cam, Camera reflectCamera)
        {
		
			UpdateSkyBox(cam, reflectCamera);

            GL.invertCulling = true;

			reflectCamera.fieldOfView = cam.fieldOfView;
			reflectCamera.nearClipPlane = cam.nearClipPlane;
			reflectCamera.farClipPlane = cam.farClipPlane;
            reflectCamera.rect = new Rect(0, 0, 1, 1);

            Vector3 eulerA = cam.transform.eulerAngles;

            reflectCamera.transform.eulerAngles = new Vector3(-eulerA.x, eulerA.y, eulerA.z);
            reflectCamera.transform.position = cam.transform.position;

			float level = m_ocean.level;

            Vector3 pos = new Vector3(0,level,0);
            Vector3 normal = Vector3.up;

            float d = -Vector3.Dot(normal, pos) - CLIP_PLANE_OFFSET;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            reflection = CalculateReflectionMatrix(reflection, reflectionPlane);
            Vector3 oldPos = cam.transform.position;
            Vector3 newpos = reflection.MultiplyPoint(oldPos);

            reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            Vector4 clipPlane = CameraSpacePlane(reflectCamera, pos, normal, 1.0f);

            Matrix4x4 projection = cam.projectionMatrix;
            projection = CalculateObliqueMatrix(projection, clipPlane);
            reflectCamera.projectionMatrix = projection;

            reflectCamera.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            reflectCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

			reflectCamera.cullingMask = reflectionMask;
			reflectCamera.cullingMask = OceanUtility.HideLayer(reflectCamera.cullingMask, Ocean.OCEAN_LAYER);

			int oldPixelLightCount = QualitySettings.pixelLightCount;
			if (!pixelLightsInReflection) 
				QualitySettings.pixelLightCount = 0;

            reflectCamera.Render();

			QualitySettings.pixelLightCount = oldPixelLightCount;

            GL.invertCulling = false;
		
        }

        Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
        {
            Vector4 q = projection.inverse * new Vector4(
                Mathf.Sign(clipPlane.x),
				Mathf.Sign(clipPlane.y),
                1.0F,
                1.0F
                );
            Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
            // third row = clip plane - fourth row
            projection[2] = c.x - projection[3];
            projection[6] = c.y - projection[7];
            projection[10] = c.z - projection[11];
            projection[14] = c.w - projection[15];

            return projection;
        }


        Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1.0F - 2.0F * plane[0] * plane[0]);
            reflectionMat.m01 = (- 2.0F * plane[0] * plane[1]);
            reflectionMat.m02 = (- 2.0F * plane[0] * plane[2]);
            reflectionMat.m03 = (- 2.0F * plane[3] * plane[0]);

            reflectionMat.m10 = (- 2.0F * plane[1] * plane[0]);
            reflectionMat.m11 = (1.0F - 2.0F * plane[1] * plane[1]);
            reflectionMat.m12 = (- 2.0F * plane[1] * plane[2]);
            reflectionMat.m13 = (- 2.0F * plane[3] * plane[1]);

            reflectionMat.m20 = (- 2.0F * plane[2] * plane[0]);
            reflectionMat.m21 = (- 2.0F * plane[2] * plane[1]);
            reflectionMat.m22 = (1.0F - 2.0F * plane[2] * plane[2]);
            reflectionMat.m23 = (- 2.0F * plane[3] * plane[2]);

            reflectionMat.m30 = 0.0F;
            reflectionMat.m31 = 0.0F;
            reflectionMat.m32 = 0.0F;
            reflectionMat.m33 = 1.0F;

            return reflectionMat;
        }

        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * CLIP_PLANE_OFFSET;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;

            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }
    }
}

















