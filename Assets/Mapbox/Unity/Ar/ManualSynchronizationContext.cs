namespace Mapbox.Unity.Ar
{
	using UnityEngine;
	using Mapbox.Unity.Location;
	using System;

	public class ManualSynchronizationContext : ISynchronizationContext
	{
		public event Action<Alignment> OnAlignmentAvailable;

		public void AddSynchronizationNodes(Location gpsNode, Vector3 arNode)
		{
			var alignment = new Alignment();
			// TODO: manually add a position and heading offset.

			OnAlignmentAvailable(alignment);
		}
	}
}