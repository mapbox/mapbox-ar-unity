using UnityEngine;
using UnityEngine.XR.iOS;
using Mapbox.Unity.Utilities;

public class ParkElement : MonoBehaviour
{
	
	public void Start()
	{

		var _mask = LayerMask.GetMask("Path");
		var origin = transform.position;
		origin.y += 10000f;
		var ray = new Ray(origin, Vector3.down);
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _mask))
		{
			Destroy(gameObject);
		}
	}

}