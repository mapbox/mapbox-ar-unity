namespace Mapbox.Unity.Ar
{
	using UnityEngine;

	public abstract class AbstractAlignmentStrategy : MonoBehaviour
	{
		[SerializeField]
		protected Transform _transform;

		public abstract void Align(Alignment alignment);
	}
}