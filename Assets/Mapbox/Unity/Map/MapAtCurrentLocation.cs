namespace Mapbox.Unity.Map
{
	using System;
	using Mapbox.Unity.MeshGeneration;
	using Mapbox.Unity.Utilities;
	using Utils;
	using UnityEngine;
	using Mapbox.Map;
	using Mapbox.Unity.Location;

	public class MapAtCurrentLocation : MonoBehaviour, IMap
	{
		[SerializeField]
		[Range(0, 22)]
		int _zoom;
		public int Zoom
		{
			get
			{
				return _zoom;
			}
			set
			{
				_zoom = value;
			}
		}

		[SerializeField]
		Transform _root;
		public Transform Root
		{
			get
			{
				return _root;
			}
		}

		[SerializeField]
		AbstractTileProvider _tileProvider;

		[SerializeField]
		MapVisualizer _mapVisualizer;

		[SerializeField]
		bool _snapMapHeightToZero = true;

		MapboxAccess _fileSouce;

		Vector2d _mapCenterLatitudeLongitude;
		public Vector2d CenterLatitudeLongitude
		{
			get
			{
				return _mapCenterLatitudeLongitude;
			}
			set
			{
				_mapCenterLatitudeLongitude = value;
			}
		}

		Vector2d _mapCenterMercator;
		public Vector2d CenterMercator
		{
			get
			{
				return _mapCenterMercator;
			}
		}

		float _worldRelativeScale;

		public float WorldRelativeScale
		{
			get
			{
				return _worldRelativeScale;
			}
		}

		bool _worldHeightFixed = false;

		public event Action OnInitialized = delegate { };

		ILocationProvider _locationProvider;
		ILocationProvider LocationProvider
		{
			get
			{
				if (_locationProvider == null)
				{
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				}

				return _locationProvider;
			}
		}

		protected virtual void Awake()
		{
			_worldHeightFixed = false;
			_fileSouce = MapboxAccess.Instance;
			_tileProvider.OnTileAdded += TileProvider_OnTileAdded;
			_tileProvider.OnTileRemoved += TileProvider_OnTileRemoved;
			if (!_root)
			{
				_root = transform;
			}
		}

		void Start()
		{
			LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}

		void LocationProvider_OnLocationUpdated(object sender, Unity.Location.LocationUpdatedEventArgs e)
		{
			LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_mapCenterLatitudeLongitude = e.Location;
			Build();
		}

		protected virtual void OnDestroy()
		{
			if (_tileProvider != null)
			{
				_tileProvider.OnTileAdded -= TileProvider_OnTileAdded;
				_tileProvider.OnTileRemoved -= TileProvider_OnTileRemoved;
			}

			_mapVisualizer.Destroy();
		}

		void Build()
		{
			var referenceTileRect = Conversions.TileBounds(TileCover.CoordinateToTileId(_mapCenterLatitudeLongitude, _zoom));
			_mapCenterMercator = referenceTileRect.Center;
			_mapVisualizer.Initialize(this, _fileSouce);
			_tileProvider.Initialize(this);
#if !UNITY_EDITOR
			//_root.localPosition = -Conversions.GeoToWorldPosition(_mapCenterLatitudeLongitude.x, _mapCenterLatitudeLongitude.y, _mapCenterMercator, 1f).ToVector3xz();
#endif
			var relativeScale =  Mathf.Cos(Mathf.Deg2Rad * (float)_mapCenterLatitudeLongitude.x);
			_root.localScale = Vector3.one * relativeScale;
			_worldRelativeScale = relativeScale;
			OnInitialized();
		}

		void TileProvider_OnTileAdded(UnwrappedTileId tileId)
		{
			if (_snapMapHeightToZero && !_worldHeightFixed)
			{
				_worldHeightFixed = true;
				var tile = _mapVisualizer.LoadTile(tileId);
				if (tile.HeightDataState == MeshGeneration.Enums.TilePropertyState.Loaded)
				{
					var h = tile.QueryHeightData(.5f, .5f);
					Root.transform.position = new Vector3(
					 Root.transform.position.x,
					 -h * WorldRelativeScale,
					 Root.transform.position.z);
				}
				else
				{
					tile.OnHeightDataChanged += (s) =>
					{
						var h = s.QueryHeightData(.5f, .5f);
						Root.transform.position = new Vector3(
						 Root.transform.position.x,
						 -h * WorldRelativeScale,
						 Root.transform.position.z);
					};
				}
			}
			else
			{
				_mapVisualizer.LoadTile(tileId);
			}
		}

		void TileProvider_OnTileRemoved(UnwrappedTileId tileId)
		{
			_mapVisualizer.DisposeTile(tileId);
		}
	}
}