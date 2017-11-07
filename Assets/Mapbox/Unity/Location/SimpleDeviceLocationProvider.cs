namespace Mapbox.Unity.Location
{
	using UnityEngine;
	public class SimpleDeviceLocationProvider : AbstractLocationProvider
	{
		void Awake()
		{
			Input.location.Start(5f, 5f);
		}

		void Update()
		{
			if (Input.location.isEnabledByUser)// && Input.location.status == LocationServiceStatus.Running)
			{
				var location = new Location();
				var lat = Input.location.lastData.latitude;
				var lon = Input.location.lastData.longitude;
				location.LatitudeLongitude = new Utils.Vector2d(lat, lon);
				location.IsLocationUpdated = true;
				location.Accuracy = (int)Input.location.lastData.horizontalAccuracy;
				SendLocation(location);
			}
		}
	}
}
