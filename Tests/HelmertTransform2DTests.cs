using jp.co.gatari.GlobalCoordinate.Runtime.Data;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using UnityEngine;

namespace jp.co.gatari.GlobalCoordinate.Tests
{
    public class HelmertTransform2DTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Passes()
        {
            var t = new HelmertTransform2D();
            t.Initialize(new HelmertTransform2DData[]
            {
                new(316.578, 301.545, -37548.103, -21027.030),
                new(318.129, 314.027, -37541.115, -21016.568),
                new(311.536, 314.648, -37546.728, -21013.058)
            });

            t.Resolve();

            var projected = t.Transform(new Vector2D(303.011, 304.983));
            Debug.Log($"original: ${(303.011, 304.983)}");
            Debug.Log(projected);

            Assert.AreEqual(-37558.685, projected.X, 0.001);
            Assert.AreEqual(-21017.870, projected.Y, 0.001);

            var inverseProjected = t.TransformInverse(new Vector2D(-37558.685, -21017.870));
            Debug.Log(inverseProjected);
            Assert.AreEqual(303.011, inverseProjected.X, 0.05);
            Assert.AreEqual(304.983, inverseProjected.Y, 0.05);
        }
    }
}