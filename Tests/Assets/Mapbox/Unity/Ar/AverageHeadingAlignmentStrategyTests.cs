using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mapbox.Unity.Ar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapbox.Unity.Ar.Tests
{
    [TestClass()]
    public class AverageHeadingAlignmentStrategyTests
    {
        double tolerance = 0.00001;

        [TestMethod()]
        public void meanAngleOver360()
        {
            List<float> rotations = new List<float>();
            rotations.Add(10);
            rotations.Add(350);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

            Assert.AreEqual(0, avgRotation, tolerance);
        }

        [TestMethod()]
        public void meanAngleOver360Negative()
        {
            List<float> rotations = new List<float>();
            rotations.Add(-10);
            rotations.Add(10);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

            Assert.AreEqual(0, avgRotation, tolerance);
        }

        [TestMethod()]
        public void meanAngleAllPositive()
        {
            List<float> rotations = new List<float>();
            rotations.Add(10);
            rotations.Add(20);
            rotations.Add(30);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);
            
            Assert.AreEqual(20, avgRotation, tolerance);
        }

        [TestMethod()]
        public void meanAngleAllNegative()
        {
            List<float> rotations = new List<float>();
            rotations.Add(-10);
            rotations.Add(-20);
            rotations.Add(-30);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

            Assert.AreEqual(-20, avgRotation, tolerance);
        }

        [TestMethod()]
        public void meanAngleSameAngleDifferentForms()
        {
            List<float> rotations = new List<float>();
            rotations.Add(270);
            rotations.Add(-90);
            rotations.Add(360 + 270);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

            Assert.AreEqual(-90, avgRotation, tolerance);
        }

        [TestMethod()]
        public void meanAnglePositiveAndNegative()
        {
            List<float> rotations = new List<float>();
            rotations.Add(-80);
            rotations.Add(80);
            rotations.Add(179);
            rotations.Add(-179);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

            Assert.AreEqual(180, avgRotation, tolerance);
        }

        [TestMethod()]
        // For consistency, angles returned are always within (-180, 180].
        // Maybe counterintuitive if inputs were > 180, so this test is here to highlight this behaviour.
        public void meanAngleWithin180OfZero()
        {
            List<float> rotations = new List<float>();
            rotations.Add(270);
            rotations.Add(270);
            float avgRotation = (float)AverageHeadingAlignmentStrategy.meanAngle(rotations);

            Assert.AreEqual(-90, avgRotation, tolerance);
        }

        [TestMethod()]
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

            Assert.AreEqual(90, avgRotation, tolerance);
        }

    }
}