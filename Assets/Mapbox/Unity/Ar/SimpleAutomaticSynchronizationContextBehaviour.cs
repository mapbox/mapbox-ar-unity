namespace Mapbox.Unity.Ar
{
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Location;
	using UnityEngine;
	using Mapbox.Unity.Utilities;
	using UnityEngine.XR.iOS;

	public class SimpleAutomaticSynchronizationContextBehaviour : MonoBehaviour
	{
		[SerializeField]
		Transform _arPositionReference;

		[SerializeField]
		MapAtCurrentLocation _map;

		[SerializeField]
		bool _useAutomaticSynchronizationBias;

		[SerializeField]
		float _synchronizationBias = 1f;

		[SerializeField]
		float _arTrustRange = 10f;

		[SerializeField]
		float _minimumDeltaDistance = 2f;

		[SerializeField]
		AbstractAlignmentStrategy _alignmentStrategy;

		SimpleAutomaticSynchronizationContext _synchronizationContext;

		float _lastHeading;
		float _lastHeight;

		ILocationProvider _locationProvider;
		public ILocationProvider LocationProvider
		{
			private get
			{
				if (_locationProvider == null)
				{
#if UNITY_EDITOR
					_locationProvider = LocationProviderFactory.Instance.TransformLocationProvider;
#else
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
#endif
				}

				return _locationProvider;
			}
			set
			{
				if (_locationProvider != null)
				{
					_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;

				}
				_locationProvider = value;
				_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			}
		}

		void Start()
		{
			_synchronizationContext = new SimpleAutomaticSynchronizationContext();
			_synchronizationContext.MinimumDeltaDistance = _minimumDeltaDistance;
			_synchronizationContext.ArTrustRange = _arTrustRange;
			_synchronizationContext.UseAutomaticSynchronizationBias = _useAutomaticSynchronizationBias;
			_synchronizationContext.SynchronizationBias = _synchronizationBias;
			_synchronizationContext.OnAlignmentAvailable += SynchronizationContext_OnAlignmentAvailable;
			_map.OnInitialized += Map_OnInitialized;
			UnityARSessionNativeInterface.ARAnchorAddedEvent += AnchorAdded;
			UnityARSessionNativeInterface.ARSessionTrackingChangedEvent += UnityARSessionNativeInterface_ARSessionTrackingChanged;
		}

		void Map_OnInitialized()
		{
			_map.OnInitialized -= Map_OnInitialized;

			// We don't want location updates until we have a map, otherwise our conversion will fail.
			LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			LocationProvider.OnHeadingUpdated += LocationProvider_OnHeadingUpdated;
		}

		// TODO: use extents of anchor to determine "floor" plane.
		// Also, account for initial height. What happens when you walk down a hill?
		void AnchorAdded(ARPlaneAnchor anchorData)
		{
			_lastHeight = UnityARMatrixOps.GetPosition(anchorData.transform).y;
			Console.Instance.Log(string.Format("AR Plane Height: {0}", _lastHeight), "yellow");
		}

		// FIXME: for some reason, I never get "normal." Use caution, here.
		void UnityARSessionNativeInterface_ARSessionTrackingChanged(UnityEngine.XR.iOS.UnityARCamera camera)
		{
			Console.Instance.Log(string.Format("AR Tracking State Changed: {0}: {1}", camera.trackingState, camera.trackingReason), "silver");
		}

		void LocationProvider_OnLocationUpdated(object sender, LocationUpdatedEventArgs e)
		{
			Console.Instance.Log(string.Format("Location: {0},{1}\tAccuracy: {2}\tHeading: {3}",
																	e.Location.x, e.Location.y, e.Accuracy, _lastHeading), "lightblue");

			var location = new Location();
			location.Position = Conversions.GeoToWorldPosition(e.Location,
																 _map.CenterMercator,
																 _map.WorldRelativeScale).ToVector3xz();

			location.Accuracy = e.Accuracy;
			_synchronizationContext.AddSynchronizationNodes(location, _arPositionReference.localPosition);
		}

		void LocationProvider_OnHeadingUpdated(object sender, HeadingUpdatedEventArgs e)
		{
			_lastHeading = e.Heading;
		}

		void SynchronizationContext_OnAlignmentAvailable(Ar.Alignment alignment)
		{
			var position = alignment.Position;
			position.y = _lastHeight;
			alignment.Position = position;
			_alignmentStrategy.Align(alignment);
		}
	}
}