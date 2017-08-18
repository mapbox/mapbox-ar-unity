namespace Mapbox.Unity.Ar
{
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Location;
	using UnityEngine;
	using Mapbox.Unity.Utilities;
	using UnityEngine.XR.iOS;
	using System;

	public class ManualSynchronizationContextBehaviour : MonoBehaviour, ISynchronizationContext
	{
		[SerializeField]
		MapAtCurrentLocation _map;

		[SerializeField]
		Transform _mapCamera;

		[SerializeField]
		AbstractAlignmentStrategy _alignmentStrategy;

		[SerializeField]
		TransformLocationProvider _locationProvider;

		float _lastHeight;
		float _lastHeading;

		public event Action<Alignment> OnAlignmentAvailable = delegate { };

		void Start()
		{
			_map.OnInitialized += Map_OnInitialized;
			UnityARSessionNativeInterface.ARAnchorAddedEvent += AnchorAdded;
		}

		void Map_OnInitialized()
		{
			_map.OnInitialized -= Map_OnInitialized;
			_locationProvider.OnHeadingUpdated += LocationProvider_OnHeadingUpdated;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}

		void LocationProvider_OnHeadingUpdated(object sender, Unity.Location.HeadingUpdatedEventArgs e)
		{
			_lastHeading = e.Heading;
		}

		void LocationProvider_OnLocationUpdated(object snder, LocationUpdatedEventArgs e)
		{
			var alignment = new Alignment();
			alignment.Position = -Conversions.GeoToWorldPosition(e.Location, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz() + _map.Root.position;
			alignment.Position.y = _lastHeight;
			alignment.Rotation = -_lastHeading + _map.Root.localEulerAngles.y;

			// TODO: change this so that alignment strategies listen to this event, rather than telling them to align.
			OnAlignmentAvailable(alignment);

			_alignmentStrategy.Align(alignment);
			var mapCameraPosition = Vector3.zero;
			mapCameraPosition.y = _mapCamera.localPosition.y;
			var mapCameraRotation = Vector3.zero;
			mapCameraRotation.x = _mapCamera.localEulerAngles.x;
			_mapCamera.localPosition = mapCameraPosition;
			_mapCamera.RotateAround(_map.Root.position, Vector3.up, -_lastHeading);
		}

		void AnchorAdded(ARPlaneAnchor anchorData)
		{
			_lastHeight = UnityARMatrixOps.GetPosition(anchorData.transform).y;
			Unity.Utilities.Console.Instance.Log(string.Format("AR Plane Height: {0}", _lastHeight), "yellow");
		}
	}
}