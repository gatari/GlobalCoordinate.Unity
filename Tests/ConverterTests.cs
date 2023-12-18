using System;
using GlobalCoordinate.Runtime.Data;
using GlobalCoordinate.Runtime.Foundation;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace jp.co.gatari.GlobalCoordinate.Tests
{
    public class ConverterTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ConvertToEcef_Passes()
        {
            // Arrange
            double longitude = 135; // 45 degrees
            double latitude = 35; // 45 degrees
            double altitude = 10; // 10 meters

            // Act
            var result = CoordinateConverter.ConvertToEcef(longitude, latitude, altitude);

            // Assert
            // These expected values are for illustration purposes.
            // In a real test, you would use known values from a trusted source or calculation.
            var expectedPosition = new Vector3D(-3698476.0795, 3698476.0795, 3637872.6451);
            // var expectedRotation = Quaterniond.identity.ToVector4d();

            Assert.AreEqual(expectedPosition.X, result.X, 0.001);
            Assert.AreEqual(expectedPosition.Y, result.Y, 0.001);
            Assert.AreEqual(expectedPosition.Z, result.Z, 0.001);
        }

        [Test]
        public void ConvertEnuToGeodetic_Passes()
        {
            // Arrange
            const int longitude = 135; // 45 degrees
            const int latitude = 35; // 45 degrees
            const int altitude = 0; // 100 meters

            var sourceLlaPose = new GeodeticCoordinate()
            {
                Latitude = latitude,
                Longitude = longitude,
                Altitude = altitude,
                Rotation = MathNet.Spatial.Euclidean.Quaternion.One
            };

            Debug.Log("source lla: " + sourceLlaPose);

            // create target 10 m in front of anchor
            var pose = new Pose()
            {
                position = Vector3.zero + Vector3.up * 100 + Vector3.forward * 100 + Vector3.right * 100,
                rotation = Quaternion.identity
            };

            // Act
            var result = CoordinateConverter.ConvertEnuPositionToGeodetic(sourceLlaPose, pose.position.ToEcrVector());

            Assert.AreEqual(35.0009, result.Latitude, 0.0001);
            Assert.AreEqual(135.0011, result.Longitude, 0.0001);
            Assert.AreEqual(100, result.Altitude, 0.01);
        }

        [Test]
        public void GetAngleOfTwoPoint_Passes()
        {
            var p1 = new GeodeticCoordinate()
            {
                Latitude = 35.000,
                Longitude = 135.00,
                Altitude = 0,
            };

            var p2 = new GeodeticCoordinate()
            {
                Latitude = 35.0009,
                Longitude = 135.0011,
                Altitude = 0,
            };

            var enuPosition = CoordinateConverter.GetEnuPosition(p1, p2);
            Assert.AreEqual(45, Math.Atan2(enuPosition.Y, enuPosition.X) * 180 / Math.PI, 0.5);
        }
    }
}