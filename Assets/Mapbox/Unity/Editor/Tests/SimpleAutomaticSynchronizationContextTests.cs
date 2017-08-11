namespace Mapbox.Ar.Tests
{
	using NUnit.Framework;
	using UnityEngine;
	using Mapbox.Unity.Location;
	using Mapbox.Unity.Ar;

	[TestFixture]
	public class SimpleAutomaticSynchronizationContextTests
	{
		float _epsilom = 0.01f;
		SimpleAutomaticSynchronizationContext _synchronizationContext;

		[SetUp]
		public void Init()
		{
			_synchronizationContext = new SimpleAutomaticSynchronizationContext();

			// Less than one for test readability, but realisticly we want this > 1.
			_synchronizationContext.MinimumDeltaDistance = .1f;
		}

		[Test]
		public void CannotCalibrateWithoutAtLeastTwoNodes()
		{
			bool alignmentAvailable = false;
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignmentAvailable = true;

			var location1 = new Location();
			location1.Position = new Vector3(0, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			Assert.IsFalse(alignmentAvailable);

			var location2 = new Location();
			location2.Position = new Vector3(0, 0, 1);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 1));

			Assert.IsTrue(alignmentAvailable);
		}

		[Test]
		public void ComputesAlignment()
		{
			var alignment = new Alignment();
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignment = obj;

			// We physically walk forward.
			// And we started 1 unit east of the center of the map.
			var location1 = new Location();
			location1.Position = new Vector3(1, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			// But forward was actually East.
			// (GPS gets converted to Unity space, relative to the map's origin.)
			var location2 = new Location();
			location2.Position = new Vector3(2, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 1));

			// Therefore, we must rotate the map counter-clockwise 90 degrees.
			Assert.AreEqual(-90, alignment.Rotation);

			// And our original offset means the rotated map should move 1 unit backwards.
			Assert.LessOrEqual((new Vector3(0, 0, -1) - alignment.Position).magnitude, _epsilom);

			// But we drift over distance (AR can't track global position as accurately as GPS, generally speaking).
			// 80 total units moved, according to AR.
			// 100 total units moved, according to GPS.
			var location3 = new Location();
			location3.Position = new Vector3(101, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 80));

			// Therefore, we must move the map 21 units backwards.
			// (Remember the 1 unit offset to begin with.)
			// Yes, this means that objects that are now farther away are not aligned with reality.
			// Lucky for us, we're interested in our immediate surroundings.
			Assert.LessOrEqual((new Vector3(0, 0, -21) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		public void ComputeDoesNotHappenIfMinimumDeltaDistanceIsNotMet()
		{
			bool alignmentAvailable = false;
			var alignment = new Alignment();

			_synchronizationContext.OnAlignmentAvailable += (obj) =>
			{
				alignmentAvailable = true;
				alignment = obj;
			};

			// Ignore minor AR movement updates if GPS is wild.
			// For example, the first GPS update from the device is usually wrong.
			_synchronizationContext.MinimumDeltaDistance = 2f;

			var location1 = new Location();
			location1.Position = new Vector3(0, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			var location2 = new Location();
			location2.Position = new Vector3(0, 0, 1);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 1));

			// We added two nodes, but we should not be calibrated because of the threshold.
			Assert.IsFalse(alignmentAvailable);

			var location3 = new Location();
			location3.Position = new Vector3(0, 0, 3);
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 2));

			Assert.LessOrEqual((new Vector3(0, 0, -1) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		public void SynchronizationBiasWeightsAlignment()
		{
			var alignment = new Alignment();
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignment = obj;

			_synchronizationContext.SynchronizationBias = 0f;

			// For simplicity, we will assume heading and origins are aligned.
			// But, we want to bias our results toward AR.
			var location1 = new Location();
			location1.Position = new Vector3(0, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			// GPS moved too far.
			var location2 = new Location();
			location2.Position = new Vector3(0, 0, 2);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 1));

			// But due to our bias, we want expect no change.
			Assert.LessOrEqual((new Vector3(0, 0, 0) - alignment.Position).magnitude, _epsilom);

			// Let's see if other bias values work.
			_synchronizationContext.SynchronizationBias = .5f;

			var location3 = new Location();
			location3.Position = new Vector3(0, 0, 4);
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 2));

			// We expect to between our last AR and GPS nodes, or an offset of .5 units backwards, plus the original original offset of 1
			Assert.LessOrEqual((new Vector3(0, 0, -1.5f) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		public void SynchronizationBiasDoesNotWeightInitialOffset()
		{
			var alignment = new Alignment();
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignment = obj;

			_synchronizationContext.SynchronizationBias = 0f;

			// Origins and headings are misaligned to mimic a realistic scenario.
			// We still want to bias our results toward AR.
			var location1 = new Location();
			location1.Position = new Vector3(1, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			// GPS moved too far.
			var location2 = new Location();
			location2.Position = new Vector3(3, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 1));

			// But due to our orinal offset and our bias, we expect to be offset backwards 1 unit.
			Assert.LessOrEqual((new Vector3(0, 0, -1) - alignment.Position).magnitude, _epsilom);

			_synchronizationContext.SynchronizationBias = 1f;
			var location3 = new Location();
			location3.Position = new Vector3(5, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 2));

			Assert.LessOrEqual((new Vector3(0, 0, -3) - alignment.Position).magnitude, _epsilom);

			_synchronizationContext.SynchronizationBias = .5f;
			var location4 = new Location();
			location4.Position = new Vector3(6, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location4, new Vector3(0, 0, 3));

			// (6 - 3) + ((6 - 5) - (3 -2))
			Assert.LessOrEqual((new Vector3(0, 0, -3f) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		public void LocationAccuracyWeightsPosition()
		{
			var alignment = new Alignment();
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignment = obj;

			_synchronizationContext.ArTrustRange = 10f;
			_synchronizationContext.UseAutomaticSynchronizationBias = true;

			// We have an initial offset of 0.
			var location1 = new Location();
			location1.Position = new Vector3(0, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			var location2 = new Location();
			location2.Position = new Vector3(0, 0, 10);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 10));

			// Our first update should just find the offset.
			Assert.LessOrEqual((new Vector3(0, 0, 0) - alignment.Position).magnitude, _epsilom);

			// GPS went 10 units further.
			var location3 = new Location();
			location3.Position = new Vector3(0, 0, 40);
			location3.Accuracy = 5f;
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 30));

			// We will bias towards GPS since our AR delta is so high.
			Assert.LessOrEqual((new Vector3(0, 0, -7.5f) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		public void BadGpsAccuracyDoesNotWeightPosition()
		{
			var alignment = new Alignment();
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignment = obj;

			_synchronizationContext.ArTrustRange = 10f;
			_synchronizationContext.UseAutomaticSynchronizationBias = true;

			var location1 = new Location();
			location1.Position = new Vector3(0, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			// No initial offset.
			var location2 = new Location();
			location2.Position = new Vector3(0, 0, 10);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 10));

			// Bad GPS update!
			var location3 = new Location();
			location3.Position = new Vector3(0, 0, 30);
			location3.Accuracy = 60f;
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 20));

			// If we ignore GPS, and we had zero initial offset, we should still have zero offset.
			Assert.LessOrEqual((new Vector3(0, 0, 0) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		public void GoodGpsAccuracyFullyWeightsPosition()
		{
			var alignment = new Alignment();
			_synchronizationContext.OnAlignmentAvailable += (obj) => alignment = obj;

			// Our trust range is huge. This will mean we get some location updates that are fully enclosed in our trust radius.
			_synchronizationContext.ArTrustRange = 30f;
			_synchronizationContext.UseAutomaticSynchronizationBias = true;

			// No initial offset.
			var location1 = new Location();
			location1.Position = new Vector3(0, 0, 0);
			_synchronizationContext.AddSynchronizationNodes(location1, new Vector3(0, 0, 0));

			var location2 = new Location();
			location2.Position = new Vector3(0, 0, 10);
			_synchronizationContext.AddSynchronizationNodes(location2, new Vector3(0, 0, 10));

			// Solid GPS update!
			var location3 = new Location();
			location3.Position = new Vector3(0, 0, 30);
			location3.Accuracy = 5f;
			_synchronizationContext.AddSynchronizationNodes(location3, new Vector3(0, 0, 20));

			// If we use just GPS, and we had zero initial offset, we should have a -10 offset.
			// (20 - 10) - (30 - 10)
			Assert.LessOrEqual((new Vector3(0, 0, -10) - alignment.Position).magnitude, _epsilom);
		}

		[Test]
		[Ignore("TODO")]
		public void WalkingInCirclesDoesNotBreakAlignment()
		{
			// TODO: because of GPS accuracy, making sharp turns/walking in circles can confuse the computed heading.
		}

		[Test]
		[Ignore("TODO")]
		public void AlignmentFailsIfRelativeAnglesExceedThreshold()
		{
			// TODO: if we move backwards right before getting a GPS update, we could potentially "inverse" our calculated heading.
		}

		[Test]
		[Ignore("TODO")]
		public void LocationAccuracyWeightsRotation()
		{
			// TODO: Ideally, our computed heading should be weighted by the accuracy, just as position is.
		}

		[Test]
		[Ignore("TODO")]
		public void RotationIsBasedOnWeightedMovingAverage()
		{
			// TODO: use weighted moving average for heading (this will alleviate sudden, drastic changes).
		}

		[Test]
		[Ignore("TODO")]
		public void NodeOffsetsAreBasedOnWeightedMovingAverage()
		{
			// TODO: use weighted average of gps nodes (accuracy + time), and  weighted average of ar nodes (time, with earliest being weighted highest).
		}

		// TODO: (UX) hold device forward as you walk forward (use compass to help teach).
	}
}