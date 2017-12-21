namespace Mapbox.Unity.Ar
{
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;
	using UnityEngine;

	public class AverageHeadingAlignmentStrategy : AbstractAlignmentStrategy
	{
		[SerializeField]
		int _maxSamples = 5;

		[SerializeField]
		float _ignoreAngleThreshold = 15f;

		[SerializeField]
		float _lerpSpeed;

		List<float> _rotations = new List<float>();

		float _averageRotation;
		Quaternion _targetRotation;
		Vector3 _targetPosition;

		public override void OnAlignmentAvailable(Alignment alignment)
		{
			var count = _rotations.Count;
			var rotation = alignment.Rotation;

			// TODO: optimize circular list.
			if (count >= _maxSamples)
			{
				_rotations.RemoveAt(0);
			}

			if (rotation < 0)
			{
				rotation += 360;
			}

			_rotations.Add(rotation);

			var total = 0f;
			foreach (var r in _rotations)
			{
				total += r;
			}

			_averageRotation = total / _rotations.Count;

			if (Mathf.Abs(Mathf.DeltaAngle(rotation, _averageRotation)) < _ignoreAngleThreshold)
			{
				Console.Instance.Log(string.Format("Average Heading: {0}", _averageRotation), "aqua");
				_targetRotation = Quaternion.Euler(0, _averageRotation, 0);
				_targetPosition = alignment.Position;

				// HACK: Undo the original expected position.
				_targetPosition = Quaternion.Euler(0, -rotation, 0) * _targetPosition;

				// Add our averaged rotation.
				_targetPosition = Quaternion.Euler(0, _averageRotation, 0) * _targetPosition;
			}
			else
			{
				Console.Instance.Log("Ignoring alignment (^) due to poor angle!", "red");
			}
		}

		// FIXME: this should be in a coroutine, which is activated in Align.
		void Update()
		{
			var t = _lerpSpeed * Time.deltaTime;
			_transform.SetPositionAndRotation(
				Vector3.Lerp(_transform.localPosition, _targetPosition, t),
				Quaternion.Lerp(_transform.localRotation, _targetRotation, t));
		}
	}
}
