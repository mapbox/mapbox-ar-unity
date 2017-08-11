namespace Mapbox.Unity.Map
{
	using UnityEngine;
	using Mapbox.Map;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;

	public class RangeAroundTransformTileProvider : AbstractTileProvider
	{
		[SerializeField]
		private Transform locationMarker;

		[SerializeField]
		private int _visibleBuffer;

		[SerializeField]
		private int _disposeBuffer;

		private bool _initialized = false;
		private Vector2d _currentLatLng;
		private UnwrappedTileId _currentTile;
		private UnwrappedTileId _cachedTile;

		internal override void OnInitialized()
		{
			if (locationMarker == null)
			{
				Debug.LogError("TransformTileProvider: No location marker transform specified.");
				Destroy(this);
			}
			else
			{
				_initialized = true;
			}
		}

		private void Update()
		{
			if (!_initialized) return;

			_currentLatLng = locationMarker.localPosition.GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
			_currentTile = TileCover.CoordinateToTileId(_currentLatLng, _map.Zoom);

			if (!_currentTile.Equals(_cachedTile))
			{
				for (int x = _currentTile.X - _visibleBuffer; x <= (_currentTile.X + _visibleBuffer); x++)
				{
					for (int y = _currentTile.Y - _visibleBuffer; y <= (_currentTile.Y + _visibleBuffer); y++)
					{
						AddTile(new UnwrappedTileId(_map.Zoom, x, y));
					}
				}
				_cachedTile = _currentTile;
				Cleanup(_currentTile);
			}
		}

		private void Cleanup(UnwrappedTileId currentTile)
		{
			var count = _activeTiles.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				var tile = _activeTiles[i];
				bool dispose = false;
				dispose = tile.X > currentTile.X + _disposeBuffer || tile.X < _currentTile.X - _disposeBuffer;
				dispose = dispose || tile.Y > _currentTile.Y + _disposeBuffer || tile.Y < _currentTile.Y - _disposeBuffer;

				if (dispose)
				{
					RemoveTile(tile);
				}
			}
		}
	}
}