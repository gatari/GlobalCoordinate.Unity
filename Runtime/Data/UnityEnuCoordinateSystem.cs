using System;
using GlobalCoordinate.Runtime.Foundation;
using MathNet.Spatial.Euclidean;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace GlobalCoordinate.Runtime.Data
{
    public class UnityEnuCoordinateSystem
    {
        public Vector3 E { get; set; }
        public Vector3 N { get; set; }
        public Vector3 U { get; set; } = Vector3.up;
        public GeodeticCoordinate OriginGeodeticCoordinate { get; set; }

        public Vector3 OriginPosition { get; set; }
        public bool IsInitialized { get; set; }

        public event EventHandler CoordinateUpdated;

        public void CalculateFromGeodeticCoordinate(GeodeticCoordinate originGeodeticCoordinate,
            Vector3 originUnityPosition, GeodeticCoordinate secondaryGeodeticCoordinate,
            Vector3 secondaryUnityPosition)
        {
            OriginGeodeticCoordinate = originGeodeticCoordinate;
            var enu = CoordinateConverter.GetEnuPosition(originGeodeticCoordinate, secondaryGeodeticCoordinate);

            // enuベクトルのEに対しての角度（反時計回り）
            var angle = Math.Atan2(enu.Y, enu.X);

            // Unity ENUベクトルの計算
            var vec = secondaryUnityPosition - originUnityPosition;

            // UnityY軸方向時計回りに修正
            E = Quaternion.Euler(0, (float)angle * Mathf.Rad2Deg, 0) * vec;
            // EからU成分を除外してnormalize
            E = (E - Vector3.Dot(E, Vector3.up) * Vector3.up).normalized;

            // UnityY軸方向時計回りに修正
            N = (Quaternion.Euler(0, (float)angle * Mathf.Rad2Deg, 0) * Quaternion.Euler(0, -90, 0) * vec).normalized;
            // NからU成分を除外してnormalize
            N = (N - Vector3.Dot(N, Vector3.up) * Vector3.up).normalized;

            U = Vector3.up;

            Debug.Log($"CalculateFromGeodeticCoordinate E: {E}, N: {N}, U: {U}");

            IsInitialized = true;
            CoordinateUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void CalculateFromTransformAndGeodeticCoordinate(Transform originTransform,
            GeodeticCoordinate geodeticCoordinate)
        {
            OriginPosition = originTransform.position;
            OriginGeodeticCoordinate = geodeticCoordinate;

            var unityRot = OriginGeodeticCoordinate.Rotation.ToUnityQuaternion();
            E = (Quaternion.Inverse(unityRot) * originTransform.right).normalized;
            N = (Quaternion.Inverse(unityRot) * originTransform.forward).normalized;
            U = (Quaternion.Inverse(unityRot) * originTransform.up).normalized;

            Debug.Log($"CalculateFromTransformAndGeodeticCoordinate E: {E}, N: {N}, U: {U}");

            IsInitialized = true;
            CoordinateUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// Unity Position => Geodetic without Rotation
        public GeodeticCoordinate UnityPositionToGeodetic(Vector3 worldPosition)
        {
            var relativePosition = worldPosition - OriginPosition;
            var enuPosition = new Vector3D(Vector3.Dot(relativePosition, E), Vector3.Dot(relativePosition, N),
                Vector3.Dot(relativePosition, U));
            return CoordinateConverter.ConvertEnuPositionToGeodetic(OriginGeodeticCoordinate, enuPosition);
        }

        /// Unity Pose => Geodetic with Rotation
        public GeodeticCoordinate UnityPoseToGeodetic(Pose worldPose)
        {
            var relativePosition = worldPose.position - OriginPosition;
            var enuPosition = new Vector3D(Vector3.Dot(relativePosition, E), Vector3.Dot(relativePosition, N),
                Vector3.Dot(relativePosition, U));
            var enuRotation = worldPose.rotation * Quaternion.Inverse(Quaternion.LookRotation(N, U));

            return CoordinateConverter.ConvertEnuPoseToGeodetic(OriginGeodeticCoordinate, enuPosition,
                enuRotation.ToEnuQuaternion());
        }

        /// Geodetic => Unity
        public Pose GeodeticToUnityPose(GeodeticCoordinate geodeticCoordinate)
        {
            var enu = CoordinateConverter.GetEnuPosition(OriginGeodeticCoordinate, geodeticCoordinate);

            return new Pose()
            {
                position = OriginPosition + E * (float)enu.X + N * (float)enu.Y + U * (float)enu.Z,
                rotation = geodeticCoordinate.Rotation.ToUnityQuaternion() * Quaternion.LookRotation(N, U)
            };
        }
    }
}