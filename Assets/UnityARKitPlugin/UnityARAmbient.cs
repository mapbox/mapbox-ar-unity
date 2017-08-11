using System.Runtime.InteropServices;
using UnityEngine.XR.iOS;

namespace UnityEngine.XR.iOS
{
    public class UnityARAmbient : MonoBehaviour
    {

        private Light l;

        public void Start()
        {
            l = GetComponent<Light>();
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UpdateAmbientIntensity;
        }

		void UpdateAmbientIntensity(UnityARCamera camera)
		{
			// Convert ARKit intensity to Unity intensity
			// ARKit ambient intensity ranges 0-2000
			// Unity ambient intensity ranges 0-8 (for over-bright lights)
			float newai = camera.ambientIntensity;
			l.intensity = newai / 1000.0f;
		}
    }
}
