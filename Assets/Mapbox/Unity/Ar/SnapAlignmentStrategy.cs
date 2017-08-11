namespace Mapbox.Unity.Ar
{
	using UnityEngine;

	public class SnapAlignmentStrategy : AbstractAlignmentStrategy
	{
		[SerializeField]
		Transform _pivot;

		public override void Align(Alignment alignment)
		{
			_transform.rotation = Quaternion.Euler(0, alignment.Rotation, 0);
			_transform.localPosition = alignment.Position;
		}
	}
}