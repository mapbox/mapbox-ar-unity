using UnityEngine;

public class Drag : MonoBehaviour
{
	[SerializeField]
	Camera _referenceCamera;

	[SerializeField]
	bool _inverseDrag;

	Vector3 _origin;
	Vector3 _delta;
	bool _shouldDrag;

	void LateUpdate()
	{
		if (Input.GetMouseButton(0))
		{
			var mousePosition = Input.mousePosition;
			mousePosition.z = _referenceCamera.transform.localPosition.y;
			_delta = _referenceCamera.ScreenToWorldPoint(mousePosition) - _referenceCamera.transform.localPosition;
			if (_shouldDrag == false)
			{
				_shouldDrag = true;
				_origin = _referenceCamera.ScreenToWorldPoint(mousePosition);
			}
		}
		else
		{
			_shouldDrag = false;
		}

		if (_shouldDrag == true)
		{
			transform.localPosition = _origin - _delta;
		}
	}

}