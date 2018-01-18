

namespace Mapbox.Unity.Ar
{

	using NUnit.Framework;
	using System.Collections.Generic;


	[TestFixture]
	internal class AverageHeadingAlignmentStrategyTests 
	{

		private double _tolerance = 0.00001;


		[Test]
		public void meanAngleOver360()
		{
			List<float> rotations = new List<float>();
			rotations.Add(10);
			rotations.Add(350);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(0, avgRotation, _tolerance);
		}


		[Test]
		public void meanAngleOver360Negative()
		{
			List<float> rotations = new List<float>();
			rotations.Add(-10);
			rotations.Add(10);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(0, avgRotation, _tolerance);
		}


		[Test]
		public void meanAngleAllPositive()
		{
			List<float> rotations = new List<float>();
			rotations.Add(10);
			rotations.Add(20);
			rotations.Add(30);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(20, avgRotation, _tolerance);
		}


		[Test]
		public void meanAngleAllNegative()
		{
			List<float> rotations = new List<float>();
			rotations.Add(-10);
			rotations.Add(-20);
			rotations.Add(-30);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(-20, avgRotation, _tolerance);
		}


		[Test]
		public void meanAngleSameAngleDifferentForms()
		{
			List<float> rotations = new List<float>();
			rotations.Add(270);
			rotations.Add(-90);
			rotations.Add(360 + 270);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(-90, avgRotation, _tolerance);
		}


		[Test]
		public void meanAnglePositiveAndNegative()
		{
			List<float> rotations = new List<float>();
			rotations.Add(-80);
			rotations.Add(80);
			rotations.Add(179);
			rotations.Add(-179);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(180, avgRotation, _tolerance);
		}


		[Test]
		// For consistency, angles returned are always within (-180, 180].
		// Maybe counterintuitive if inputs were > 180, so this test is here to highlight this behaviour.
		public void meanAngleWithin180OfZero()
		{
			List<float> rotations = new List<float>();
			rotations.Add(270);
			rotations.Add(270);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(-90, avgRotation, _tolerance);
		}


		[Test]
		//Test to check we avoid the common (wrong) "solution": (sum(angles)%360) / count(angles)
		public void meanAngleFiveNinetys()
		{
			List<float> rotations = new List<float>();
			rotations.Add(90);
			rotations.Add(90);
			rotations.Add(90);
			rotations.Add(90);
			rotations.Add(90);
			float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

			Assert.AreEqual(90, avgRotation, _tolerance);
		}



	}
}