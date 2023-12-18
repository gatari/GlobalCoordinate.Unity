using GlobalCoordinate.Runtime.Data;
using GlobalCoordinate.Runtime.Foundation;
using NUnit.Framework;
using UnityEngine;

namespace jp.co.gatari.GlobalCoordinate.Tests
{
    public class UnityEnuCoordinateSystemTests
    {
        [Test]
        public void CalculateFromGeodeticCoordinate_Passes()
        {
            var coordinateSystem = new UnityEnuCoordinateSystem();

            var originGeodeticCoordinate = new GeodeticCoordinate()
            {
                Altitude = 0,
                Latitude = 35,
                Longitude = 135,
                Rotation = MathNet.Spatial.Euclidean.Quaternion.One
            };

            // 東に100m、北に100m移動した座標
            var secondaryGeodeticCoordinate = new GeodeticCoordinate()
            {
                Altitude = 30,
                Latitude = 35.0009,
                Longitude = 135.0011,
                Rotation = MathNet.Spatial.Euclidean.Quaternion.One
            };

            coordinateSystem.CalculateFromGeodeticCoordinate(originGeodeticCoordinate, Vector3.zero,
                secondaryGeodeticCoordinate, Vector3.forward);

            Debug.Log(coordinateSystem.E);
            Debug.Log(coordinateSystem.N);
            Debug.Log(coordinateSystem.U);
        }

        [Test]
        public void CalculateFromTransformAndGeodeticCoordinate_Passes()
        {
            var coordinateSystem = new UnityEnuCoordinateSystem();

            var euler = Quaternion.Euler(0, 45, 0).ToEnuQuaternion().ToEulerAngles();
            Debug.Log("euler: " + euler.Alpha + ", " + euler.Beta + ", " + euler.Gamma);

            var originGeodeticCoordinate = new GeodeticCoordinate()
            {
                Altitude = 0,
                Latitude = 35,
                Longitude = 135,
                Rotation = Quaternion.Euler(0, 45, 0).ToEnuQuaternion()
            };

            var gameObject = new GameObject
            {
                transform =
                {
                    rotation = Quaternion.Euler(0, -45, 0)
                }
            };

            coordinateSystem.CalculateFromTransformAndGeodeticCoordinate(gameObject.transform,
                originGeodeticCoordinate);

            Debug.Log(coordinateSystem.E);
            Debug.Log(coordinateSystem.N);
            Debug.Log(coordinateSystem.U);
        }
    }
}