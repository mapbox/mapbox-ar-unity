namespace Mapbox.Unity.Location
{
	using Mapbox.Unity.Map;
	using System;
	using Mapbox.Unity.Utilities;
	using Mapbox.Utils;
	using UnityEngine;

	/// <summary>
	/// The TransformLocationProvider is responsible for providing mock location and heading data
	/// for testing purposes in the Unity editor.
	/// This is achieved by querying a Unity <see href="https://docs.unity3d.com/ScriptReference/Transform.html">Transform</see> every frame.
	/// You might use this to to update location based on a touched position, for example.
	/// </summary>
	public class TransformLocationProvider : MonoBehaviour, ILocationProvider
	{
		[SerializeField]
		private MapAtCurrentLocation _map;

		[SerializeField]
		float _accuracy = 5f;

		/// <summary>
		/// The transform that will be queried for location and heading data.
		/// </summary>
		[SerializeField]
		Transform _targetTransform;

		[SerializeField]
		bool _sendEventsOnUpdate = true;

		[SerializeField]
		bool _sendUpdate;

		/// <summary>
		/// Gets the latitude, longitude of the transform.
		/// This is converted from unity world space to real world geocoordinate space.
		/// </summary>
		/// <value>The location.</value>
		public Vector2d Location
		{
			get
			{
				return GetLocation();
			}
		}

		/// <summary>
		/// Sets the target transform.
		/// Use this if you want to switch the transform at runtime.
		/// </summary>
		public Transform TargetTransform
		{
			set
			{
				_targetTransform = value;
			}
		}

		/// <summary>
		/// Occurs every frame.
		/// </summary>
		public event EventHandler<HeadingUpdatedEventArgs> OnHeadingUpdated;

		/// <summary>
		/// Occurs every frame.
		/// </summary>
		public event EventHandler<LocationUpdatedEventArgs> OnLocationUpdated;

#if UNITY_EDITOR
		void Update()
		{
			if (_sendEventsOnUpdate)
			{
				SendEvents();
			}
		}
#endif

		Vector2d GetLocation()
		{
			return _targetTransform.GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
		}

		public void SendEvents()
		{
			if (OnHeadingUpdated != null)
			{
				OnHeadingUpdated(this, new HeadingUpdatedEventArgs() { Heading = _targetTransform.localEulerAngles.y });
			}

			if (OnLocationUpdated != null)
			{
				OnLocationUpdated(this, new LocationUpdatedEventArgs() { Location = GetLocation(), Accuracy = _accuracy });
			}
		}

		void OnValidate()
		{
			if (_sendUpdate)
			{
				_sendUpdate = false;
				SendEvents();
			}
		}
	}
}