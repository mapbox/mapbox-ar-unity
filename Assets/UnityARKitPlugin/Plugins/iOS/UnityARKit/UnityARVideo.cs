using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.XR.iOS
{

    public class UnityARVideo : MonoBehaviour
    {
        public Material m_ClearMaterial;

        private CommandBuffer m_VideoCommandBuffer;
        private Texture2D _videoTextureY;
        private Texture2D _videoTextureCbCr;

		private bool bCommandBufferInitialized;

		private float fTexCoordScale;
		private ScreenOrientation screenOrientation;


		public void Start()
		{
			fTexCoordScale = 1.0f;
			screenOrientation = ScreenOrientation.LandscapeLeft;
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateFrame;
			bCommandBufferInitialized = false;
		}

		void UpdateFrame(UnityARCamera cam)
		{
			fTexCoordScale = cam.videoParams.texCoordScale;
			screenOrientation = (ScreenOrientation) cam.videoParams.screenOrientation;

		}

		void InitializeCommandBuffer()
		{
			m_VideoCommandBuffer = new CommandBuffer(); 
			m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, m_ClearMaterial);
			GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
			bCommandBufferInitialized = true;

		}

		void OnDestroy()
		{
			GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UpdateFrame;
			bCommandBufferInitialized = false;
		}

#if !UNITY_EDITOR

        public void OnPreRender()
        {
			ARTextureHandles handles = UnityARSessionNativeInterface.GetARSessionNativeInterface ().GetARVideoTextureHandles();
            if (handles.textureY == System.IntPtr.Zero || handles.textureCbCr == System.IntPtr.Zero)
            {
                return;
            }

            if (!bCommandBufferInitialized) {
                InitializeCommandBuffer ();
            }

            Resolution currentResolution = Screen.currentResolution;

            // Texture Y
            _videoTextureY = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
                TextureFormat.R8, false, false, (System.IntPtr)handles.textureY);
            _videoTextureY.filterMode = FilterMode.Bilinear;
            _videoTextureY.wrapMode = TextureWrapMode.Repeat;
            _videoTextureY.UpdateExternalTexture(handles.textureY);

            // Texture CbCr
            _videoTextureCbCr = Texture2D.CreateExternalTexture(currentResolution.width, currentResolution.height,
                TextureFormat.RG16, false, false, (System.IntPtr)handles.textureCbCr);
            _videoTextureCbCr.filterMode = FilterMode.Bilinear;
            _videoTextureCbCr.wrapMode = TextureWrapMode.Repeat;
            _videoTextureCbCr.UpdateExternalTexture(handles.textureCbCr);

            m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
            m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
            int isPortrait = 0;

            float rotation = 0;
            if (screenOrientation == ScreenOrientation.Portrait) {
                rotation = -90;
                isPortrait = 1;
            }
            else if (screenOrientation == ScreenOrientation.PortraitUpsideDown) {
                rotation = 90;
                isPortrait = 1;
            }
            else if (screenOrientation == ScreenOrientation.LandscapeRight) {
                rotation = -180;
            }
            Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(0.0f, 0.0f, rotation), Vector3.one);
            m_ClearMaterial.SetMatrix("_TextureRotation", m);
			m_ClearMaterial.SetFloat("_texCoordScale", fTexCoordScale);
            m_ClearMaterial.SetInt("_isPortrait", isPortrait);
        }
#else

		public void SetYTexure(Texture2D YTex)
		{
			_videoTextureY = YTex;
		}

		public void SetUVTexure(Texture2D UVTex)
		{
			_videoTextureCbCr = UVTex;
		}

		public void OnPreRender()
		{

			if (!bCommandBufferInitialized) {
				InitializeCommandBuffer ();
			}

			m_ClearMaterial.SetTexture("_textureY", _videoTextureY);
			m_ClearMaterial.SetTexture("_textureCbCr", _videoTextureCbCr);
			int isPortrait = 0;

			float rotation = 0;
			if (screenOrientation == ScreenOrientation.Portrait) {
				rotation = -90;
				isPortrait = 1;
			}
			else if (screenOrientation == ScreenOrientation.PortraitUpsideDown) {
				rotation = 90;
				isPortrait = 1;
			}
			else if (screenOrientation == ScreenOrientation.LandscapeRight) {
				rotation = -180;
			}

			Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(0.0f, 0.0f, rotation), Vector3.one);
			m_ClearMaterial.SetMatrix("_TextureRotation", m);
			m_ClearMaterial.SetFloat("_texCoordScale", fTexCoordScale);
			m_ClearMaterial.SetInt("_isPortrait", isPortrait);


		}
 
#endif
    }
}
