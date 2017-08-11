namespace Mapbox.Unity.MeshGeneration.Factories
{
	using UnityEngine;
	using Mapbox.Directions;
	using Mapbox.Unity.Map;
	using Modifiers;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;

	public class DirectionsFactory : MonoBehaviour
	{
		[SerializeField]
		MapAtCurrentLocation _map;

		[SerializeField]
		MapVisualizer _mapVisualizer;

		[SerializeField]
		Transform[] _waypoints;

		[SerializeField]
		LineRenderer _lineRenderer;

		[SerializeField]
		float _height = 1f;

		Directions _directions;

		void Awake()
		{
			_directions = MapboxAccess.Instance.Directions;
			_mapVisualizer.OnMapVisualizerStateChanged += MapVisualizer_OnMapVisualizerStateChanged;
		}

		void MapVisualizer_OnMapVisualizerStateChanged(MeshGeneration.ModuleState state)
		{
			if (state == ModuleState.Finished)
			{
				_mapVisualizer.OnMapVisualizerStateChanged -= MapVisualizer_OnMapVisualizerStateChanged;
				Query();
			}
		}

		void Query()
		{
			var count = _waypoints.Length;
			var wp = new Vector2d[count];
			for (int i = 0; i < count; i++)
			{
				wp[i] = _waypoints[i].GetLocalGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
			}
			var _directionResource = new DirectionResource(wp, RoutingProfile.Driving);
			_directionResource.RoutingProfile = RoutingProfile.Walking;
			_directions.Query(_directionResource, HandleDirectionsResponse);
		}

		void HandleDirectionsResponse(DirectionsResponse response)
		{
			if (null == response.Routes || response.Routes.Count < 1)
			{
				return;
			}

			var count = response.Routes[0].Geometry.Count;
			_lineRenderer.positionCount = count;
			for (int i = 0; i < count; i++)
			{
				var point = response.Routes[0].Geometry[i];
				var position = Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz();
				position.y = _height;
				_lineRenderer.SetPosition(i, position);
			}
		}
	}
}