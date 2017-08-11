namespace Mapbox.Unity.Ar
{
	using System;
	using Mapbox.Unity.Location;
	using UnityEngine;

	public interface ISynchronizationContext
	{
		event Action<Alignment> OnAlignmentAvailable;
		void AddSynchronizationNodes(Location gpsNode, Vector3 arNode);
	}

	public struct Alignment
	{
		public Vector3 Position;
		public float Rotation;
	}
}
